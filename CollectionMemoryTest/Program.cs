using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollectionMemoryTest
{
    using System.Runtime.CompilerServices;

    using Microsoft.SqlServer.Server;

    class Program
    {
        const int numRecords = 10000;

        const int numColumns = 20;

        const int valueLength = 10;

        // [CompilationRelaxations(CompilationRelaxations.NoStringInterning)]
        static void Main(string[] args)
        {
            double theoreticalSize = numRecords * numColumns * valueLength;
            Console.WriteLine($"Theoretical data size: {theoreticalSize / 1024 / 1024:N1}Mb \t{theoreticalSize / 1024:N1}Kb \t{theoreticalSize:N0} bytes");
            ReportMemory("Before starting test");
            Console.WriteLine("------------------------");
            var t = ListListString();
            Console.WriteLine("------------------------");
            var t2 = ListListObject();
            Console.WriteLine("------------------------");
            var t3 = ListArrayString();
            Console.WriteLine("------------------------");
            var t4 = ListDictStringString();
            Console.WriteLine("------------------------");
            var t5 = ArrayArrayString();
            Console.WriteLine("------------------------");
            var t6 = ArrayArrayWithSimilarStrings();
            Console.WriteLine("------------------------");

            Console.WriteLine("Finished");
            GC.Collect();
            ReportMemory("After final collection");
            Console.ReadKey();
        }

        private static int ListListString()
        {  
            List<List<string>> records = new List<List<string>>();
            using (var stringCreator = new RandomStringCreator.StringCreator(100000))
            {
                records = Enumerable.Range(0, numRecords).Select(r =>
                        {
                            return Enumerable.Range(0, numColumns).Select(c => stringCreator.Get(valueLength)).ToList();
                        }).ToList();
            }
            //ReportMemory("ListListString Before GC");
            GC.Collect();
            ReportMemory("ListListString After GC");

            return records.Count; // Just trying to ensure it doesn't get optmised away
        }

        private static int ListListObject()
        {
            List<List<object>> records = new List<List<object>>();
            using (var stringCreator = new RandomStringCreator.StringCreator(100000))
            {
                records = Enumerable.Range(0, numRecords).Select(r =>
                {
                    return Enumerable.Range(0, numColumns).Select(c => stringCreator.Get(valueLength) as object).ToList();
                }).ToList();
            }
            //ReportMemory("ListListObject Before GC");
            GC.Collect();
            ReportMemory("ListListObject GC");

            return records.Count; // Just trying to ensure it doesn't get optmised away
        }

        private static int ListArrayString()
        {
            List<string[]> records = new List<string[]>();
            using (var stringCreator = new RandomStringCreator.StringCreator(100000))
            {
                records = Enumerable.Range(0, numRecords).Select(r =>
                {
                    return Enumerable.Range(0, numColumns).Select(c => stringCreator.Get(valueLength)).ToArray();
                }).ToList();
            }
            GC.Collect();
            ReportMemory("ListArrayString After GC");

            return records.Count; // Just trying to ensure it doesn't get optmised away
        }

        private static int ArrayArrayString()
        {
            string[][] records;
            using (var stringCreator = new RandomStringCreator.StringCreator(100000))
            {
                records = Enumerable.Range(0, numRecords).Select(r =>
                {
                    return Enumerable.Range(0, numColumns).Select(c => stringCreator.Get(valueLength)).ToArray();
                }).ToArray();
            }
            GC.Collect();
            ReportMemory("ArrayArrayString After GC");

            return records.Length; // Just trying to ensure it doesn't get optmised away
        }

        private static int ArrayArrayWithSimilarStrings()
        {
            // All rows are identical, though each column in each row is different
            string[][] records;
            {
                records = Enumerable.Range(0, numRecords).Select(r =>
                    {
                        return Enumerable.Range(0, numColumns).Select(c => c.ToString().PadLeft(valueLength)).ToArray();
                    }).ToArray();
            }
            GC.Collect();
            ReportMemory("ArrayArrayWithSimilarStrings After GC");

            return records.Length; // Just trying to ensure it doesn't get optmised away
        }

        private static int ListDictStringString()
        {
            var records = new List<Dictionary<string,string>>();
            using (var stringCreator = new RandomStringCreator.StringCreator(100000))
            {
                records = Enumerable.Range(0, numRecords).Select(r =>
                {
                    var dict = new Dictionary<string, string>();
                    for (int i = 0; i < numColumns; i++)
                    {
                        dict.Add($"Field{i}", stringCreator.Get(valueLength));
                    }

                    return dict;
                }).ToList();
            }
            GC.Collect();
            ReportMemory("ListDictStringString After GC");

            return records.Count; // Just trying to ensure it doesn't get optmised away
        }

        private static void ReportMemory(string label)
        {
            double mem = GC.GetTotalMemory(false);
            Console.WriteLine($"{label.PadRight(30)}. \t{mem/1024/1024:N1}Mb \t{mem/1024:N1}Kb \t{mem:N0} bytes");
        }
    }
}
