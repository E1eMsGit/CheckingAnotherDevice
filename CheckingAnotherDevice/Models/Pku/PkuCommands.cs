using Rss.TmFramework.Modules.RkPku;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace CheckingAnotherDevice.Models.Pku;

[XmlRoot("settingspku")]
public class PkuCommands
{
    [XmlElement("pku")]
    public List<PkuCommand> Pku { get; set; } = new();
}

public class PkuCommand
{
    [XmlAttribute("name")]
    public string Name { get; set; }

    [XmlAttribute("modulename")]
    public string ModuleName { get; set; }

    [XmlAttribute("number")]
    public uint Number { get; set; }

    [XmlAttribute("mode")]
    public EPkuMode Mode { get; set; }

    [XmlAttribute("moduleid")]
    public int ModuleId { get; set; }
}
