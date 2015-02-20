namespace Specification.Halp
{
    using System;
    using NUnit.Framework;
    using Wamp;
    using WampSharp.V2;

    [TestFixture]
    class WampBaseTestFixture
    {
        protected const string ServerAddress = "ws://127.0.0.1:8080/ws";
        private DefaultWampHost _host;

        [SetUp]
        public void Setup()
        {
            _host = WampHostHelper.GetHost(ServerAddress);

            _host.Open();
        }

        [TearDown]
        public void Teardown()
        {
            _host.Dispose();
        }

        protected void WaitForExpected<T>(ref T actual, T expected)
        {
            var start = DateTime.Now;
            while (!actual.Equals(expected))
            {
                if (DateTime.Now - start > TimeSpan.FromSeconds(2))
                {
                    return;
                }
            }
        }
    }
}
