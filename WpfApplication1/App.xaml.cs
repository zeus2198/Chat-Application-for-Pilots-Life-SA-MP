using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        // Global Settings :
        public short fetchsec { get; set; }
        public bool chatlogging { get; set; }        
        public List<nickalert> nicks{ get; set; }

        public void SaveSettings()
        {
            new XDocument(
                new XElement("settings",
                new XElement("nickalert",
                nicks.Select(x =>
                    new XElement("nick", x.name,
                            new XAttribute("activated", x.activated ? "1" : "0"),
                            new XAttribute("color", x.color)
                    )) 
                ),
                new XElement("ChatLogging", (chatlogging ? "1" : "0")),
                new XElement("PlayerFetchTime", fetchsec)
                )).Save("settings.xml");
        }

        public void LoadSettings()
        {
            XDocument doc = XDocument.Load("settings.xml");
            fetchsec = short.Parse( doc.Element("settings").Element("PlayerFetchTime").Value );
            chatlogging = doc.Element("settings").Element("ChatLogging").Value == "1" ? true : false;
            nicks = (from c in doc.Descendants("nick")
                     select new nickalert()
                     {
                         name = c.Value,
                         activated = c.Attribute("activated").Value == "1" ? true : false,
                         color = c.Attribute("color").Value
                     }).ToList<nickalert>();
        }
        
    }
    public class nickalert
    {
        public string name { get; set; }
        public bool activated { get; set; }
        public string color { get; set; }
    }

    
    
}
