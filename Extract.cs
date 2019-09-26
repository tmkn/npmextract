using System.IO;

namespace Extract
{
    public class Extractor
    {
        private string jsonpath;

        public Extractor(string jsonpath)
        {
            this.jsonpath = jsonpath;
        }

        public void Read()
        {
            StreamReader file = new System.IO.StreamReader(this.jsonpath);
            string line;
            long lines = 0;
            long failures = 0;

            while((line = file.ReadLine()) != null)  
            {  
                if(line.StartsWith(@"{""id"":"""))
                {
                    lines++;
                    System.Console.WriteLine($"{this.Statistics(lines, failures)} {line.Substring(0, 48)}");
                }
                else
                {
                    failures++;
                } 
            }  
            
            file.Close();  
        }

        private string Statistics(long _ok, long _failures)
        {
            var ok = _ok.ToString();
            var failures = _failures.ToString();

            ok = ok.PadLeft(16);
            failures = failures.PadRight(16);

            return $"{ok}|{failures}";
        }
    }
}