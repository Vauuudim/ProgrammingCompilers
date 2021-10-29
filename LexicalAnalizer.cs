using System;
using System.Collections.Generic;
using System.IO;

namespace Programming_Compilers_Pascal
{
    public class LexicalAnalizer
    {
        private List<LexemeData> lexemesData = new List<LexemeData>();
        private string filePath = "";
        private FileReader fileReader;
        private ErrorData errorData = new ErrorData();

        private string currentLexeme = null;
        private string currentLexemeValue = null;
        private int currentIndexLine = 0;
        private int currentIndexSymbol = 0;
        private ClassLexeme currentLexemeClass;

        private string previousLexeme = null;
        private int previousLexemeIndexLine = 0;
        private int previousLexemeIndexSymbol = 0;

        private string symbol = null;
        private ClassLexeme symbolClass;

        private string previousSymbol = null;
        private ClassLexeme previousSymbolClass;

        public LexicalAnalizer(string filePath)
        {
            this.filePath = filePath;
        }

        public List<LexemeData> GetNewListLexemesData()
        {
            errorData = new ErrorData();
            using (StreamReader streamReader = new StreamReader(filePath))
            {
                fileReader = new FileReader(streamReader);
                while (errorData.IsProblemIsNull())
                {
                    LexemeData lexeme = GetNextLexeme();
                    if (lexeme == null)
                        break;
                    if (errorData.IsProblemIsNull())
                        lexemesData.Add(lexeme);
                }
            }
            return lexemesData;
        }

