using HttpParser;
using NUnit.Framework;
using System;

namespace HttpParserTest
{
    public class Tests
    {
        HttpRequestLogParser _parser;

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Constructor_emptyParameter_shouldThrowAnException()
        {
            Assert.Throws<ArgumentException>(() => { new HttpRequestLogParser(string.Empty); });
        }

        [Test]
        public void ProcessLogFile_emptyLogFile_shouldNotThrowExceptionAndReturnZero()
        {
            var parser = new HttpRequestLogParser("../../../empty.log");
            Assert.DoesNotThrow(() =>
            {
                parser.ProcessLogFile();
                Assert.Zero(parser.GetNumberOfUniqueIpAddresses());
                Assert.Zero(parser.GetTopThreeMostActiveIpAddresses().Length);
                Assert.Zero(parser.GetTopThreeMostVisitedUrls().Length);
            });
        }

        [Test]
        public void ProcessLogFile_incorrectDataLogFile_shouldThrowAnException()
        {
            var parser = new HttpRequestLogParser("../../../incorrect-data.log");
            Assert.Throws<IndexOutOfRangeException>(() => { parser.ProcessLogFile(); });
            Assert.Throws<NullReferenceException>(() => { parser.GetNumberOfUniqueIpAddresses(); });
            Assert.Zero(parser.GetTopThreeMostActiveIpAddresses().Length);
            Assert.Zero(parser.GetTopThreeMostVisitedUrls().Length);
        }

        [Test]
        public void Constructor_correctDataLogFile_shouldThrowAnException()
        {
            var parser = new HttpRequestLogParser("../../../correct-data.log");
            Assert.DoesNotThrow(() => { parser.ProcessLogFile(); });
            Assert.AreEqual(parser.GetNumberOfUniqueIpAddresses(), 11);
            Assert.AreEqual(parser.GetTopThreeMostVisitedUrls().ToString(), "/docs/manage-websites/, /intranet-analytics/, http://example.net/faq/");
            Assert.AreEqual(parser.GetTopThreeMostActiveIpAddresses().ToString(), "168.41.191.40, 177.71.128.21, 50.112.00.11");
            
        }
    }
}