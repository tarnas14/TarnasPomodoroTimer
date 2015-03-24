namespace Specification.Halp
{
    using System;
    using NUnit.Framework;
    using WampSharp.V2;

    [TestFixture]
    class WampBaseTestFixture
    {
        private int _counter;
        private const string _serverAddress = "ws://127.0.0.1:8080/ws";
        protected string ServerAddress { get; set; }
        protected DefaultWampHost Host;

        [SetUp]
        public void Setup()
        {
            ServerAddress = string.Format("{0}{1}", _serverAddress, ++_counter);

            if (Host != null)
            {
                Host.Dispose();
            }

            Host = new DefaultWampHost(ServerAddress);
            Host.Open();
        }

        protected void WaitForExpected<T>(T actual, T expected)
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
