﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace AdherentTests
{
    [TestFixture]
    public class TestFixture1
    {
        [Test]
        public void TestGetPortByName()
        {
            AdherentSampleOven.DataObjects.MccPortInformation portInfo =
                AdherentSampleOven.DataObjects.MccPortInformationAccessor.Instance.portForName("Port3A4");

            Assert.NotNull(portInfo);
            Assert.AreEqual("Port3A4", portInfo.Name);
        }

    }
}
