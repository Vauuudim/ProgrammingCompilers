using System;
using System.Collections.Generic;
using System.IO;

namespace Programming_Compilers_Pascal
{
    public class LexicalAnalizer
    {
        private List<LexemeData> lexemesData = new List<LexemeData>();
        private string filePath = "";
        private bool autotest;

        private ErrorData errorData = new ErrorData();

        private string currentLexeme = null;
        private int currentIndexLine = 0;
        private int currentIndexInLine = 0;
        private ClassLexeme currentLexemeClass;

        private FileReader fileReader;
        private string symbol;
        private bool isNew = true;

        public LexicalAnalizer (string filePath, bool autotest = false)
        {
            this.filePath = filePath;
            this.autotest = autotest;
        }

        public List<LexemeData> ¿nalysis()
        {
            using (StreamReader streamReader = new StreamReader(filePath))
            {
                fileReader = new FileReader(streamReader);
                while (errorData.IsProblemIsNull())
                {
                    symbol = fileReader.ReadNextSymbolAndChangeIndexes();
                    if (symbol == null)
                    {
                        break;
                    }

                    ClassLexeme symbolClass = LexemeVerification.GetClass(symbol);
                    if (!IsRequired—lass(symbolClass, ClassLexeme.control))
                    {
                        if (IsRequired—lass(symbolClass, ClassLexeme.NONAME))
                        {
                            SaveErrorForSymbol("found unknown symbol");
                            lexemesData.Add(new LexemeData(fileReader.GetIndexLine(), fileReader.GetIndexSymbol(), symbolClass, symbol, symbol));
                            break;
                        }

                        if (IsRequired—lass(symbolClass, ClassLexeme.standart))
                        {
                            if (isNew)
                            {
                                currentIndexLine = fileReader.GetIndexLine();
                                currentIndexInLine = fileReader.GetIndexSymbol();
                            }
                            currentLexeme += symbol;
                            isNew = false;
                        }
                        else
                        {
                            CheckAndAddLexemeToListAndUpdateParameters();
                        }

                        if (IsRequired—lass(symbolClass, ClassLexeme.keyword, ClassLexeme.operation, ClassLexeme.type, ClassLexeme.separator))
                        {
                            if (IsRequired—lass(symbolClass, ClassLexeme.standart))
                                symbolClass = ClassLexeme.variable;
                            lexemesData.Add(new LexemeData(fileReader.GetIndexLine(), fileReader.GetIndexSymbol(), symbolClass, symbol, symbol));
                            CheckAndAddLexemeToListAndUpdateParameters();
                        }
                    }
                    else
                    {
                        CheckAndAddLexemeToListAndUpdateParameters();
                    }

                    if (currentLexeme != null)
                    {
                        currentLexemeClass = LexemeVerification.GetClass(currentLexeme);
                    }
                }
                CheckAndAddLexemeToListAndUpdateParameters();
            }

            MakeEdits();
            OutputResults();
            return lexemesData;
        }

        private void OutputResults()
        {
            if (!autotest)
            {
                for (int i = 0; i < lexemesData.Count; i++)
                {
                    if (!errorData.IsProblemIsNull())
                    {
                        if (errorData.GetLineIndex() == lexemesData[i].indexLine & errorData.GetSymbolIndex() == lexemesData[i].indexInLine)
                        {
                            Console.WriteLine("" + lexemesData[i].indexLine + '\t' + lexemesData[i].indexInLine + '\t' + errorData.GetErrorText());
                            break;
                        }
                    }
                    Console.WriteLine("" + lexemesData[i].indexLine + '\t' + lexemesData[i].indexInLine + '\t' + lexemesData[i].classLexeme + '\t' + "\"" + lexemesData[i].value + "\"" + '\t' + lexemesData[i].code);
                }
            }
            else
            {
                using (StreamWriter streamWriter = new StreamWriter(filePath.Remove(filePath.Length - 4, 4) + "_output.txt"))
                {
                    for (int i = 0; i < lexemesData.Count; i++)
                    {
                        if (!errorData.IsProblemIsNull())
                        {
                            if (errorData.GetLineIndex() == lexemesData[i].indexLine & errorData.GetSymbolIndex() == lexemesData[i].indexInLine)
                            {
                                streamWriter.WriteLine("" + lexemesData[i].indexLine + '\t' + lexemesData[i].indexInLine + '\t' + errorData.GetErrorText());
                                break;
                            }
                        }
                        streamWriter.WriteLine("" + lexemesData[i].indexLine + '\t' + lexemesData[i].indexInLine + '\t' + lexemesData[i].classLexeme + '\t' + "\"" + lexemesData[i].value + "\"" + '\t' + lexemesData[i].code);
                    }
                }
            }
        }

        private void CheckAndAddLexemeToListAndUpdateParameters()
        {
            if (!isNew)
            {
                CheckLexeme();
                AddLexemeToList();
                UpdateParameters();
            }
        }

        private void CheckLexeme()
        {
            if (IsRequired—lass(currentLexemeClass, ClassLexeme.NONAME, ClassLexeme.standart))
                currentLexemeClass = ClassLexeme.variable;
        }

