using System.IO;
using System.Text.Json;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System;

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
            List<long> offsets = new List<long>()
            {
                0
            };
            //UnicodeEncoding unicodeEncoding = new UnicodeEncoding();
            Encoding unicodeEncoding = Encoding.UTF8;

            //System.Console.WriteLine(file.BaseStream.Position);

            List<long> offsets2 = new List<long>()
            {
                0
            };

            while((line = file.ReadLine()) != null)  
            {  
                offsets2.Add(file.BaseStream.Position);
                try
                {
                    //int offset = offsets.Sum() + unicodeEncoding.GetBytes(line).Length + 2;
                    offsets.Add(unicodeEncoding.GetBytes(line).Length + 2);

                    
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

                            //Encoding encodingUTF8 = Encoding.UTF8;
                            Byte[] encodedBytes = unicodeEncoding.GetBytes(json);
                            var byteLength = encodedBytes.Length;
                            var strLength = json.Length;

                            System.Console.WriteLine("");
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

            long _offset = 0;
            foreach(int offset in offsets)
            {
                _offset += offset;
                
                file.BaseStream.Seek(_offset, SeekOrigin.Begin);
                byte[] test2 = new byte[32];

                file.BaseStream.Read(test2, 0, 31);
                var str = unicodeEncoding.GetString(test2);

                System.Console.WriteLine(str);
            }

            var test = file.CurrentEncoding;
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