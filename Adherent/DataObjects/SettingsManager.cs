using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;


namespace AdherentSampleOven.DataObjects
{
    public sealed class SettingsManager
    {
        private static string samplePropertyPrefix = "Sample";
        private static string tempBoardNumberPropertyName = "tempBoard";
        private static string tempPortNumberPropertyName = "tempPort";
        private static string dioBoardNumberPropertyName = "dioBoard";

        // SampleInformationProvider is a Singleton - it holds device configuration 
        // and status of each sample
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
            get {
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
                        sampleConfigDict.Add(i,portInfo);
                    }
                applicationSettings.SampleConfigurationDictionary = sampleConfigDict;
                Object tempBoardNumber = Properties.Settings.Default[tempBoardNumberPropertyName];
                if (tempBoardNumber == null)
                {
                    applicationSettings.TempBoardNumber = 0;
                }
                else
                {
                    applicationSettings.TempBoardNumber = (int)tempBoardNumber;
                }
                Object tempPortNumber = Properties.Settings.Default[tempPortNumberPropertyName];
                if (tempPortNumber == null)
                {
                    applicationSettings.TempPortNumber = 0;
                }
                else
                {
                    applicationSettings.TempPortNumber = (byte)tempPortNumber;
                }
                Object dioBoardNumber = Properties.Settings.Default[dioBoardNumberPropertyName];
                if (dioBoardNumber == null)
                {
                    applicationSettings.DIOBoardNumber = 0;
                }
                else
                {
                    applicationSettings.DIOBoardNumber = (int)dioBoardNumber;
                }
                return applicationSettings;
                }
                
            }
        }

        //public void updateSampleSettingProperties(IDictionary<byte, string> sampleConfigurationDictionary)
        //{
        //    foreach (var pair in sampleConfigurationDictionary)
        //    {
        //        Properties.Settings.Default[samplePropertyPrefix + pair.Key] = pair.Value;
        //    }
        //    SampleConfigurationDictionary.Clear();
        //}
}

