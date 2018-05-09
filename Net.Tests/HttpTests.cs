using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using NightlyCode.Net.Http;
using NightlyCode.Net.Http.Requests;
using NUnit.Framework;

namespace Net.Tests {

    [TestFixture]
    public class HttpTests {
        static readonly HttpServer server = new HttpServer(IPAddress.Any, 50000);

        [OneTimeSetUp]
        public void Setup() {
            server.Start();
        }

        [OneTimeTearDown]
        public void Teardown() {
            server.Stop();
        }

        void SendRequest(params string[] urls) {
            using(WebClient webclient = new WebClient()) {
                foreach(string url in urls)
                    webclient.DownloadString(url);
            }
        }

        void PostRequest(string url, string data) {
            using (WebClient webclient = new WebClient()) {
                webclient.UploadString(url, data);
            }
        }

        [Test]
        public void TestSimpleGetRequest() {
            HttpRequest incoming = null;
            
            server.Request += (client, request) => {
                if(request.Resource != "/simpleget")
                    return;

                incoming = request;
                client.WriteStatus(200, "OK");
                client.WriteHeader("Content-Length", "0");
                client.EndHeader();
            };
            try {
                SendRequest("http://localhost:50000/simpleget");
            }
            catch(Exception e) {
                Assert.Fail(e.Message);
            }

            Assert.NotNull(incoming);
        }

        [Test]
        public void TestGetRequestWithQuery() {
            HttpRequest incoming=null;
            server.Request += (client, request) => {
                if (request.Resource != "/query")
                    return;

                incoming = request;
                client.WriteStatus(200, "OK");
                client.WriteHeader("Content-Length", "0");
                client.EndHeader();
            };

            try
            {
                SendRequest("http://localhost:50000/query?test=test&name=hans");
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }

            Assert.NotNull(incoming);
            Assert.AreEqual("GET", incoming.Method);
            Assert.AreEqual("test", incoming.GetParameter("test"));
            Assert.AreEqual("hans", incoming.GetParameter("name"));
        }

        [Test]
        public void TestPostRequest() {
            HttpRequest incoming = null;
            string data = null;

            server.Request += (client, request) => {
                if (request.Resource != "/post")
                    return;

                incoming = request;
                byte[] buffer = new byte[(request as HttpPostRequest)?.ContentLength??0];

                client.GetContent(request).Read(buffer, 0, buffer.Length);
                data = Encoding.UTF8.GetString(buffer);

                client.WriteStatus(200, "OK");
                client.WriteHeader("Content-Length", "0");
                client.EndHeader();
            };

            try
            {
                PostRequest("http://localhost:50000/post", "data and stuff");
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }

            Assert.NotNull(incoming);
            Assert.That(incoming is HttpPostRequest);
            Assert.AreEqual("POST", incoming.Method);
            Assert.AreEqual("data and stuff", data);
        }

        [Test]
        public void KeepAliveTest() {
            HashSet<HttpClient> clients=new HashSet<HttpClient>();
            int requestcount=0;

            server.Request += (client, request) => {
                if (request.Resource != "/keepalive")
                    return;

                ++requestcount;
                clients.Add(client);
                client.WriteStatus(200, "OK");
                client.WriteHeader("Content-Length", "0");
                client.EndHeader();
            };

            try
            {
                SendRequest("http://localhost:50000/keepalive?test=test&name=hans", "http://localhost:50000/keepalive?test=test&name=rolf", "http://localhost:50000/keepalive?test=test&name=ulf");
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }

            Assert.AreEqual(3, requestcount, "Received requests do not match up");
            Assert.AreEqual(1, clients.Count, "More than one connection was used");
        }
    }
}