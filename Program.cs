using System;

using Extract;

namespace npmextract
{
    class Program
    {
        static void Main(string[] args)
        {
            var extractor = new Extractor(@"test.json", @"D:/npm");

            extractor.Read();
        }
    }
}
