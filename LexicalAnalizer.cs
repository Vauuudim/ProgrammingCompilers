using System;
using System.Collections.Generic;
using System.IO;

namespace Programming_Compilers_Pascal
{
    public class LexicalAnalizer
    {
        private List<LexemeData> lexemesData = new List<LexemeData>();
        private string filePath = "";

        private string currentLexeme = null;
        private int currentIndexLine = 0;
        private int currentIndexInLine = 0;
        private ClassLexeme currentLexemeClass;

        private bool isNew = true;

        public int errorLineIndex = 0;
        public int errorSymbolIndex = 0;
        public string errorLexeme = null;
        public string errorText = null;

        public LexicalAnalizer (string filePath)
        {
            this.filePath = filePath;
        }

        public List<LexemeData> Ànalysis()
        {
            using (StreamReader streamReader = new StreamReader(filePath))
            {
                FileReader fileReader = new FileReader(streamReader);

                while (fileReader.isEnd | errorLexeme == null)
                {
                    string symbol = fileReader.ReadSymbol();

                    if (symbol == null)
                        break;

                    ClassLexeme symbolClass = LexemeVerification.GetClass(symbol);

                    if (symbolClass != ClassLexeme.control)
                    {
                        if (symbolClass == ClassLexeme.NONAME)
                        {
                            SaveError(fileReader.indexLine, fileReader.indexSymbol, symbol, "found unknown symbol");
                            lexemesData.Add(new LexemeData(fileReader.indexLine, fileReader.indexSymbol, symbolClass, symbol, symbol));
                            break;
                        }

                        if (symbolClass == ClassLexeme.standart)
                        {
                            if (isNew)
                            {
                                currentIndexLine = fileReader.indexLine;
                                currentIndexInLine = fileReader.indexSymbol;
                            }
                            currentLexeme += symbol;
                            isNew = false;
                        }
                        else
                        {
                            if (isNew == false)
                            {
                                if (currentLexemeClass == ClassLexeme.NONAME | currentLexemeClass == ClassLexeme.standart)
                                    lexemesData.Add(new LexemeData(currentIndexLine, currentIndexInLine, ClassLexeme.variable, currentLexeme, currentLexeme));
                                else
                                    lexemesData.Add(new LexemeData(currentIndexLine, currentIndexInLine, currentLexemeClass, currentLexeme, currentLexeme));
                                isNew = true;
                                currentLexeme = null;
                            }
                        }

                        if (symbolClass == ClassLexeme.keyword | symbolClass == ClassLexeme.operation | symbolClass == ClassLexeme.type | symbolClass == ClassLexeme.separator)
                        {
                            if (symbolClass == ClassLexeme.standart)
                                symbolClass = ClassLexeme.variable;
                            lexemesData.Add(new LexemeData(fileReader.indexLine, fileReader.indexSymbol, symbolClass, symbol, symbol));

                            if (currentLexeme != null & !isNew)
                            {
                                if (currentLexemeClass == ClassLexeme.NONAME | currentLexemeClass == ClassLexeme.standart)
                                    lexemesData.Add(new LexemeData(currentIndexLine, currentIndexInLine, ClassLexeme.variable, currentLexeme, currentLexeme));
                                else
                                    lexemesData.Add(new LexemeData(currentIndexLine, currentIndexInLine, currentLexemeClass, currentLexeme, currentLexeme));
                            }
                            isNew = true;
                            currentLexeme = null;
                        }
                    }
                    else
                    {
                        if (!isNew)
                        {
                            if (currentLexemeClass == ClassLexeme.NONAME | currentLexemeClass == ClassLexeme.standart)
                                lexemesData.Add(new LexemeData(currentIndexLine, currentIndexInLine, ClassLexeme.variable, currentLexeme, currentLexeme));
                            else
                                lexemesData.Add(new LexemeData(currentIndexLine, currentIndexInLine, currentLexemeClass, currentLexeme, currentLexeme));
                            isNew = true;
                            currentLexeme = null;
                        }
                    }

                    if (currentLexeme != null)
                    {
                        currentLexemeClass = LexemeVerification.GetClass(currentLexeme);
                    }
                }

                if (currentLexeme != null & !isNew)
                {
                    if (currentLexemeClass == ClassLexeme.NONAME | currentLexemeClass == ClassLexeme.standart)
                        lexemesData.Add(new LexemeData(currentIndexLine, currentIndexInLine, ClassLexeme.variable, currentLexeme, currentLexeme));
                    else
                        lexemesData.Add(new LexemeData(currentIndexLine, currentIndexInLine, currentLexemeClass, currentLexeme, currentLexeme));
                    isNew = true;
                    currentLexeme = null;
                }
            }

            MakeEdits();
            return lexemesData;
        }

