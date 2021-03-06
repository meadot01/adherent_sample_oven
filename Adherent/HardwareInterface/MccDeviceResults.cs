﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdherentSampleOven.HardwareInterface
{
    /*
     * MccDeviceResults
     *   Data object containing the results of the hardware scan.  This is returned from the 
     *   MccDeviceReader.
     */
    class MccDeviceResults
    {
        public float Temperature
        {
            get;
            set;
        }

        private IDictionary<byte,bool>sampleValues = new Dictionary<byte, bool>();

        public IDictionary<byte, bool> SampleValues
        {
            get
            {
                return sampleValues;
            }
        }

        public String ErrorString
        {
            get;
            set;
        }

        public bool ErrorCondition
        {
            get;
            set;
        }

        public bool RunCompleted
        {
            get;
            set;
        }

    }
}
