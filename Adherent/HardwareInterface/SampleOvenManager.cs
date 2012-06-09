using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AdherentSampleOven.DataObjects;

namespace AdherentSampleOven.HardwareInterface
{
    /*
     * Class SampleOvenManager will get the oven status and will store the results.  
     * SampleOvenManager is a singleton.
     */
    public class SampleOvenManager
    {
        public struct SampleData {
            public float finalTemp;
            public TimeSpan finalTime;

            public SampleData(float _finalTemp, TimeSpan _finalTime)
            {
                finalTemp = _finalTemp;
                finalTime = _finalTime;
            }
        }

        private IDictionary<byte, SampleData> sampleDictionary = new Dictionary<byte, SampleData>();

        private DateTime startTime;
        private MccDeviceReader deviceReader;

        public TimeSpan ElapsedTime
        {
            get
            {
                return startTime - DateTime.Now;
            }
        }

        public float OvenTemperature
        {
            get;
            private set;
        }


        private SampleOvenManager()
        {
        }

        public SampleOvenManager(Settings settings)
        {
            deviceReader = new MccDeviceReader(settings);
            startTime = DateTime.Now;
        }

        public void updateResults()
        {
            MccDeviceResults results = deviceReader.readDevices();
            OvenTemperature = results.Temperature;
        }

    }

}
