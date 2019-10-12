using System.IO;
using System.Text.Json;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Extract
{
    public class Extractor
    {
        private readonly string jsonpath;
        private readonly string targetfolder;
        private readonly List<string> errors = new List<string>();

        public Extractor(string jsonpath, string targetfolder)
        {
            this.jsonpath = jsonpath;
            this.targetfolder = targetfolder;
        }

        public void Read()
        {
            StreamReader file = new System.IO.StreamReader(this.jsonpath);
            string? line;
            long lines = 0;
            long failures = 0;

            while((line = file.ReadLine()) != null)  
            {  
                try
                {
                    if(line.StartsWith(@"{""id"":"""))
                    {
                        lines++;
                    
                        var options = new JsonDocumentOptions
                        {
                            AllowTrailingCommas = true
                        };

                        var json = line.EndsWith(",") ?  line.Substring(0, line.Length - 1) : line;
                        using (JsonDocument document = JsonDocument.Parse(json, options))
                        {
                            var name = document.RootElement.GetProperty("key").GetString();

                            System.Console.WriteLine($"{this.Statistics(lines, failures)} {name} {json.Length/1024}kb");

                            //Save(name, json);
                        }
                    }
                    else
                    {
                        failures++;
                    }  
                    
                    if(errors.Count > 0)
                        File.WriteAllLines("errors.txt", errors);
                }
                catch(System.Exception e)
                {
                    failures++;
                    errors.Add(e.ToString());
                }
            }

            file.Close();
        }

        private string Statistics(long _ok, long _failures)
        {
            var ok = _ok.ToString();
            var failures = _failures.ToString();

            ok = ok.PadLeft(12);
            failures = failures.PadRight(12);

            return $"{ok}|{failures}";
        }

        private void Save(string name, string json)
        {
            string fullPath = Path.Combine(targetfolder, SHA1Filename(name));

            if(!File.Exists(fullPath))
                File.WriteAllText(fullPath, json);
            else
                System.Console.WriteLine($"Skipping {name}");
        }

        private string SHA1Filename(string name)
        {
            using (SHA1Managed sha1 = new SHA1Managed())
            {
                var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(name));
                var sb = new StringBuilder(hash.Length * 2);

                foreach (byte b in hash)
                {
                    sb.Append(b.ToString("x2"));
                }

                return $"{sb.ToString()}.json";
            }
        }
    }
}