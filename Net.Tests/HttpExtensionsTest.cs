using System;
using System.Collections;
using System.Collections.Generic;
using NightlyCode.Net;
using NUnit.Framework;

namespace Net.Tests
{

    [TestFixture]
    public class HttpExtensionsTest
    {

        IEnumerable<string> Prefixes
        {
            get
            {
                yield return "http://localhost/updater/";
                yield return "http://1.2.3.4/updates/";
                yield return "http://updates.cardlog.in/";
            }
        }

        [Test]
        public void TestRelativePathExtraction(
            [Values("http://updates.cardlog.in/find/customer", 
                    "http://localhost/updater/find/customer", 
                    "http://1.2.3.4/updates/find/customer",
                    "http://updates.cardlog.in/find/customer?key=value&otherkey=othervalue")]
            string path) {
            string relativepath = HttpExtensions.GetRelativePath(path, Prefixes);
            Assert.AreEqual("/find/customer", relativepath);
        }

        [Test]
        public void TestExtractionWithGenericBindings([Values("http://cardlog.in/updater/find/customer",
            "http://localhost/updater/find/customer",
            "http://1.2.3.4/updater/find/customer",
            "http://cardlog.in/updater/find/customer?key=value&otherkey=othervalue")] string path) {
            string relativepath = HttpExtensions.GetRelativePath(path, new[] {"http://*/updater/"});
            Assert.AreEqual("/find/customer", relativepath);
        }

        [Test]
        public void TestGenericBindingWithPort() {
            string relativepath = HttpExtensions.GetRelativePath("http://localhost:49221/updater/find/customer", new[] { "http://*:49221/updater/" });
            Assert.AreEqual("/find/customer", relativepath);
        }

        [Test]
        public void TestRelativePathWithPort() {
            string relativepath = HttpExtensions.GetRelativePath("http://localhost:49221/updater/find/customer", new [] {"http://localhost:49221/updater/"});
            Assert.AreEqual("/find/customer", relativepath);
        }

        [Test]
        public void TestRelativeUri()
        {
            Uri relativeuri = HttpExtensions.GetRelativeUri(new Uri("http://localhost:49221/updater/find/customer?id=4434"), new[] { "http://localhost:49221/updater/" });
            Assert.AreEqual("/find/customer?id=4434", relativeuri.ToString());
        }

        [Test]
        public void ReadableAddressConversionTest([Values("www.gmx.de", "suche.gmx.de/web", "facebook.com/signin", "google.com/accounts/login")]string data) {
            string address=HttpExtensions.GetReadableAddress(data);
        }
    }
}
