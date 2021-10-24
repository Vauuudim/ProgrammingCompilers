using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Programming_Compilers_Pascal
{
    class Program
    {
        private static string filePath;
        private static string mode;
        private static bool autotest = false;
        private static bool isClose = false;

        static void Main(string[] input)
        {
            SaveParameters(input);
            СheckParameters();
            SelectMode();

            if (autotest)
                AutoTester.AutoTest(filePath);   
        }

        private static void SaveParameters(string[] input)
        {
            if (!(input.Length < 3))
            {
                if (input[2].Equals("-at"))
                    autotest = true;
                else
                    Console.WriteLine("Error: incorrect autotest command.");

                filePath = input[0];
                mode = input[1];
            }
            else if (!(input.Length < 2))
            {
                autotest = false;
                filePath = input[0];
                mode = input[1];
            }
            else
            {
                Console.WriteLine("Error: missing/incorrect input data.");
                Console.WriteLine("Process close.");
                Process.GetCurrentProcess().Kill();
            }
        }

        private static void СheckParameters()
        {
            if (!(mode.Equals("-la") | mode.Equals("-sa")))
            {
                Console.WriteLine("Error: incorrect working mode.");
                isClose = true;
            }

            if (autotest)
            {
                if (!Directory.Exists(filePath))
                {
                    Console.WriteLine("Error: Directory for autotests not found");
                    isClose = true;
                }
            }
            else
            {
                if (!File.Exists(filePath))
                {
                    Console.WriteLine("Error: file not found.");
                    isClose = true;
                }
            }

            if (isClose)
            {
                Console.WriteLine("Process close.");
                Process.GetCurrentProcess().Kill();
            }

            Console.WriteLine("--------------------------------");
            Console.WriteLine("Path: " + filePath);
            Console.WriteLine("Working mode: " + mode);
            Console.WriteLine("Autotests " + (autotest == true ? "enabled" : "disabled"));
            Console.WriteLine("--------------------------------");
        }

        private static void SelectMode()
        {
            if (mode.Equals("-la"))
            {
                LexicalAnalizer();
            }
            else if (mode.Equals("-sa"))
            {
                SyntaxAnalizer();
            }
        }

        private static void LexicalAnalizer()
        {
            if (!autotest)
            {
                LexicalAnalizer lexicalAnalizer = new LexicalAnalizer(filePath);
                List<LexemeData> lexemesData = lexicalAnalizer.Аnalysis();
            }
            else
            {
                string[] filesPath = Directory.GetFiles(filePath);
                for (int j = 0; j < filesPath.Length; j++)
                {
                    if (!filesPath[j].Contains("check") & !filesPath[j].Contains("output"))
                    {
                        LexicalAnalizer lexicalAnalizer = new LexicalAnalizer(filesPath[j], true);
                        List<LexemeData> lexemesData = lexicalAnalizer.Аnalysis();
                    }
                }
            }
        }

        private static void SyntaxAnalizer()
        {
            if (!autotest)
            {
                LexicalAnalizer lexicalAnalizer = new LexicalAnalizer(filePath);
                List<LexemeData> lexemesData = lexicalAnalizer.Аnalysis();
                SyntaxAnalizer syntaxAnalizer = new SyntaxAnalizer(lexemesData);
                syntaxAnalizer.Analise();
            }
            else
            {
                string[] filesPath = Directory.GetFiles(filePath);
                for (int j = 0; j < filesPath.Length; j++)
                {
                    if (!filesPath[j].Contains("check") & !filesPath[j].Contains("output"))
                    {
                        LexicalAnalizer lexicalAnalizer = new LexicalAnalizer(filesPath[j], true);
                        List<LexemeData> lexemesData = lexicalAnalizer.Аnalysis();
                        SyntaxAnalizer syntaxAnalizer = new SyntaxAnalizer(lexemesData, filesPath[j]);
                        syntaxAnalizer.Analise();
                    }
                }
            }
        }
    }
}