using Newtonsoft.Json;
using SanteDB.Core.Model;
using SanteDB.Core.Model.Acts;
using SanteDB.Core.Model.Entities;
using SanteDB.Core.Model.Roles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SanteDB.Matcher.Model
{
    /// <summary>
    /// Represents a match report
    /// </summary>
    [JsonObject, XmlType(nameof(MatchReport), Namespace = "http://santedb.org/matcher")]
    [XmlInclude(typeof(Act))]
    [XmlInclude(typeof(TextObservation))]
    [XmlInclude(typeof(CodedObservation))]
    [XmlInclude(typeof(QuantityObservation))]
    [XmlInclude(typeof(PatientEncounter))]
    [XmlInclude(typeof(SubstanceAdministration))]
    [XmlInclude(typeof(Entity))]
    [XmlInclude(typeof(Patient))]
    [XmlInclude(typeof(Person))]
    [XmlInclude(typeof(Provider))]
    [XmlInclude(typeof(Organization))]
    [XmlInclude(typeof(Place))]
    [XmlInclude(typeof(Material))]
    [XmlInclude(typeof(ManufacturedMaterial))]
    [XmlInclude(typeof(DeviceEntity))]
    [XmlInclude(typeof(UserEntity))]
    [XmlInclude(typeof(ApplicationEntity))]
    public class MatchReport
    {

        /// <summary>
        /// The input
        /// </summary>
        [XmlElement("input"), JsonProperty("input")]
        public IdentifiedData Input { get; set; }

        /// <summary>
        /// The results for the matches
        /// </summary>
        [XmlElement("result"), JsonProperty("result")]
        public List<MatchResultReport> Results { get; set; }

    }
}
