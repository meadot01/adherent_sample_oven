using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MccDaq;
using AdherentSampleOven.DataObjects;
using NLog;

namespace AdherentSampleOven.HardwareInterface
{
    class MccDeviceReader
    {
        private static Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private MccBoard tempBoard, dioBoard;
        private Settings settings;
        private TempScale tempScale;



        private ThermocoupleOptions termocoupleOptions = ThermocoupleOptions.Filter;

        public MccDeviceReader(Settings _settings)
        {
            settings = _settings;
            ISet<MccDaq.DigitalPortType> portsUsed = settings.getPortsUsed();
            tempBoard = new MccBoard(settings.TempBoardNumber);
            if (settings.TemperatureFormat == TemperatureFormatEnum.Farenheit)
            {
                tempScale = TempScale.Fahrenheit;
            }
            else
            {
                tempScale = TempScale.Celsius;
            }
            dioBoard = new MccBoard(settings.DIOBoardNumber);
            foreach (MccDaq.DigitalPortType portType in settings.getPortsUsed())
            {
                dioBoard.DConfigPort(portType, MccDaq.DigitalPortDirection.DigitalIn);
            }

            //    MccDaq.ErrorInfo ULStat = MccDaq.MccService.ErrHandling(MccDaq.ErrorReporting.PrintAll, MccDaq.ErrorHandling.StopAll);

        }

        public MccDeviceResults readDevices()
        {
            MccDeviceResults results = new MccDeviceResults();
            results.ErrorCondition = false;
            float tempuratureValue;
            MccDaq.ErrorInfo ulStat = tempBoard.TIn(settings.TempPortNumber, tempScale, out tempuratureValue, termocoupleOptions);
            if (ulStat.Value == ErrorInfo.ErrorCode.NoErrors)
            {
                results.Temperature = tempuratureValue;
            }
            else
            {
                results.ErrorCondition = true;
                results.ErrorString = "Error while reading Temperature : " + ulStat.Message;
                return results;
            }
            IDictionary<MccDaq.DigitalPortType, ushort> dioPortValues = new Dictionary<MccDaq.DigitalPortType, ushort>();
            foreach (MccDaq.DigitalPortType portType in settings.getPortsUsed())
            {
                ushort tmpPortValues;
                ulStat = dioBoard.DIn(portType, out tmpPortValues);
                if (ulStat.Value == ErrorInfo.ErrorCode.NoErrors)
                {
                    dioPortValues[portType] = tmpPortValues;
                }
                else
                {
                    results.ErrorCondition = true;
                    results.ErrorString = "Error while reading dio port " + portType.ToString() + " : " + ulStat.Message;
                    return results;
                }

            }
            bool tempRunCompleted = true;
            foreach (var sampleInfo in settings.SampleConfigurationDictionary)
            {
                MccPortInformation portInfo = sampleInfo.Value;
                if (portInfo != null)
                {
                    bool currentValue = ((dioPortValues[portInfo.PortType] & (1 << portInfo.BitNumber)) != 0);
                    if (settings.SwitchDefaultClosed)
                    {
                        results.SampleValues[sampleInfo.Key] = !currentValue;
                    }
                    else
                    {
                        results.SampleValues[sampleInfo.Key] = currentValue;
                    }
                    if (results.SampleValues[sampleInfo.Key])
                    {
                        tempRunCompleted = false;
                    }
                }
            }
            results.RunCompleted = tempRunCompleted;

            return results;

        }
    }
}
