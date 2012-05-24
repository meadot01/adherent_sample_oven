using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;


namespace WpfApplication1.DataObjects
{
    public sealed class SampleInformationProvider
    {
        // SampleInformationProvider is a Singleton - it holds device configuration 
        // and status of each sample
        static readonly SampleInformationProvider _instance = new SampleInformationProvider();
        
        private ObservableCollection<SampleInformation> deviceConfigurationList = new ObservableCollection<SampleInformation>();

        public static SampleInformationProvider Instance
        {
            get { return _instance; }
        }

        private SampleInformationProvider()
        {
        }
        public ObservableCollection<SampleInformation> SampleInformationList
        {
            get {
                if (deviceConfigurationList.Count == 0)
                {
                    for (byte i = 1; i <= 30; i++)
                    {
                        String portName = null;
                        try
                        {
                            portName = Properties.Settings.Default["Sample" + i.ToString()].ToString();
                        }
                        catch (System.Configuration.SettingsPropertyNotFoundException)
                        { }
                        MccPortInformation portInfo = MccPortInformationAccessor.Instance.portForName(portName);
                        deviceConfigurationList.Add(new SampleInformation(i, portInfo));
                    }
                   
                   // Properties.Settings.
                }
                return deviceConfigurationList;
            }
        }

       //     deviceConfigurationList.Add(new SampleInformation(1, null));
       
    }
 }
