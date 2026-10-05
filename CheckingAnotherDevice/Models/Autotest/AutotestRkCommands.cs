using System.Xml.Serialization;
using CheckingAnotherDevice.Models.Rk;

namespace CheckingAnotherDevice.Models.Autotest;

public class AutotestRkCommands : RkCommand
{
    [XmlAttribute("duration")] 
    public ushort Duration { get; set; }
}