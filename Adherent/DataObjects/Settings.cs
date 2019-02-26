using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdherentShear.DataObjects
{
    public enum TemperatureFormatEnum { Celsius, Farenheit };

    /*
     * Settings
     *   DataObject to hold Application Settings
     */
    public class Settings
    {
        private ISet<MccDaq.DigitalPortType> portsUsed;
        public IDictionary<byte, MccPortInformation> SampleConfigurationDictionary
        {
            get;
            set;
        }

        public int DIOBoardNumber
        {
            get;
            set;
        }

        public bool SwitchDefaultClosed
        {
            get;
            set;
        }

        public int SecondsBeforeErrorTimeout
        {
            get;
            set;
        }

        public ISet<MccDaq.DigitalPortType> getPortsUsed()
        {
            if (portsUsed == null)
            {
                portsUsed = new SortedSet<MccDaq.DigitalPortType>();
                foreach (var pair in SampleConfigurationDictionary)
                {
                    if (pair.Value != null)
                    {
                        portsUsed.Add(pair.Value.PortType);
                    }
                }
            }
            return portsUsed;
        }

        public void clearPortsUsed()
        {
            portsUsed = null;
        }



    }
}
