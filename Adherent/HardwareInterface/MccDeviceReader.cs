using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MccDaq;
using AdherentSampleOven.DataObjects;

namespace AdherentSampleOven.HardwareInterface
{
    class MccDeviceReader
    {
        private MccBoard tempBoard;
        private Settings settings;
        private TempScale tempScale;

        private ThermocoupleOptions termocoupleOptions = ThermocoupleOptions.Filter;

        public MccDeviceReader(Settings _settings)
        {
            settings = _settings;
            tempBoard = new MccBoard(settings.TempBoardNumber);
            if (settings.TemperatureFormat == TemperatureFormatEnum.Farenheit)
            {
                tempScale = TempScale.Fahrenheit;
            }
            else
            {
                tempScale = TempScale.Celsius;
            }
        }

        public MccDeviceResults readDevices()
        {
            float tempValue;
            MccDaq.ErrorInfo ULStat = tempBoard.TIn(settings.TempPortNumber, tempScale, out tempValue, termocoupleOptions);
            MccDeviceResults results = new MccDeviceResults();
            results.Temperature = tempValue;
            return results;
        }
    }
}
