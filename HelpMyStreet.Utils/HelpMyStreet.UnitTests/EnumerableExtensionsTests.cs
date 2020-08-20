using HelpMyStreet.Utils.Dtos;
using HelpMyStreet.Utils.Extensions;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace HelpMyStreet.UnitTests
{
    public class EnumerableExtensionsTests
    {
        [Test]
        public void WhereWithinBoundary()
        {
            List<ILatitudeLongitude> latLngs = new List<ILatitudeLongitude>()
            {
                Mock.Of<ILatitudeLongitude>(x => x.Latitude == 0d && x.Longitude == 0d),
                Mock.Of<ILatitudeLongitude>(x => x.Latitude == 5d && x.Longitude == 5d),
                Mock.Of<ILatitudeLongitude>(x => x.Latitude == 10d && x.Longitude == 10d),

                Mock.Of<ILatitudeLongitude>(x => x.Latitude == 10d && x.Longitude == 10.1d),
                Mock.Of<ILatitudeLongitude>(x => x.Latitude == 10.1d && x.Longitude == 10d),
                Mock.Of<ILatitudeLongitude>(x => x.Latitude == 10.1d && x.Longitude == 10.1d),

                Mock.Of<ILatitudeLongitude>(x => x.Latitude == -0.1d && x.Longitude == 0d),
                Mock.Of<ILatitudeLongitude>(x => x.Latitude == 0d && x.Longitude == -0.1d),
                Mock.Of<ILatitudeLongitude>(x => x.Latitude == -0.1d && x.Longitude == -0.1d),
            };

            IEnumerable<ILatitudeLongitude> result = latLngs.WhereWithinBoundary(0, 0, 10, 10);

            Assert.AreEqual(3, result.Count());
            Assert.IsTrue(result.Any(x => x.Latitude == 0d && x.Longitude == 0d));
            Assert.IsTrue(result.Any(x => x.Latitude == 5d && x.Longitude == 5d));
            Assert.IsTrue(result.Any(x => x.Latitude == 10d && x.Longitude == 10d));
        }


        [Test]
        public void SplitList()
        {
            var strings = new List<string>
        {
            "true",
            "true",
            "true",
            "false",
            "false",
            "false",
            "false"
        };

            var (truelist, falselist) = strings.Split(x => x == "true");
            Assert.AreEqual(3, truelist.Count());
            Assert.AreEqual(4, falselist.Count());
            Assert.IsTrue(truelist.All(x => x == "true"));
            Assert.IsTrue(falselist.All(x => x == "false"));
        }
    }
}
