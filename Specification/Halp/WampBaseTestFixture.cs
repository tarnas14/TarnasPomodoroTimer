namespace Specification.Halp
{
    using System;
    using NUnit.Framework;
    using WampSharp.V2;

    [TestFixture]
    class WampBaseTestFixture
    {
        protected const string ServerAddress = "ws://127.0.0.1:8080/ws";
        protected DefaultWampHost Host;

        [SetUp]
        public void Setup()
        {
            Host = WampHostHelper.GetHost(ServerAddress);

            Host.Open();
        }

        [TearDown]
        public void Teardown()
        {
            Host.Dispose();
            Host = null;
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
