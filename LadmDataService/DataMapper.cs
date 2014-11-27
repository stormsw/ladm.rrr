using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Ladm.DataModel;
using LadmDataService.PresentationModel;

namespace LadmDataService
{
    public sealed class DataMapper
    {
        public static void Configure()
        {
            Mapper.CreateMap<SpatialUnit, PropertyPresentation>()
                .ForMember(presenter=>presenter.MembersCount,ops=>ops.MapFrom(src=>src.Members.Count));
        }
    }
}
