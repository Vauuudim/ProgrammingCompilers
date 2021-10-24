using System;
using System.Collections.Generic;
using System.IO;

namespace Programming_Compilers_Pascal
{
    public static class AutoTester
    {
        private static List<string> checks = new List<string>();
        private static List<string> outputs = new List<string>();
        private static List<bool> results = new List<bool>();

        private static int trueCount = 0;
        private static int test = 1;

        private static string[] filesPath;

        public static void AutoTest(string filePathForTests)
        {
            filesPath = Directory.GetFiles(filePathForTests);

            FillListsForComparison();
            ÑompareLists();
            OutputResults();
        }

        private static void FillListsForComparison()
        {
            for (int i = 0; i < filesPath.Length; i++)
            {
                CheckAndAddToList(filesPath[i], "check");
                CheckAndAddToList(filesPath[i], "output");
            }
        }

        private static void CheckAndAddToList(string filePath, string subString)
        {
            if (filePath.Contains(subString))
            {
                using (StreamReader streamReader = new StreamReader(filePath))
                {
                    if (subString.Equals("check"))
                        checks.Add(streamReader.ReadToEnd());
                    else
                        outputs.Add(streamReader.ReadToEnd());
                }
            }
        }

        private static void ÑompareLists()
        {
            for (int i = 0; i < checks.Count; i++)
            {
                bool isEquals = false;
                if (checks[i].Equals(outputs[i]))
                {
                    isEquals = true;
                    trueCount++;
                }

                results.Add(isEquals);
                test++;
            }
        }

        private static void OutputResults()
        {
            for (int i = 0; i < results.Count; i++)
            {
                Console.WriteLine("Test " + (i + 1) + " " + results[i]);
            }

            Console.WriteLine("--------------------------------");
            Console.WriteLine("Number of completed tests: " + trueCount);
            Console.WriteLine("Number of failed tests: " + (test - trueCount - 1));
        }
    }
}