        public LexemeData GetNextLexeme()
        {
            LexemeData lexemeData = null;

            currentLexeme = null;

            if (symbol == null)
                if (VerifyNextSymbolAndCheckOnNull())
                    return lexemeData;
            //SPACE/CONTROL
            SkipSpaceAndControl();

            //RUS...
            if (IsRequired—lass(symbolClass, ClassLexeme.NONAME))
            {
                SaveErrorForSymbol("found unknown symbol");
                return lexemeData;
            }

            //COMMENTARY
            if (symbol.Equals("{"))
            {
                SkipBlockCommentary();
                if (symbol == null)
                    return lexemeData;
            }
            if (symbol.Equals("/"))
            {
                if (previousSymbol != null)
                {
                    if (previousSymbol.Equals("/") & symbol.Equals("/"))
                    {
                        lexemesData.Remove(lexemesData[lexemesData.Count - 1]);
                        SkipStringCommentary();
                    }
                }
                SavePreviousSymbol();
                if (symbol == null)
                    return lexemeData;
            }

            //SPACE/CONTROL (AGAIN)
            SkipSpaceAndControl();

            //STRINGS
            if (symbol.Equals("'"))
            {
                SaveIndexes();
                currentLexeme = symbol;

                if (VerifyNextSymbolAndCheckOnNull())
                {
                    SaveErrorForLexeme("closing symbol not found");
                    return lexemeData;
                }

                while (!symbol.Equals("'"))
                {
                    currentLexeme += symbol;
                    if (VerifyNextSymbolAndCheckOnNull())
                    {
                        SaveErrorForLexeme("closing symbol not found");
                        return lexemeData;
                    }
                    if (currentIndexLine != fileReader.GetIndexLine())
                    {
                        currentLexeme = currentLexeme.Remove(currentLexeme.Length - 2, 2);
                        SaveErrorForLexeme("closing symbol not found");
                        return lexemeData;
                    }
                }
                currentLexeme += symbol;
                symbol = null;
                return new LexemeData(currentIndexLine, currentIndexSymbol, ClassLexeme.@string, currentLexeme, currentLexeme);
            }

            //OPERATIONS
            if (IsRequired—lass(symbolClass, ClassLexeme.operation))
            {
                SaveIndexes();
                SavePreviousSymbol();

                if (VerifyNextSymbolAndCheckOnNull())
                    return lexemeData;

                currentLexeme = previousSymbol + symbol;
                currentLexemeClass = LexemeVerification.GetClass(currentLexeme);

                if (IsRequired—lass(symbolClass, ClassLexeme.operation))
                {
                    if (IsTwoSymbolOperation())
                    {
                        symbol = null;
                        return new LexemeData(currentIndexLine, currentIndexSymbol, currentLexemeClass, currentLexeme, currentLexeme);
                    }
                }
                        
                return new LexemeData(currentIndexLine, currentIndexSymbol, previousSymbolClass, previousSymbol, previousSymbol);
            }

            //COMPOSITE (MULTI-LETTER)
            if (IsRequired—lass(symbolClass, ClassLexeme.standart))
            {
                SaveIndexes();

                while (IsRequired—lass(symbolClass, ClassLexeme.standart, ClassLexeme.number))
                {
                    currentLexeme += symbol;
                    currentLexemeClass = LexemeVerification.GetClass(currentLexeme);

                    if (IsRequired—lass(currentLexemeClass, ClassLexeme.keyword, ClassLexeme.type, ClassLexeme.operation))
                    {
                        LexemeData newLexeme = null;
                        if (previousLexeme != null)
                        {
                            ClassLexeme newClass = LexemeVerification.GetClass(previousLexeme + currentLexeme);
                            if (IsRequired—lass(newClass, ClassLexeme.keyword, ClassLexeme.type, ClassLexeme.operation))
                            {
                                currentLexeme = previousLexeme + currentLexeme;
                                UpdateCurrentLexeme();

                                newLexeme = new LexemeData(previousLexemeIndexLine, previousLexemeIndexSymbol, currentLexemeClass, currentLexeme, currentLexeme);
                                previousLexeme = currentLexeme;
                            }
                        }

                        symbol = null;

                        if (newLexeme != null)
                        {
                            lexemesData.Remove(lexemesData[lexemesData.Count - 1]);
                            previousLexeme = null;
                            return newLexeme;
                        }

                        previousLexeme = currentLexeme;
                        previousLexemeIndexLine = currentIndexLine;
                        previousLexemeIndexSymbol = currentIndexSymbol;

                        return new LexemeData(currentIndexLine, currentIndexSymbol, currentLexemeClass, currentLexeme, currentLexeme);
                    }

                    if (previousLexeme != null)
                    {
                        LexemeData newLexeme = null;
                        ClassLexeme newClass = LexemeVerification.GetClass(previousLexeme + currentLexeme);

                        if (IsRequired—lass(newClass, ClassLexeme.keyword, ClassLexeme.type, ClassLexeme.operation))
                        {
                            currentLexeme = previousLexeme + currentLexeme;
                            UpdateCurrentLexeme();

                            newLexeme = new LexemeData(previousLexemeIndexLine, previousLexemeIndexSymbol, currentLexemeClass, currentLexeme, currentLexeme);
                            previousLexeme = currentLexeme;

                        }
                        if (newLexeme != null)
                        {
                            lexemesData.Remove(lexemesData[lexemesData.Count - 1]);
                            symbol = null;
                            previousLexeme = null;
                            return newLexeme;
                        }
                    }

                    SavePreviousSymbol();

                    if (VerifyNextSymbolAndCheckOnNull())
                    {
                        UpdateCurrentLexeme();
                        return new LexemeData(currentIndexLine, currentIndexSymbol, currentLexemeClass, currentLexemeValue, currentLexeme);
                    }
                }
                UpdateCurrentLexeme();
                return new LexemeData(currentIndexLine, currentIndexSymbol, currentLexemeClass, currentLexeme, currentLexeme);
            }

            //NUMBERS
            if (IsRequired—lass(symbolClass, ClassLexeme.number))
            {
                SaveIndexes();
                int dotCount = 0;
                int eCount = 0;

                while (IsRequired—lass(symbolClass, ClassLexeme.standart, ClassLexeme.number))
                {
                    currentLexeme += symbol;
                    currentLexemeClass = LexemeVerification.GetClass(currentLexeme);

                    SavePreviousSymbol();

                    if (VerifyNextSymbolAndCheckOnNull())
                    {
                        UpdateCurrentLexeme();
                        return new LexemeData(currentIndexLine, currentIndexSymbol, currentLexemeClass, currentLexemeValue, currentLexeme);
                    }

                    if (IsRequired—lass(symbolClass, ClassLexeme.standart))
                    {
                        currentLexeme += symbol;
                        if (symbol.Equals("e") & dotCount == 0)
                        {
                            eCount++;

                            if (VerifyNextSymbolAndCheckOnNull())
                            {
                                SaveErrorForLexeme("incorrect variable format");
                                return lexemeData;
                            }

                            if (!IsRequired—lass(symbolClass, ClassLexeme.number) & !(symbol.Equals("+") | symbol.Equals("-")) | dotCount > 0)
                            {
                                currentLexeme += symbol;
                                SaveErrorForLexeme("incorrect variable format");
                                return lexemeData;
                            }
                            
                            if (symbol.Equals("+") | symbol.Equals("-"))
                            {
                                currentLexeme += symbol;

                                if (VerifyNextSymbolAndCheckOnNull())
                                {
                                    SaveErrorForLexeme("incorrect variable format");
                                    return lexemeData;
                                }
                                else
                                {
                                    if (!IsRequired—lass(symbolClass, ClassLexeme.number))
                                    {
                                        currentLexeme += symbol;
                                        SaveErrorForLexeme("incorrect variable format");
                                        return lexemeData;
                                    }
                                }
                            }
                        }
                        else
                        {
                            SaveErrorForLexeme("incorrect variable format");
                            return lexemeData;
                        }
                    }

                    if (symbol.Equals("."))
                    {
                        dotCount++;
                        currentLexeme += symbol;

                        if (dotCount > 1)
                        {
                            SaveErrorForLexeme("incorrect variable format");
                            return lexemeData;
                        }

                        if (eCount != 0)
                        {
                            currentLexeme += symbol;
                            SaveErrorForLexeme("incorrect variable format");
                            return lexemeData;
                        }

                        SavePreviousSymbol();

                        if (VerifyNextSymbolAndCheckOnNull())
                            return lexemeData;

                        if (!IsRequired—lass(symbolClass, ClassLexeme.number))
                        {
                            SaveErrorForLexeme("incorrect variable format");
                            return lexemeData;
                        }
                    }
                }

                if (currentLexeme != null)
                {
                    if (previousSymbol.Equals("."))
                    {
                        SaveErrorForLexeme("incorrect variable format");
                        return lexemeData;
                    }
                    if (IsRequired—lass(currentLexemeClass, ClassLexeme.separator, ClassLexeme.control, ClassLexeme.NONAME, ClassLexeme.standart, ClassLexeme.number))
                    {
                        UpdateCurrentLexeme();
                        return new LexemeData(currentIndexLine, currentIndexSymbol, currentLexemeClass, currentLexemeValue, currentLexeme);
                    }
                }
            }

            //SEPARATORS
            if (IsRequired—lass(symbolClass, ClassLexeme.separator))
            {
                SaveIndexes();
                currentLexeme = symbol;
                UpdateCurrentLexeme();

                if (VerifyNextSymbolAndCheckOnNull())
                    return new LexemeData(currentIndexLine, currentIndexSymbol, currentLexemeClass, currentLexemeValue, currentLexeme);

                if (IsRequired—lass(symbolClass, ClassLexeme.standart, ClassLexeme.number))
                {
                    if (currentLexeme.Equals("."))
                    {
                        currentLexeme += symbol;
                        SaveErrorForLexeme("incorrect variable format");
                    }
                }
                return new LexemeData(currentIndexLine, currentIndexSymbol, currentLexemeClass, currentLexeme, currentLexeme);
            }

            return lexemeData;
        }

