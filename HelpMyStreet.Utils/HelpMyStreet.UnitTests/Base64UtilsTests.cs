using HelpMyStreet.Utils.Utils;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace HelpMyStreet.UnitTests
{
    class Base64UtilsTests
    {
        [Test]
        public void Base64Encode_Base64Decode_EmptyString()
        {
            string result = Base64Utils.Base64Decode(Base64Utils.Base64Encode(""));

            Assert.AreEqual("", result);
        }

        [Test]
        public void Base64Encode_Base64Decode_HelloWorld()
        {
            string result = Base64Utils.Base64Decode(Base64Utils.Base64Encode("Hello World!"));

            Assert.AreEqual("Hello World!", result);
        }
    }
}
