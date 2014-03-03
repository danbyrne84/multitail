using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MultiTail
{
    [XmlRoot("LogLocations"), XmlType("LogLocations")]
    public class LogLocations
    {
        protected List<string> _log = new List<string>();

        public List<string> Log
        {
            get
            {
                return _log;
            }
            set
            {
                _log = value;
            }
        }
    }
}