        private void UpdateCurrentLexeme()
        {
            currentLexemeClass = LexemeVerification.GetClass(currentLexeme);
            if (IsRequired—lass(currentLexemeClass, ClassLexeme.NONAME, ClassLexeme.standart, ClassLexeme.number))
                currentLexemeClass = IdentifyClass();
            currentLexemeValue = SetValue();
        }

        private ClassLexeme IdentifyClass()
        {
            if (IsInt(currentLexeme))
                return ClassLexeme.integer;
            else if (IsFloat(currentLexeme))
                return ClassLexeme.real;
            return ClassLexeme.variable;
        }

        private string SetValue()
        {
            if (IsRequired—lass(currentLexemeClass, ClassLexeme.real))
                return double.Parse(currentLexeme.Replace('.', ',')).ToString();
            return currentLexeme;
        }

        private bool VerifyNextSymbolAndCheckOnNull()
        {
            symbol = fileReader.ReadNextSymbolAndChangeIndexes();
            if (symbol == null)
                return true;
            symbolClass = LexemeVerification.GetClass(symbol);
            return false;
        }

        public List<LexemeData> GetExistingListLexemesData()
        {
            return lexemesData;
        }

        public void LexemesDataOutputInConsole()
        {
            Console.WriteLine("Number of lexemes: " + lexemesData.Count);
            for (int i = 0; i < lexemesData.Count; i++)
                Console.WriteLine("" + lexemesData[i].indexLine + '\t' + lexemesData[i].indexSymbol + '\t' + lexemesData[i].classLexeme + '\t' + "\"" + lexemesData[i].value + "\"" + '\t' + lexemesData[i].code);
            if (!errorData.IsProblemIsNull())
                Console.WriteLine(errorData.GetLineIndex() + "\t" + errorData.GetSymbolIndex() + "\t" + errorData.GetErrorText());
        }

