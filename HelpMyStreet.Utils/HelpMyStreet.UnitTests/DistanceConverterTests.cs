using HelpMyStreet.Utils.Utils;
using NUnit.Framework;
using System;

namespace HelpMyStreet.UnitTests
{
    public class DistanceConverterTests
    {

        [Test]
        public void ConvertMetresToMiles()
        {
            double result = DistanceConverter.MetresToMiles(1000);

            Assert.AreEqual(0.621371, Math.Round(result, 6));
        }

        [Test]
        public void ConvertMilesToMetres()
        {
            int result = DistanceConverter.MilesToMetres(1);

            Assert.AreEqual(1609, result);
        }

        [Test]
        public void ConvertMetresToMiles_MetresIsDouble()
        {
            double result = DistanceConverter.MetresToMiles(1000d);

            Assert.AreEqual(0.621371, Math.Round(result, 6));
        }
    }
}
