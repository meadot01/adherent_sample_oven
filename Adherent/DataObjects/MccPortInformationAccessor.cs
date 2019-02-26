using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdherentShear.DataObjects
{
    /*
     * MccPortInformationAccessor is a Singleton - it holds information about the
     * Mcc ports
     */
    
    [Serializable]
    public sealed class MccPortInformationAccessor
    {
        static readonly MccPortInformationAccessor _instance = new MccPortInformationAccessor();
        private Dictionary<String, MccPortInformation> mccPortInformationDictionary;

        public static MccPortInformationAccessor Instance
        {
            get { return _instance; }
        }

        private MccPortInformationAccessor()
        {
        }

        /*
         * MccPortInfomationDictionary is a property that contains a dictionary with the
         * keys being the readable port name and the the value containing the MccPortInformation
         * for that port.
         */
        private IDictionary<String, MccPortInformation> MccPortInformationDictionary
        {
            get
            {
                if (mccPortInformationDictionary == null)
                {
                    mccPortInformationDictionary = new Dictionary<String, MccPortInformation>();
                    addPortInformationToDictionary(mccPortInformationDictionary, MccDaq.DigitalPortType.FirstPortA,
                        "Port1A", 8, 0);
                    addPortInformationToDictionary(mccPortInformationDictionary, MccDaq.DigitalPortType.FirstPortB,
                        "Port1B", 8, 0);
                    addPortInformationToDictionary(mccPortInformationDictionary, MccDaq.DigitalPortType.FirstPortCL,
                        "Port1C", 4, 0);
                    addPortInformationToDictionary(mccPortInformationDictionary, MccDaq.DigitalPortType.FirstPortCH,
                        "Port1C", 4, 4);
                    addPortInformationToDictionary(mccPortInformationDictionary, MccDaq.DigitalPortType.SecondPortA,
                        "Port2A", 8, 0);
                    addPortInformationToDictionary(mccPortInformationDictionary, MccDaq.DigitalPortType.SecondPortB,
                        "Port2B", 8, 0);
                    addPortInformationToDictionary(mccPortInformationDictionary, MccDaq.DigitalPortType.SecondPortCL,
                        "Port2C", 4, 0);
                    addPortInformationToDictionary(mccPortInformationDictionary, MccDaq.DigitalPortType.SecondPortCH,
                        "Port2C", 4, 4);
                    addPortInformationToDictionary(mccPortInformationDictionary, MccDaq.DigitalPortType.ThirdPortA,
                        "Port3A", 8, 0);
                    addPortInformationToDictionary(mccPortInformationDictionary, MccDaq.DigitalPortType.ThirdPortB,
                        "Port3B", 8, 0);
                    addPortInformationToDictionary(mccPortInformationDictionary, MccDaq.DigitalPortType.ThirdPortCL,
                        "Port3C", 4, 0);
                    addPortInformationToDictionary(mccPortInformationDictionary, MccDaq.DigitalPortType.ThirdPortCH,
                        "Port3C", 4, 4);
                    addPortInformationToDictionary(mccPortInformationDictionary, MccDaq.DigitalPortType.FourthPortA,
                        "Port4A", 8, 0);
                    addPortInformationToDictionary(mccPortInformationDictionary, MccDaq.DigitalPortType.FourthPortB,
                        "Port4B", 8, 0);

                    addPortInformationToDictionary(mccPortInformationDictionary, MccDaq.DigitalPortType.FourthPortCL,
                        "Port4C", 4, 0);
                    addPortInformationToDictionary(mccPortInformationDictionary, MccDaq.DigitalPortType.FourthPortCH,
                        "Port4C", 4, 4);
                }
                return (IDictionary<String, MccPortInformation>)      mccPortInformationDictionary;
            }
        }

        /*
         * MccPortInformationList is a property that contains all the MccPortInformation objects
         */
        public ICollection<MccPortInformation> MccPortInformationList
        {
            get
            {
                return (ICollection<MccPortInformation>)  MccPortInformationDictionary.Values;
            }
        }

        /*
         * MccPortNameList is a property that contains all the readable port names
         */
        public ICollection<String> MccPortNameList
        {
            get
            {
                return (ICollection<String>)MccPortInformationDictionary.Keys;
            }
        }

        
        private void addPortInformationToDictionary( IDictionary<String, MccPortInformation> _portInfoDict, 
            MccDaq.DigitalPortType _digitalPortType, String _portNamePrefix, 
            byte _numberOfBits, byte _bitOffset) 
        {
            for (byte i = 0; i < _numberOfBits; i++)
            {
                MccPortInformation mccPortInformation = new MccPortInformation(_portNamePrefix + (i + _bitOffset).ToString(),
                    _digitalPortType,i);
                _portInfoDict.Add(mccPortInformation.Name, mccPortInformation);
            }
        }

        /*
         * portForName returns MccPortInformation for the given port name.
         * If not found it will return null.
         */
        public MccPortInformation portForName(String _portName)
        {

            MccPortInformation value = null;
            if (!String.IsNullOrEmpty(_portName))
            {
                MccPortInformationDictionary.TryGetValue(_portName, out value);
            }
            return value;
        }

    }
}
