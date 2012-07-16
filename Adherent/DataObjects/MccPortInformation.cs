using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdherentSampleOven.DataObjects
{
    /*
     * MccPortInformation holds device information for a specific port on the DIO device 
     */
    [Serializable]
    public class MccPortInformation
    {


        public MccPortInformation(string name, MccDaq.DigitalPortType portType, byte bitNumber)
        {
            Name = name;
            PortType = portType;
            BitNumber = bitNumber;
        }

        public string Name { get; set; }
        public MccDaq.DigitalPortType PortType { get; set; }
        public byte BitNumber { get; set; }
    }
}