        public void LexemesDataOutputInFile()
        {
            using (StreamWriter streamWriter = new StreamWriter(filePath.Remove(filePath.Length - 4, 4) + "_output.txt"))
            {
                for (int i = 0; i < lexemesData.Count; i++)
                    streamWriter.WriteLine("" + lexemesData[i].indexLine + '\t' + lexemesData[i].indexSymbol + '\t' + lexemesData[i].classLexeme + '\t' + "\"" + lexemesData[i].value + "\"" + '\t' + lexemesData[i].code);
                if (!errorData.IsProblemIsNull())
                    streamWriter.WriteLine(errorData.GetLineIndex() + "\t" + errorData.GetSymbolIndex() + "\t" + errorData.GetErrorText());
            }
        }

        private void SaveIndexes()
        {
            currentIndexLine = fileReader.GetIndexLine();
            currentIndexSymbol = fileReader.GetIndexSymbol();
        }

        private void SavePreviousSymbol()
        {
            previousSymbol = symbol;
            previousSymbolClass = symbolClass;
        }

        private bool IsTwoSymbolOperation()
        {
            if (currentLexeme.Length < 2)
                return false;
            return IsRequired—lass(currentLexemeClass, ClassLexeme.operation);
        }

        private void SkipStringCommentary()
        {
            int indexCurrentLine = fileReader.GetIndexLine();
            while (indexCurrentLine == fileReader.GetIndexLine())
                if (VerifyNextSymbolAndCheckOnNull())
                    break;
        }

        private void SkipBlockCommentary()
        {
            while (!symbol.Equals("}"))
            {
                if (VerifyNextSymbolAndCheckOnNull())
                    break;
            }
            VerifyNextSymbolAndCheckOnNull();
        }

        private void SkipSpaceAndControl()
        {
            while (symbol.Equals(" ") | IsRequired—lass(symbolClass, ClassLexeme.control))
                if (VerifyNextSymbolAndCheckOnNull())
                    break;
        }

        private bool IsInt(string lexemeCode)
        {
            for (int i = 0; i < lexemeCode.Length; i++)
                if (!(lexemeCode[i] >= '0' & lexemeCode[i] <= '9'))
                    return false;
            return true;
        }

        private bool IsFloat(string lexemeCode)
        {
            if (float.TryParse(currentLexeme.Replace('.', ','), out Single result))
                return true;

            return false;
        }

        private void SaveErrorForLexeme(string errorMessage = "")
        {
            errorData = new ErrorData(currentIndexLine, currentIndexSymbol, "error: " + errorMessage + " (" + currentLexeme + ")");
        }

        private void SaveErrorForSymbol(string errorMessage = "")
        {
            errorData = new ErrorData(fileReader.GetIndexLine(), fileReader.GetIndexSymbol(), "error: " + errorMessage + " (" + symbol + ")");
        }

        public ErrorData GetError()
        {
            return errorData;
        }

        ///<summary> œÓ‚ÂˇÂÚ ‡‚ÂÌ ÎË ÍÎ‡ÒÒ Ó‰ÌÓÏÛ ËÁ ÛÍ‡Á‡ÌÌ˚ı. </summary>
        private bool IsRequired—lass(ClassLexeme lexemeClass, params ClassLexeme[] classes)
        {
            for (int i = 0; i < classes.Length; i++)
                if (lexemeClass == classes[i])
                    return true;
            return false;
        }
    }
}