        private void AddLexemeToList()
        {
            lexemesData.Add(new LexemeData(currentIndexLine, currentIndexInLine, currentLexemeClass, currentLexeme, currentLexeme));
        }

        private void UpdateParameters()
        {
            isNew = true;
            currentLexeme = null;
        }

        ///<summary> œÓ‚ÂˇÂÚ ‡‚ÂÌ ÎË ÍÎ‡ÒÒ Ó‰ÌÓÏÛ ËÁ ÛÍ‡Á‡ÌÌ˚ı. </summary>
        private bool IsRequired—lass(ClassLexeme lexemeClass, params ClassLexeme[] classes)
        {
            for (int i = 0; i < classes.Length; i++)
            {
                if (lexemeClass == classes[i])
                    return true;
            }
            return false;
        }

        private void MakeEdits()
        {
            for (int i = 0; i < lexemesData.Count; i++)
            {
                if (lexemesData[i].code.Equals("\'"))
                {
                    if (i < lexemesData.Count - 1)
                    {
                        int nextLexemeIndex = i + 1;
                        while (!lexemesData[nextLexemeIndex].code.Equals("\'"))
                        {
                            if (lexemesData[i].indexLine != lexemesData[nextLexemeIndex].indexLine | i == lexemesData.Count - 1)
                            {
                                SaveErrorForLexeme(lexemesData[i], "closing symbol not found");
                                return;
                            }
                            AddToCurrentAndRemoveNextLexeme(lexemesData[i], lexemesData[i + 1]);
                        }
                        AddToCurrentAndRemoveNextLexeme(lexemesData[i], lexemesData[i + 1]);
                        lexemesData[i].SetClass(ClassLexeme.@string);
                    }
                    else
                    {
                        SaveErrorForLexeme(lexemesData[i], "closing symbol not found");
                    }
                }

                if (IsRequired—lass(lexemesData[i].classLexeme, ClassLexeme.variable))
                {
                    if (IsInt(lexemesData[i].code))
                        lexemesData[i].SetClass(ClassLexeme.integer);
                }
            }

            for (int i = 0; i < lexemesData.Count; i++)
            {
                if (i < lexemesData.Count - 3)
                {
                    bool isRequired1 = IsRequired—lass(lexemesData[i].classLexeme, ClassLexeme.variable, ClassLexeme.integer, ClassLexeme.real);
                    bool isRequired2 = IsRequired—lass(lexemesData[i + 2].classLexeme, ClassLexeme.variable, ClassLexeme.integer, ClassLexeme.real);
                    if (isRequired1 & lexemesData[i + 1].code.Equals(".") & isRequired2 & lexemesData[i + 3].code.Equals("."))
                        SaveErrorForLexeme(lexemesData[i + 3], "incorrect variable format");
                }

                if (i < lexemesData.Count - 2)
                {
                    bool isRequired1 = IsRequired—lass(lexemesData[i].classLexeme, ClassLexeme.integer);
                    bool isRequired2 = IsRequired—lass(lexemesData[i + 2].classLexeme, ClassLexeme.integer);
                    if (isRequired1 & lexemesData[i + 1].code.Equals(".") & isRequired2)
                    {
                        if (lexemesData[i].indexLine == lexemesData[i + 1].indexLine & lexemesData[i].indexLine == lexemesData[i + 2].indexLine)
                        {
                            lexemesData[i].value = double.Parse(lexemesData[i].code + "," + lexemesData[i + 2].code).ToString();
                            lexemesData[i].code += "." + lexemesData[i + 2].code;
                            lexemesData[i].SetClass(ClassLexeme.real);
                            lexemesData.RemoveRange(i + 1, 2);
                        }
                        else
                        {
                            SaveErrorForLexeme(lexemesData[i + 2], "incorrect variable format");
                        }
                    }
                }
            }
        }

        private void AddToCurrentAndRemoveNextLexeme(LexemeData currentLexeme, LexemeData nextLexeme)
        {
            currentLexeme.code += nextLexeme.code;
            currentLexeme.value += nextLexeme.code;
            lexemesData.Remove(nextLexeme);
        }

        private bool IsInt(string lexemeCode)
        {
            for (int i = 0; i < lexemeCode.Length; i++)
            {
                if (!(lexemeCode[i] >= '0' & lexemeCode[i] <= '9'))
                    return false;
            }
            return true;
        }

        private void SaveErrorForLexeme(LexemeData lexemeData, string errorMessage = "")
        {
            errorData = new ErrorData(lexemeData.indexLine, lexemeData.indexInLine, "error: " + errorMessage + " (" + lexemeData.code + ")");
        }

        private void SaveErrorForSymbol(string errorMessage = "")
        {
            errorData = new ErrorData(fileReader.GetIndexLine(), fileReader.GetIndexSymbol(), "error: " + errorMessage + " (" + symbol + ")");
        }

        public ErrorData GetError()
        {
            //return errorData.IsProblemIdentified() ? errorData : null;
            return errorData;
        }
    }
}