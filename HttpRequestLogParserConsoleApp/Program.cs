using HttpParser;
using System;

namespace HttpRequestLogParserConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello Azenix Team!\n");

            try
            {
                var parser = new HttpRequestLogParser(@"..\..\..\..\data.log");
                
                parser.ProcessLogFile();

                Console.WriteLine("Number of unique IP addresses: {0}", parser.GetNumberOfUniqueIpAddresses());
                Console.WriteLine("Top 3 most visited URLs: {0}", parser.GetTopThreeMostVisitedUrls());
                Console.WriteLine("Top 3 most active IP addresses: {0}", parser.GetTopThreeMostActiveIpAddresses());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                Console.Read();
            }
        }
    }
}
