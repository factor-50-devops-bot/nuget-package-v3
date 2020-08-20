using NUnit.Framework;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using HelpMyStreet.Utils.Extensions;

namespace HelpMyStreet.UnitTests
{
    public class ObjectExtensionsTests
    {

        private class TestClass
        {
            [System.ComponentModel.DataAnnotations.Range(0, 10)]
            public int Property { get; set; }
        }

        [Test]
        public void ClassIsInValidState()
        {
            TestClass testClass = new TestClass()
            {
                Property = 10
            };

            bool result = testClass.IsValid(out ICollection<ValidationResult> validationResults);

            Assert.IsTrue(result);
            Assert.AreEqual(0, validationResults.Count);
        }
        
        [Test]
        public void ClassIsNotInValidState()
        {
            TestClass testClass = new TestClass()
            {
                Property = 11
            };

            bool result = testClass.IsValid(out ICollection<ValidationResult> validationResults);

            Assert.IsFalse(result);
            Assert.AreEqual(1, validationResults.Count);
            Assert.IsTrue(validationResults.Any(x=>x.ErrorMessage.Contains("The field Property")));
            Assert.IsTrue(validationResults.Any(x=>x.MemberNames.Any(x=> x == "Property")));
        }

    }
}