        private void MakeEdits()
        {
            for (int i = 0; i < lexemesData.Count; i++)
            {
                if (lexemesData[i].code.Equals("\'"))
                {
                    if (i < lexemesData.Count - 1)
                    {
                        string nextLexemeCode = lexemesData[i + 1].code;
                        while (!nextLexemeCode.Equals("\'"))
                        {
                            int nextLexemeIndexLine = lexemesData[i + 1].indexLine;

                            if (lexemesData[i].indexLine != nextLexemeIndexLine)
                            {
                                SaveError(lexemesData[i], "closing symbol not found");
                                return;
                            }

                            lexemesData[i].code += nextLexemeCode;
                            lexemesData[i].value += nextLexemeCode;
                            lexemesData.Remove(lexemesData[i + 1]);
                            nextLexemeCode = null;

                            if (i == lexemesData.Count - 1)
                            {
                                SaveError(lexemesData[i], "closing symbol not found");
                                break;
                            }

                            nextLexemeCode = lexemesData[i + 1].code;
                        }

                        if (nextLexemeCode != null)
                        {
                            lexemesData[i].code += nextLexemeCode;
                            lexemesData[i].value += nextLexemeCode;
                            lexemesData.Remove(lexemesData[i + 1]);
                            lexemesData[i].classLexeme = ClassLexeme.@string;
                        }
                    }
                    else
                    {
                        SaveError(lexemesData[i], "closing symbol not found");
                    }
                }

                if (lexemesData[i].classLexeme == ClassLexeme.variable)
                {
                    if (IsInt(lexemesData[i].code))
                        lexemesData[i].classLexeme = ClassLexeme.integer;
                }
            }

            for (int i = 0; i < lexemesData.Count; i++)
            {
                if (i < lexemesData.Count - 3)
                {
                    if ((lexemesData[i].classLexeme == ClassLexeme.variable | lexemesData[i].classLexeme == ClassLexeme.integer | lexemesData[i].classLexeme == ClassLexeme.real | lexemesData[i].classLexeme == ClassLexeme.@string) & lexemesData[i + 1].code.Equals(".") & (lexemesData[i].classLexeme == ClassLexeme.variable | lexemesData[i].classLexeme == ClassLexeme.integer | lexemesData[i].classLexeme == ClassLexeme.real | lexemesData[i].classLexeme == ClassLexeme.@string) & lexemesData[i + 3].code.Equals("."))
                    {
                        SaveError(lexemesData[i + 3], "incorrect variable format");
                    }
                }

                if (i < lexemesData.Count - 2)
                {
                    if (lexemesData[i].classLexeme == ClassLexeme.integer & lexemesData[i + 1].code.Equals(".") & lexemesData[i + 2].classLexeme == ClassLexeme.integer)
                    {
                        if (lexemesData[i].indexLine == lexemesData[i + 1].indexLine & lexemesData[i].indexLine == lexemesData[i + 2].indexLine)
                        {
                            lexemesData[i].value = double.Parse(lexemesData[i].code + "," + lexemesData[i + 2].code).ToString();
                            lexemesData[i].code += "." + lexemesData[i + 2].code;
                            lexemesData[i].classLexeme = ClassLexeme.real;

                            lexemesData.Remove(lexemesData[i + 2]);
                            lexemesData.Remove(lexemesData[i + 1]);
                        }
                        else
                        {
                            SaveError(lexemesData[i + 2], "incorrect variable format");
                        }

                    }
                }
            }
        }

        private bool IsInt(string lexeme)
        {
            for (int i = 0; i < lexeme.Length; i++)
            {
                if (!(lexeme[i] >= '0' & lexeme[i] <= '9'))
                    return false;
            }
            return true;
        }

        private void SaveError(LexemeData lexemeData, string errorMessage = "")
        {
            errorLineIndex = lexemeData.indexLine;
            errorSymbolIndex = lexemeData.indexInLine;
            errorLexeme = lexemeData.code;
            errorText = "error: " + errorMessage;
        }

        private void SaveError(int indexLine, int indexSymbol, string code, string errorMessage = "")
        {
            errorLineIndex = indexLine;
            errorSymbolIndex = indexSymbol;
            errorLexeme = code;
            errorText = "error: " + errorMessage;
        }
    }
}