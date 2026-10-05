using System.Xml.Serialization;

namespace CheckingAnotherDevice.Models.Autotest;

public class AutotestPkuCommand
{
    [XmlAttribute("name")]
    public string Name { get; set; }
}