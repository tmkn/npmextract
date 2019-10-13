using System;

using Extract;

namespace npmextract
{
    class Program
    {
        static void Main(string[] args)
        {
            var lookup = new LookupCreator(@"test.json");

            lookup.Parse();
            lookup.Write(@"test.lookup.txt");
        }
    }
}
