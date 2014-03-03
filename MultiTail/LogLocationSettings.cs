using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Xml.Serialization;
using System.IO;
using System.Xml;

namespace MultiTail
{
    public class LogLocationSettings : IConfigurationSectionHandler
    {  
        public object Create(object parent, object configContext, System.Xml.XmlNode section)
        {
            var szConfig = section.SelectNodes("//logLocations/log");
            LogLocations retConf = new LogLocations();

            if (szConfig != null)
            {
                foreach (XmlNode node in szConfig)
                {
                    retConf.Log.Add(node.InnerText);
                }
            }

            return retConf;
        }
    }
}
