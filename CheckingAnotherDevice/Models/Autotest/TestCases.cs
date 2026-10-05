using System.Collections.Generic;
using System.Xml.Serialization;

namespace CheckingAnotherDevice.Models.Autotest;

[XmlRoot("TestCases")]
public class TestCases
{
    [XmlElement("TestCase")]
    public List<TestCase> TestCaseList { get; set; } = new();
}

public class TestCase
{
    [XmlAttribute("delay")]
    public int Delay { get; set; }

    [XmlArray("RkCommands")]
    [XmlArrayItem("command")]
    public List<AutotestRkCommands> RkCommands { get; set; } = new();

    [XmlArray("PkuCommands")]
    [XmlArrayItem("pku")]
    public List<AutotestPkuCommand> PkuCommands { get; set; } = new();
}
