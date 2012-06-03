using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdherentSampleOven.DataObjects
{
    public enum TemperatureFormatEnum { Celsius, Farenheit };

    public class Settings
    {
        public IDictionary<byte, MccPortInformation> SampleConfigurationDictionary
        {
            get;
            set;
        }

        public int TempBoardNumber
        {
            get;
            set;
        }
        
        public byte TempPortNumber
        {
            get;
            set;
        }

        public int DIOBoardNumber
        {
            get;
            set;
        }

        public TemperatureFormatEnum TemperatureFormat
        {
            get;
            set;
        }


    }
}
