using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace Extract
{
    public class LookupCreator
    {
        string jsonPath;
        public List<LookupEntry> lookups = new List<LookupEntry>();

        static readonly string StartToken = @"{""id"":""";
        
        public LookupCreator(string jsonPath)
        {
            this.jsonPath = jsonPath;
        }

        public void Parse()
        {
            using(StreamReader reader = new System.IO.StreamReader(this.jsonPath))
            {
                int count = 1;
                int errors = 0;
                string? line;
                long currentOffset = 0;
                Encoding utf8Encoding = Encoding.UTF8;

                while((line = reader.ReadLine()) != null)  
                {
                    if(line.StartsWith(StartToken))
                    {
                        try
                        {
                            var json = line.EndsWith(",") ?  line.Substring(0, line.Length - 1) : line;
                            var lookup = new LookupEntry
                            {
                                name = GetName(json),
                                offset = currentOffset,
                                length = utf8Encoding.GetBytes(json).Length
                            };

                            lookups.Add(lookup);

                            System.Console.WriteLine($"{count.ToString().PadLeft(12)} {errors} {lookup.name}");
                            count++;
                        }
                        catch(Exception e)
                        {
                            errors++;
                            System.Console.WriteLine(e);
                        }
                    }

                    currentOffset += utf8Encoding.GetBytes(line).Length + 2;
                }
            }
        }

        public void Write(string outPath)
        {
            List<string> data = new List<string>();

            using(StreamReader reader = new System.IO.StreamReader(this.jsonPath))
            {
                Encoding utf8Encoding = Encoding.UTF8;
                
                foreach(var lookup in lookups)
                {
                    /*byte[] read = new byte[lookup.length];
                    
                    reader.BaseStream.Seek(lookup.offset, SeekOrigin.Begin);
                    reader.BaseStream.Read(read, 0, (int)lookup.length);

                    string test = utf8Encoding.GetString(read);*/
                    data.Add($"{lookup.name} {lookup.offset} {lookup.length}");
                }
            }

            File.WriteAllLines(outPath, data, Encoding.UTF8);
        }

        private string GetName(string json)
        {            
            var options = new JsonDocumentOptions
            {
                AllowTrailingCommas = true
            };

            using (JsonDocument document = JsonDocument.Parse(json.Substring(0, json.Length), options))
            {
                return document.RootElement.GetProperty("key").GetString();
            }
        }
    }

    public class LookupEntry
    {
        public string? name;
        public long offset;
        public long length;
    }
}