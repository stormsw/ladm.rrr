using Ladm.DataModel;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace LadmDataService.PresentationModel
{
    [DataContract]
    public class PropertyPresentation
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string SuId { get; set; }
        [DataMember]
        public float Area { get; set; }
        [DataMember]
        public string Status { get; set; }
        [DataMember]
        public List<PropertyPresentation> Members { get; set; }
        [DataMember]
        public int MembersCount { get; set; }
    }
}
