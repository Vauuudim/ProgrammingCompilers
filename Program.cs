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
            Сomparison();
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
                LexicalAnalysis();
            }
            else if (mode.Equals("-sa"))
            {
                SyntaxAnalizer();
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
                if (filesPath.Length > 0)
                {
                    for (int j = 0; j < filesPath.Length; j++)
                    {
                        if (!filesPath[j].Contains("check") & !filesPath[j].Contains("output"))
                        {
                            LexicalAnalizer lexicalAnalizer = new LexicalAnalizer(filesPath[j]);
                            List<LexemeData> lexemesData = lexicalAnalizer.Аnalysis();
                            SyntaxAnalizer syntaxAnalizer = new SyntaxAnalizer(lexemesData, filesPath[j]);
                            syntaxAnalizer.Analise();
                        }
                    }
                }
            }
        }

        private static List<LexemeData> LexicalAnalysis()
        {
            if (!autotest)
            {
                LexicalAnalizer lexicalAnalizer = new LexicalAnalizer(filePath);
                List<LexemeData> lexemesData = lexicalAnalizer.Аnalysis();

                for (int i = 0; i < lexemesData.Count; i++)
                {
                    if (lexicalAnalizer.errorLineIndex == lexemesData[i].indexLine & lexicalAnalizer.errorSymbolIndex == lexemesData[i].indexInLine)
                    {
                        Console.WriteLine("" + lexemesData[i].indexLine + '\t' + lexemesData[i].indexInLine + '\t' + lexicalAnalizer.errorText + " (" + lexicalAnalizer.errorLexeme + ")");
                        break;
                    }
                    Console.WriteLine("" + lexemesData[i].indexLine + '\t' + lexemesData[i].indexInLine + '\t' + lexemesData[i].classLexeme + '\t' + lexemesData[i].value + '\t' + lexemesData[i].code);
                }
                return lexemesData;
            }
            else
            {
                string[] filesPath = Directory.GetFiles(filePath);
                if (filesPath.Length > 0)
                {
                    for (int j = 0; j < filesPath.Length; j++)
                    {
                        if (!filesPath[j].Contains("check") & !filesPath[j].Contains("output"))
                        {
                            LexicalAnalizer lexicalAnalizer = new LexicalAnalizer(filesPath[j]);
                            List<LexemeData> lexemesData = lexicalAnalizer.Аnalysis();

                            using (StreamWriter streamWriter = new StreamWriter(filesPath[j].Remove(filesPath[j].Length - 4, 4) + "_output.txt"))
                            {
                                for (int i = 0; i < lexemesData.Count; i++)
                                {
                                    if (lexicalAnalizer.errorLineIndex == lexemesData[i].indexLine & lexicalAnalizer.errorSymbolIndex == lexemesData[i].indexInLine)
                                    {
                                        streamWriter.WriteLine("" + lexemesData[i].indexLine + '\t' + lexemesData[i].indexInLine + '\t' + lexicalAnalizer.errorText + " (" + lexicalAnalizer.errorLexeme + ")");
                                        break;
                                    }
                                    else
                                    {
                                        streamWriter.WriteLine("" + lexemesData[i].indexLine + '\t' + lexemesData[i].indexInLine + '\t' + lexemesData[i].classLexeme + '\t' + '"' + lexemesData[i].value + '"' + '\t' + lexemesData[i].code);
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Files not found");
                }
                return null;
            }
        }

        private static void Сomparison()
        {
            if (autotest)
            {
                List<string> checks = new List<string>();
                List<string> outputs = new List<string>();

                string[] filesPath = Directory.GetFiles(filePath);
                if (filesPath.Length > 0)
                {
                    for (int i = 0; i < filesPath.Length; i++)
                    {
                        if (filesPath[i].Contains("check"))
                        {
                            using (StreamReader streamReader = new StreamReader(filesPath[i]))
                            {
                                checks.Add(streamReader.ReadToEnd());
                            }
                        }

                        if (filesPath[i].Contains("output"))
                        {
                            using (StreamReader streamReader = new StreamReader(filesPath[i]))
                            {
                                outputs.Add(streamReader.ReadToEnd());
                            }
                        }
                    }
                }

                int trueCount = 0;
                int test = 1;
                for (int i = 0; i < checks.Count; i++)
                {
                    bool isEquals = false;
                    if (checks[i].Equals(outputs[i]))
                    {
                        isEquals = true;
                        trueCount++;
                    }
                    
                    Console.WriteLine("Test " + test + " " + isEquals);
                    test++;
                }

                Console.WriteLine("--------------------------------");
                Console.WriteLine("Number of completed tests: " + trueCount);
                Console.WriteLine("Number of failed tests: " + (test - trueCount - 1));
            }
        }
    }
}