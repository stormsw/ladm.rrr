using Ladm.DataModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;

namespace Ladm
{
    public class RegistrationOperations
    {
        /// <summary>
        /// Transaction status set to withdrawed. If transaction is Registration type
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="Context"></param>
        public static void WithdrawTransaction(Transaction transaction, DbContext Context)
        {
            transaction.Status = Transaction.TransactionStatus.Withdrawed;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="Context"></param>
        public static void CompleteTransaction(Transaction transaction, LadmDbContext Context)
        {
            switch (transaction.TransactionType.Meta)
            {
                case TransactionMetaData.MetaCode.Registration:
                    completeRegistrationTransaction(transaction, Context);
                    break;
                case TransactionMetaData.MetaCode.Documentation:
                    completeDocumentationTransaction(transaction, Context);
                    break;
                case TransactionMetaData.MetaCode.Technical:
                    completeTechnicalTransaction(transaction, Context);
                    break;
                default:
                    break;
            }

            transaction.Status = Transaction.TransactionStatus.Completed;
            Context.SaveChanges();
        }
        /// <summary>
        /// Selector for registrational routines based on Action type
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="context"></param>
        private static void completeRegistrationTransaction(Transaction transaction, LadmDbContext context)
        {
            switch (transaction.TransactionType.Action)
            {
                case TransactionMetaData.ActionCode.Create:
                    completeCreateRegistration(transaction,context);
                    break;
                case TransactionMetaData.ActionCode.Alter:
                    completeAlterRegistration(transaction, context);
                    break;
                case TransactionMetaData.ActionCode.Cancell:
                    completeCancelRegistration(transaction, context);
                    break;
                default:
                    throw new InvalidOperationException("Unknown action.");
                    //break;
            }
        }
        /// <summary>
        /// For each target properties of transaction cancel RRR of transaction right type
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="context"></param>
        private static void completeCancelRegistration(Transaction transaction, LadmDbContext context)
        {
            var sourceSU = transaction.GetSourceProperties();
            
            var srcLa = context.LAUnit.Where(item=>item.Properties.Intersect(sourceSU).Count()>0);

            var filteredRRR = context.RRRs.Where(item=>srcLa.Contains(item.LAUnit));

            foreach (var item in filteredRRR)
	        {
                item.CancelledBy = transaction;
	        }        
            
            throw new NotImplementedException();
        }
        /// <summary>
        /// Simplest way supposes to cancel old and create new
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="context"></param>
        private static void completeAlterRegistration(Transaction transaction, LadmDbContext context)
        {
            completeCancelRegistration(transaction, context);
            completeCreateRegistration(transaction, context);
        }
        /// <summary>
        /// For each target parties and properties register RRRs of transaction right type
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="context"></param>
        private static void completeCreateRegistration(Transaction transaction, LadmDbContext context)
        {
            var targetParties = transaction.Parties.Where(
                party=>transaction.TransactionType.TargetPartyRole.Equals(party.Role));

            var TargetProperties = transaction.GetTargetProperties();
            var rrrType = Type.GetType("Ladm."+ transaction.TransactionType.RightType);
            
            /*ObjectHandle handle = Activator.CreateInstance("Ladm", transaction.TransactionType.RightType);
                ///(transaction.TransactionType.RightType);
            var rrr = (RRR) handle.Unwrap();
            */
            /// Ladm dont restrict amount of LAUnits
            /// normal registration assumes each transaction have single LAUnit
            /// But for case of clarification of parts it may result in collections 
            foreach(var party in targetParties )
            {
                var laUnits = transaction.GetPartyTargetLaUnit(party);
                foreach (var laUnit in laUnits)
                {
                    var rrr = (RRR)Activator.CreateInstance(rrrType);
                    rrr.Origin = rrr.CreatedBy = transaction;
                    rrr.BeginLifeSpan = DateTime.Now;
                    rrr.Party = party;
                    rrr.LAUnit = laUnit;
                    //there shoud proceed handler to implement right dependent attribute filling
                    //BuissnessLogicProvider.RegisterRRRHandler(transaction,rrr)
                    context.RRRs.Add(rrr);
                }
            }

            context.SaveChanges();
        }
        #region Documentation and Technical
        /// <summary>
        /// 
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="Context"></param>
        private static void completeTechnicalTransaction(Transaction transaction, DbContext Context)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Documentation transaction affects
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="Context"></param>
        private static void completeDocumentationTransaction(Transaction transaction, DbContext Context)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
