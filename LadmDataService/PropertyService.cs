using AutoMapper;
using Ladm.DataModel;
using LadmDataService.PresentationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace LadmDataService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in both code and config file together.
    public class PropertyService : IPropertyService
    {
        public PropertyService()
        {
            DataMapper.Configure();
        }

        public PropertyPresentation GetPropertyInfo(string suid)
        {
            PropertyPresentation result = new PropertyPresentation();
            
            using (var context = new LadmDbContext())
            {
                var entity = (from su in context.SpatialUnits where su.SuId == suid && su.Status == SpatialUnit.SpatialUnitStatus.Normal select su).Single();
                Mapper.Map(entity, result);
            }

            if (result == null)
            {
                throw new Exception("SU is not found");
            }

            return result;
        }
    }
}
