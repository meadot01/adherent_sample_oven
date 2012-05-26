using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace WpfApplication1
{
    [Serializable]
    public class SampleInformation : INotifyPropertyChanged

    {

        public event PropertyChangedEventHandler PropertyChanged;
        private DataObjects.MccPortInformation portInformation;

        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        public SampleInformation(byte _sampleNumber, DataObjects.MccPortInformation _portInformation)
        {
            SampleNumber = _sampleNumber;
            PortInformation = _portInformation;
            
        }

        public byte SampleNumber
        {
            get;
            set;
        }

        public DataObjects.MccPortInformation PortInformation
        {
            get
            {
                return portInformation;
            }
            set
            {
                if (value != this.portInformation)
                {
                    this.portInformation = value;
                    NotifyPropertyChanged("PortInformation");
                }
            }
        }

        public String PortName
        {
            get
            {
                if (PortInformation != null)
                {
                    return PortInformation.Name;
                }
                else
                {
                    return "";
                }
            }

            set
            {
                PortInformation = DataObjects.MccPortInformationAccessor.Instance.portForName(value);

            }
        }

        public override bool Equals(object obj)
        {
            if (obj is SampleInformation)
            {
            SampleInformation fooItem = obj as SampleInformation;

            return fooItem.SampleNumber == this.SampleNumber;

            }
            return false;
        }

        public override int GetHashCode()
        {
            // Which is preferred?

            return SampleNumber;

            //return this.FooId.GetHashCode();
        }

    }


}
