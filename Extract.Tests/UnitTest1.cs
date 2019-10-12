using System;
using Xunit;

using Extract;

namespace Extract.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var extractor = new Extractor(@"test.json", "");

            extractor.Read();

            Assert.True(true);
        }
    }
}
