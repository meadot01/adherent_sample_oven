using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Serilog;
using AdherentShear.DataObjects;

namespace AdherentShear.HardwareInterface
{
    /*
     * Class SampleOvenManager will get the oven status and will store the results.  
     * SampleOvenManager is a singleton.
     */
    public class SampleOvenManager
    {
        //private static Logger logger = NLog.LogManager.GetCurrentClassLogger();
        //private static Logger sampleLogger = NLog.LogManager.GetLogger("SampleLogger");
        private Settings settings;

        public struct SampleData
        {

            public DateTime startDateTime;
            public DateTime? endDateTime;
            public String stationName;

            public SampleData(DateTime _startDateTime, String _stationName)
            {
                startDateTime = _startDateTime;
                endDateTime = null;
                stationName = _stationName;
            }
        }

        private IDictionary<byte, SampleData> sampleDictionary = new Dictionary<byte, SampleData>();

        public IDictionary<byte, SampleData> SampleDictionary
        {
            get
            {
                return sampleDictionary;
            }
        }

        private DateTime startTime;
        private MccDeviceReader deviceReader;

        public TimeSpan ElapsedTime
        {
            get
            {
                return DateTime.Now - startTime;
            }
        }

        public float OvenTemperature
        {
            get;
            private set;
        }

        public String StatusMessage
        {
            get;
            private set;
        }

        private bool runCompleted = false;
        public bool RunCompleted
        {
            get
            {
                return runCompleted;
            }
        }

        private SampleOvenManager()
        {
        }

        public SampleOvenManager(Settings _settings)
        {
            settings = _settings;
            deviceReader = new MccDeviceReader(settings);
            startTime = DateTime.Now;
        }

        public void updateResults()
        {
            MccDeviceResults results = deviceReader.readDevices();
            if (results.ErrorCondition)
            {
                StatusMessage = results.ErrorString;
            }
            else
            {
                StatusMessage = ""; ;
                runCompleted = results.RunCompleted;
                foreach (var sampleValue in results.SampleValues)
                {
                    if (!sampleValue.Value)
                    {
                        if (sampleDictionary.ContainsKey(sampleValue.Key))
                        {
                            SampleData sampleData = sampleDictionary[sampleValue.Key];
                            if (sampleData.endDateTime == null) {
                                sampleData.endDateTime = DateTime.Now;
                                DateTime end = sampleData.endDateTime ?? DateTime.Now;
                                TimeSpan elapsed = end - sampleData.startDateTime;
                                sampleDictionary[sampleValue.Key] = sampleData;
                                String sampleName = "";
                                if (sampleData.stationName.Length != 0)
                                {
                                    sampleName = "(" + sampleData.stationName + ")";
                                }
                                Log.Information("Sample #" + sampleValue.Key + " " + sampleName + " triggered, elapsed time : " + elapsed.ToString(@"hh\:mm"));
                            }
                        }
                    }
                }
            }
        }
    }
}
