using System.Collections.Generic;
using System.Xml.Serialization;

namespace CheckingAnotherDevice.Models.Modules
{
    [XmlRoot("settingsmodule")]
    public class RkPkuModulesSettings
    {
        [XmlElement("module")]
        public List<ModuleSettings> ModulesSettings { get; set; } = new();
    }

    public class ModuleSettings
    {
        [XmlAttribute("id")]
        public int ModuleId { get; set; }

        [XmlAttribute("name")]
        public string ModuleName { get; set; }

        [XmlAttribute("connectionstring")]
        public string ConnectionString { get; set; }

        [XmlAttribute("mapversion")]
        public string MapVersion { get; set; }
    }
}
