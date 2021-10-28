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

        static void Main(string[] input)
        {
            SaveParameters(input);
            SelectMode();

            if (autotest)
                AutoTester.AutoTest(filePath);   
        }

        private static void SaveParameters(string[] input)
        {
            for (int i = 0; i < input.Length; i++)
            {
                if (input[i].Equals("-at"))
                    autotest = true;
                else if (input[i].Equals("-sa"))
                    mode = input[i];
                else if (input[i].Equals("-la"))
                    mode = input[i];
                else if (Directory.Exists(input[i]))
                    filePath = input[i];
                else if (File.Exists(input[i]))
                    filePath = input[i];
            }

            if (mode == null | filePath == null | (mode != null & !autotest & !File.Exists(filePath)))
            {
                if (mode == null)
                    Console.WriteLine("Error: incorrect working mode.");
                if (filePath == null)
                    Console.WriteLine("Error: file/directory not found.");
                if (mode != null & !autotest & !File.Exists(filePath))
                    Console.WriteLine("Error: file not found.");

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
                LexicalAnalizer1();
            }
            else if (mode.Equals("-sa"))
            {
                SyntaxAnalizer();
            }
        }

        private static void LexicalAnalizer1()
        {
            if (!autotest)
            {
                LexicalAnalizer lexicalAnalizer = new LexicalAnalizer(filePath);
                lexicalAnalizer.GetNewListLexemesData();
                lexicalAnalizer.LexemesDataOutputInConsole();
            }
            else
            {
                string[] filesPath = Directory.GetFiles(filePath);
                for (int j = 0; j < filesPath.Length; j++)
                {
                    if (!filesPath[j].Contains("check") & !filesPath[j].Contains("output"))
                    {
                        LexicalAnalizer lexicalAnalizer = new LexicalAnalizer(filesPath[j]);
                        lexicalAnalizer.GetNewListLexemesData();
                        lexicalAnalizer.LexemesDataOutputInFile();
                    }
                }
            }
        }

        private static void SyntaxAnalizer()
        {
            if (!autotest)
            {
                LexicalAnalizer lexicalAnalizer = new LexicalAnalizer(filePath);
                List<LexemeData> lexemesData = lexicalAnalizer.GetNewListLexemesData();
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
                        LexicalAnalizer lexicalAnalizer = new LexicalAnalizer(filesPath[j]);
                        List<LexemeData> lexemesData = lexicalAnalizer.GetNewListLexemesData();
                        SyntaxAnalizer syntaxAnalizer = new SyntaxAnalizer(lexemesData, filesPath[j]);
                        syntaxAnalizer.Analise();
                    }
                }
            }
        }
    }
}