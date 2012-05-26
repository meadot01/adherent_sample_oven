﻿using System;
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
        
        private IDictionary<byte, MccPortInformation> sampleConfigurationDictionary = new Dictionary<byte,MccPortInformation>();

        public static SampleInformationProvider Instance
        {
            get { return _instance; }
        }

        private SampleInformationProvider()
        {
        }
        public IDictionary<byte, MccPortInformation> SampleConfigurationDictionary
        {
            get {
                if (sampleConfigurationDictionary.Count == 0)
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
                        sampleConfigurationDictionary.Add(i,portInfo);
                    }
                   
                   // Properties.Settings.
                }
                return sampleConfigurationDictionary;
            }
        }

       //     deviceConfigurationList.Add(new SampleInformation(1, null));
       
    }
 }
