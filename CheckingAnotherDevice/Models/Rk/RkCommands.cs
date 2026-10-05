using System.Collections.Generic;
using System.Xml.Serialization;

namespace CheckingAnotherDevice.Models.Rk
{
    [XmlRoot("commands")]
    public class RkCommands
    {
        [XmlElement("command")] 
        public List<RkCommand> Rk { get; set; } = new();
    }

    public class RkCommand 
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("modulename")]
        public string ModuleName { get; set; }

        [XmlAttribute("rknumber")]
        public int NumberRk { get; set; }

        [XmlAttribute("moduleid")]
        public int ModuleId { get; set; }

        [XmlAttribute("duration")]
        public ushort Duration { get; set; }
    }
}
