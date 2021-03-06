﻿using Ladm.DataModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;

namespace Ladm
{
    public sealed class RegistrationOperations
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
            if (transaction.Status == Transaction.TransactionStatus.Lodged)
            {
                switch (transaction.TransactionType.Action)
                {
                    case TransactionMetaData.ActionCode.Create:
                        completeCreateRegistration(transaction, context);
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
        }
        /// <summary>
        /// For each properties of transaction cancel RRR of transaction right type
        /// (at usual it's no sence with target separation in regular and cancellation transactions)
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="context"></param>
        private static void completeCancelRegistration(Transaction transaction, LadmDbContext context)
        {
            /// filter out source SpatialUnits from trasaction
            var sourceSU = transaction.GetSourceProperties();

            var filter = transaction.Properties.SelectMany(la => la.SpatialUnits).ToList().ConvertAll<string>(item => item.SuId);
            
            
            var RightTypeFilter = transaction.TransactionType.RightType;

            var affected = GetActiveRRRsBySpatialUnits(context, filter, RightTypeFilter);
            
            foreach (var oldRRR in affected)
            {
                var newRRR = (RRR)Activator.CreateInstance(oldRRR.GetType());
                newRRR.LAUnit = oldRRR.LAUnit;
                /// LAUnit exists only as a prototype (in the transaction) or in an active RRR, 
                /// so it's own lifespan is tied to the RRR lifespan (1=1)
                oldRRR.LAUnit.EndLifeSpanVersion = 
                oldRRR.EndLifeSpanVersion = newRRR.BeginLifeSpanVersion = DateTime.Now;
                newRRR.Version = oldRRR.Version + 1;

                /// sometimes we have request to get first of a kind!
                newRRR.Origin = oldRRR.Origin;
                /// inherite creator
                newRRR.CreatedBy = oldRRR.CreatedBy;

                newRRR.CanExpire = oldRRR.CanExpire;
                newRRR.ExpirationDate = oldRRR.ExpirationDate;
                newRRR.StartDate = oldRRR.StartDate;
                newRRR.EndDate = DateTime.Now;
                /// mark by me
                newRRR.CancelledBy = transaction;

                context.RRRs.Add(newRRR);
            }

            var archivedSU = transaction.Properties.SelectMany(la => la.SpatialUnits).Where(su => su.Status == SpatialUnit.SpatialUnitStatus.Archived);

            if (archivedSU != null)
            {
                foreach (var asu in archivedSU)
                {
                    asu.EndLifeSpanVersion = DateTime.Now;
                }
            }
        }

        private static ICollection<RRR> GetActiveRRRsBySpatialUnits(LadmDbContext context, List<string> spatialUnitsSuIds, string rightTypeFilter)
        {
            var result = new List<RRR>(); 
            var allActiveRRR = context.RRRs.Where(activeRRR => activeRRR.BeginLifeSpanVersion != null
                                             && activeRRR.EndLifeSpanVersion == null
                                             //lets assume we have to work with only rrr type
                                             && activeRRR.TypeName == rightTypeFilter
                                             );
            // if we need su=RRR we may go
            // Dictionary<string, RRR> suid2RRR = new Dictionary<string, RRR>();
            allActiveRRR.Select(activeR => new { Right = activeR, SpatialUnits = activeR.LAUnit.SpatialUnits })
                .ToList()
                .ForEach(dynoR2S =>
                                (dynoR2S.SpatialUnits
                                    /// LADM supposed RRR may have empty SU ref
                                    /// but this case suposed it always applied!
                                    /// ??Enumerable.Empty<SpatialUnit>()
                                                    )
                                .Where(c => spatialUnitsSuIds.Contains(c.SuId))
                                .ToList()
                                .ForEach(su => result.Add(dynoR2S.Right)
                                    // if need su=RRR we may go    
                                    //suid2RRR[su.SuId] = dynoR2S.Right
                ));

            return result;
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
                party => transaction.TransactionType.TargetPartyRole.Equals(party.Role));
            
            var rrrType = Type.GetType("Ladm.DataModel." + transaction.TransactionType.RightType);
            /// Ladm dont restrict amount of LAUnits
            /// normal registration assumes each transaction have single LAUnit
            /// But for case of clarification of parts it may result in collections (as in case of multiply RRR ops)
            foreach (var party in targetParties)
            {
                var laUnits = transaction.GetPartyTargetLaUnit(party);

                if (laUnits.Count() == 0)
                    throw new ArgumentException("Can't proceed registration transaction w/o target properties assigned to target party.");
                /// we have to process new SU there and update them
                foreach (var laUnit in laUnits)
                {
                    laUnit.SpatialUnits.ToList().Where(su => su.Status == SpatialUnit.SpatialUnitStatus.New).ToList().
                        ForEach(newSU => newSU.BeginLifeSpanVersion = DateTime.Now);
                    laUnit.BeginLifeSpanVersion = DateTime.Now;

                    var rrr = (RRR)Activator.CreateInstance(rrrType);
                    rrr.Origin = rrr.CreatedBy = transaction;
                    rrr.BeginLifeSpanVersion = DateTime.Now;
                    rrr.Party = party;
                    rrr.LAUnit = laUnit;

                    rrr.StartDate = transaction.StartDate;
                    //if rrr meta expirable
                    rrr.ExpirationDate = transaction.ExpirationDate;

                    //there shoud proceed handler to implement right dependent attribute filling
                    //BuissnessLogicProvider.RegisterRRRHandler(transaction,rrr)
                    context.RRRs.Add(rrr);
                }
            }
            // versionaise prev LAUnits
            var sourcelaUnits = transaction.GetTransactionSourceLAUnits();
            if (sourcelaUnits != null)
            {
                sourcelaUnits.ToList().ForEach(la => la.EndLifeSpanVersion = DateTime.Now);
            }
            // actualise new SU (Implementation is not complete, cause version source property have to be used and updated
            var newSpatialUnits = transaction.Properties.SelectMany(la => la.SpatialUnits).Where(su => su.Status == SpatialUnit.SpatialUnitStatus.New);
            foreach (var nsu in newSpatialUnits)
            {
                nsu.Status = SpatialUnit.SpatialUnitStatus.Normal;
                nsu.BeginLifeSpanVersion = DateTime.Now;
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
