using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;


namespace AdherentSampleOven.DataObjects
{
    /*
     * SettingsManager
     *   Singleton that is used to read or write Application settings.  
     */
    public sealed class SettingsManager
    {
        private static string samplePropertyPrefix = "Sample";
        private static string tempBoardNumberPropertyName = "tempBoard";
        private static string tempPortNumberPropertyName = "tempPort";
        private static string dioBoardNumberPropertyName = "dioBoard";
        private static string tempFormatCelsiusPropertyName = "temperatureFormatCelsius";
        private static string switchDefaultClosedPropertyName = "switchDefaultClosed";
        private static string secondsBeforeErrorTimeoutPropertyName = "secondsBeforeErrorTimeout";


        static readonly SettingsManager _instance = new SettingsManager();

        public static SettingsManager Instance
        {
            get { return _instance; }
        }

        private SettingsManager()
        {
        }

        public Settings ApplicationSettings
        {
            get
            {
                Settings applicationSettings = new Settings();
                IDictionary<byte, MccPortInformation> sampleConfigDict = new Dictionary<byte, MccPortInformation>();
                for (byte i = 1; i <= 30; i++)
                {
                    String portName = null;
                    try
                    {
                        portName = Properties.Settings.Default[samplePropertyPrefix + i.ToString()] as string;
                    }
                    catch (System.Configuration.SettingsPropertyNotFoundException)
                    { }
                    MccPortInformation portInfo = MccPortInformationAccessor.Instance.portForName(portName);
                    sampleConfigDict.Add(i, portInfo);
                }
                applicationSettings.SampleConfigurationDictionary = sampleConfigDict;
                Object dioBoardNumber = Properties.Settings.Default[dioBoardNumberPropertyName];
                if (dioBoardNumber == null)
                {
                    applicationSettings.DIOBoardNumber = 0;
                }
                else
                {
                    applicationSettings.DIOBoardNumber = (int)dioBoardNumber;
                }
                Object switchDefaultClosed = Properties.Settings.Default[switchDefaultClosedPropertyName];
                if (switchDefaultClosed == null)
                {
                    applicationSettings.SwitchDefaultClosed = false;
                }
                else
                {
                    applicationSettings.SwitchDefaultClosed = (bool)switchDefaultClosed;
                }

                Object secondsBeforeErrorTimeout = Properties.Settings.Default[secondsBeforeErrorTimeoutPropertyName];
                if (secondsBeforeErrorTimeout == null)
                {
                    applicationSettings.SecondsBeforeErrorTimeout = 0;
                }
                else
                {
                    applicationSettings.SecondsBeforeErrorTimeout = (int)secondsBeforeErrorTimeout;
                }

                return applicationSettings;
            }

        }


        public void updateProperties(Settings settings)
        {
            if (settings != null)
            {
                settings.clearPortsUsed();
                if (settings.SampleConfigurationDictionary != null)
                {
                    foreach (var pair in settings.SampleConfigurationDictionary)
                    {
                        string portName = null;
                        if (pair.Value != null)
                        {
                            portName = pair.Value.Name;
                        }
                        Properties.Settings.Default[samplePropertyPrefix + pair.Key] = portName;
                    }
                }
                Properties.Settings.Default[dioBoardNumberPropertyName] = settings.DIOBoardNumber;
                if (settings.SwitchDefaultClosed == true)
                {
                    Properties.Settings.Default[switchDefaultClosedPropertyName] = true;
                } else
                {
                    Properties.Settings.Default[switchDefaultClosedPropertyName] = false;
                }
                Properties.Settings.Default[secondsBeforeErrorTimeoutPropertyName] = settings.SecondsBeforeErrorTimeout;
            }

        }
    }
}

