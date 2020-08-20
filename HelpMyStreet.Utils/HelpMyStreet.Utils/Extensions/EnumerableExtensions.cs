using HelpMyStreet.Utils.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HelpMyStreet.Utils.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> WhereWithinBoundary<T>(this IEnumerable<T> source, double swLatitude, double swLongitude, double neLatitude, double neLongitude) where T : ILatitudeLongitude
        {
            IEnumerable<T> result = source.Where(pt =>
                pt.Latitude >= swLatitude &&
                pt.Latitude <= neLatitude &&
                pt.Longitude >= swLongitude &&
                pt.Longitude <= neLongitude);

            return result;
        }

        public static (IEnumerable<T>, IEnumerable<T>) Split<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            var trueList = source.Where(predicate).ToList();
            var falseList = source.Where(x => !predicate(x)).ToList();

            return (trueList, falseList);
        }
    }
}
