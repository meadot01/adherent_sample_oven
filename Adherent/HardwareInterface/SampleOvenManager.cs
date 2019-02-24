using System;
using System.Collections.Generic;
using AdherentSampleOven.DataObjects;
using Serilog;

namespace AdherentSampleOven.HardwareInterface
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

            public float finalTemp;
            public TimeSpan finalTime;

            public SampleData(float _finalTemp, TimeSpan _finalTime)
            {
                finalTemp = _finalTemp;
                finalTime = _finalTime;
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
                OvenTemperature = results.Temperature;
                runCompleted = results.RunCompleted;
                foreach (var sampleValue in results.SampleValues)
                {
                    if (!sampleValue.Value)
                    {
                        if (!sampleDictionary.ContainsKey(sampleValue.Key))
                        {
                            sampleDictionary[sampleValue.Key] = new SampleData(results.Temperature, ElapsedTime);
                            String temperatureString;
                            if (settings.TemperatureFormat == TemperatureFormatEnum.Farenheit)
                            {
                                temperatureString = System.Math.Round(results.Temperature) + "°F";
                            }
                            else
                            {
                                temperatureString = System.Math.Round(results.Temperature) + "°C";
                            }
                            Log.Information("Sample #" + sampleValue.Key + " triggered with temperature = " + temperatureString + ", elapsed time : " + ElapsedTime.ToString(@"hh\:mm"));
                            
                        }
                    }
                }
            }
        }
    }
}
