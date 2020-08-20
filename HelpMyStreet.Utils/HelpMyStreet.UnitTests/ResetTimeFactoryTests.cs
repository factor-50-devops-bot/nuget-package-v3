using HelpMyStreet.Cache;
using NUnit.Framework;
using System;

namespace HelpMyStreet.UnitTests
{
    public class ResetTimeFactoryTests
    {
        [Test]
        public void NextMinute1()
        {
            DateTimeOffset timeNow = new DateTimeOffset(2020, 05, 22, 20, 08, 30, new TimeSpan(0));

            DateTimeOffset expectedResult = new DateTimeOffset(2020, 05, 22, 20, 09, 00, new TimeSpan(0));

            Func<DateTimeOffset, DateTimeOffset> resetTimeDelegate = ResetTimeFactory.GetResetTime(ResetTime.OnMinute);

            DateTimeOffset result = resetTimeDelegate.Invoke(timeNow);

            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public void NextMinute2()
        {
            DateTimeOffset timeNow = new DateTimeOffset(2020, 05, 22, 23, 59, 30, new TimeSpan(0));

            DateTimeOffset expectedResult = new DateTimeOffset(2020, 05, 23, 00, 00, 00, new TimeSpan(0));

            Func<DateTimeOffset, DateTimeOffset> resetTimeDelegate = ResetTimeFactory.GetResetTime(ResetTime.OnMinute);

            DateTimeOffset result = resetTimeDelegate.Invoke(timeNow);

            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public void NextMinute3()
        {
            DateTimeOffset timeNow = new DateTimeOffset(2020, 05, 22, 23, 00, 00, new TimeSpan(0));

            DateTimeOffset expectedResult = new DateTimeOffset(2020, 05, 22, 23, 01, 00, new TimeSpan(0));

            Func<DateTimeOffset, DateTimeOffset> resetTimeDelegate = ResetTimeFactory.GetResetTime(ResetTime.OnMinute);

            DateTimeOffset result = resetTimeDelegate.Invoke(timeNow);

            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public void NextHour1()
        {
            DateTimeOffset timeNow = new DateTimeOffset(2020, 05, 22, 20, 08, 30, new TimeSpan(0));

            DateTimeOffset expectedResult = new DateTimeOffset(2020, 05, 22, 21, 00, 00, new TimeSpan(0));

            Func<DateTimeOffset, DateTimeOffset> resetTimeDelegate = ResetTimeFactory.GetResetTime(ResetTime.OnHour);

            DateTimeOffset result = resetTimeDelegate.Invoke(timeNow);

            Assert.AreEqual(expectedResult, result);
        }


        [Test]
        public void NextHour2()
        {
            DateTimeOffset timeNow = new DateTimeOffset(2020, 05, 22, 20, 08, 30, new TimeSpan(0));

            DateTimeOffset expectedResult = new DateTimeOffset(2020, 05, 22, 21, 00, 00, new TimeSpan(0));

            Func<DateTimeOffset, DateTimeOffset> resetTimeDelegate = ResetTimeFactory.GetResetTime(ResetTime.OnHour);

            DateTimeOffset result = resetTimeDelegate.Invoke(timeNow);

            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public void NextHour3()
        {
            DateTimeOffset timeNow = new DateTimeOffset(2020, 05, 22, 20, 00, 00, new TimeSpan(0));

            DateTimeOffset expectedResult = new DateTimeOffset(2020, 05, 22, 21, 00, 00, new TimeSpan(0));

            Func<DateTimeOffset, DateTimeOffset> resetTimeDelegate = ResetTimeFactory.GetResetTime(ResetTime.OnHour);

            DateTimeOffset result = resetTimeDelegate.Invoke(timeNow);

            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public void NextMidday1()
        {
            DateTimeOffset timeNow = new DateTimeOffset(2020, 05, 22, 20, 08, 30, new TimeSpan(0));

            DateTimeOffset expectedResult = new DateTimeOffset(2020, 05, 23, 12, 00, 00, new TimeSpan(0));

            Func<DateTimeOffset, DateTimeOffset> resetTimeDelegate = ResetTimeFactory.GetResetTime(ResetTime.OnMidday);

            DateTimeOffset result = resetTimeDelegate.Invoke(timeNow);

            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public void NextMidday2()
        {
            DateTimeOffset timeNow = new DateTimeOffset(2020, 05, 22, 11, 08, 30, new TimeSpan(0));

            DateTimeOffset expectedResult = new DateTimeOffset(2020, 05, 22, 12, 00, 00, new TimeSpan(0));

            Func<DateTimeOffset, DateTimeOffset> resetTimeDelegate = ResetTimeFactory.GetResetTime(ResetTime.OnMidday);

            DateTimeOffset result = resetTimeDelegate.Invoke(timeNow);

            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public void NextMidday3()
        {
            DateTimeOffset timeNow = new DateTimeOffset(2020, 05, 22, 12, 00, 00, new TimeSpan(0));

            DateTimeOffset expectedResult = new DateTimeOffset(2020, 05, 23, 12, 00, 00, new TimeSpan(0));

            Func<DateTimeOffset, DateTimeOffset> resetTimeDelegate = ResetTimeFactory.GetResetTime(ResetTime.OnMidday);

            DateTimeOffset result = resetTimeDelegate.Invoke(timeNow);

            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public void NextMidday4_LastDayOfMonth()
        {
            DateTimeOffset timeNow = new DateTimeOffset(2020, 05, 31, 12, 00, 00, new TimeSpan(0));

            DateTimeOffset expectedResult = new DateTimeOffset(2020, 06, 01, 12, 00, 00, new TimeSpan(0));

            Func<DateTimeOffset, DateTimeOffset> resetTimeDelegate = ResetTimeFactory.GetResetTime(ResetTime.OnMidday);

            DateTimeOffset result = resetTimeDelegate.Invoke(timeNow);

            Assert.AreEqual(expectedResult, result);
        }

    }
}
