using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using CheckingAnotherDevice.Models.Autotest;
using CheckingAnotherDevice.Models.Modules;
using CheckingAnotherDevice.Models.Pku;
using CheckingAnotherDevice.Models.Rk;

namespace CheckingAnotherDevice.Helpers
{
    public class XmlHelper
    {
        public static List<RkCommand> LoadRkCommands(string path)
        {
            var settings = new XmlReaderSettings
            {
                IgnoreWhitespace = true,
                IgnoreComments = true
            };
            
            using var reader = XmlReader.Create(path, settings);
            var serializer = new XmlSerializer(typeof(RkCommands));
            var result = (RkCommands)serializer.Deserialize(reader);
            return result?.Rk ?? new List<RkCommand>();
        }
        
        public static void SaveRkCommands()
        {
            var rkCommands = RkCommandsDict.GetInstance().RkCommands;
            var rkCommandsToSave = new RkCommands();

            foreach (var rkCommand in rkCommands.Values)
            {
                rkCommandsToSave.Rk.AddRange(rkCommand);
            }

            var serializer = new XmlSerializer(typeof(RkCommands));
            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add("", "");

            using (var writer = new StreamWriter("settingsRk.xml"))
            {
                serializer.Serialize(writer, rkCommandsToSave, namespaces);
            }
        }

        public static List<RkPkuModule> LoadModulesConnectionSettings(string path)
        {
            var settings = new XmlReaderSettings
            {
                IgnoreWhitespace = true,
                IgnoreComments = true
            };

            var modules = new List<RkPkuModule>();

            using (var reader = XmlReader.Create(path, settings))
            {
                while (reader.Read())
                {
                    if (reader is { NodeType: XmlNodeType.Element, Name: "module" })
                    {
                        var module = new RkPkuModule(
                            id: int.Parse(reader.GetAttribute("id")),
                            moduleName: reader.GetAttribute("name"),
                            connectionString: reader.GetAttribute("connectionstring"),
                            mapVersion: reader.GetAttribute("mapversion")
                        );
                        modules.Add(module);
                    }
                }
            }

            return modules;
        }
        
        public static void SaveModuleConnectionSettings()
        {
            var modules = DeviceModules.GetInstance().AllModules;
            var rkPkuModules = new RkPkuModulesSettings();

            foreach (var module in modules)
            {
                rkPkuModules.ModulesSettings.Add(new ModuleSettings
                {
                    ModuleId = module.ModuleId,
                    ModuleName = module.ModuleName,
                    ConnectionString = module.ConnectionString,
                    MapVersion = module.MapVersion
                });
            }

            var serializer = new XmlSerializer(typeof(RkPkuModulesSettings));
            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add("", "");

            using (var writer = new StreamWriter("settings.xml"))
            {
                serializer.Serialize(writer, rkPkuModules, namespaces);
            }
        }

        public static List<PkuCommand> LoadPkuCommands(string path)
        {
            var settings = new XmlReaderSettings
            {
                IgnoreWhitespace = true,
                IgnoreComments = true
            };

            using var reader = XmlReader.Create(path, settings);
            var serializer = new XmlSerializer(typeof(PkuCommands));
            var result = (PkuCommands)serializer.Deserialize(reader);
            return result?.Pku ?? new List<PkuCommand>();
        }
        
        public static void SavePkuCommands()
        {
            var pkuCommands = PkuCommandsDict.GetInstance().PkuCommands;
            var pkuCommandsToSave = new PkuCommands();

            foreach (var pkuCommand in pkuCommands.Values)
            {
                pkuCommandsToSave.Pku.AddRange(pkuCommand);
            }

            var serializer = new XmlSerializer(typeof(PkuCommands));
            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add("", "");

            using (var writer = new StreamWriter("settingsPku.xml"))
            {
                serializer.Serialize(writer, pkuCommandsToSave, namespaces);
            }
        }

        public static List<TestCase> LoadTestCases(string path)
        {
            var settings = new XmlReaderSettings
            {
                IgnoreWhitespace = true,
                IgnoreComments = true
            };

            using var reader = XmlReader.Create(path, settings);
            var serializer = new XmlSerializer(typeof(TestCases));
            var result = (TestCases)serializer.Deserialize(reader);
            return result?.TestCaseList ?? new List<TestCase>();
        }
    }
}
