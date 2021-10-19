namespace Programming_Compilers_Pascal
{
    public static class LexemeVerification
    {
        private static string[] keywords = { "write", "writeln", "read", "readln", "if", "while", "then", "else", "to", "downto", "do", "program", "var", "begin", "end", "random", "randomize" };
        private static string[] types = { "integer", "real", "string", "char", "boolean", "double", "single", "decimal", "shortint", "smallint", "longint", "int64", "byte", "word", "longword", "cardinal", "uint64", "biginteger" };
        private static string[] operations = { "+", "-", "*", "/", "=", ":=", "<", ">", ">=", "<=", "<>", "div", "mod", "abs", "cos", "sin", "exp", "ln", "sqr", "sqrt", "@", "not", "^", "and", "shl", "shr", "or", "xor", "as", "is", "in", "?:" };
        private static string[] separators = { " ", ";", "(", ")", "\'", "\\", "\"", "[", "]", "{", "}", ",", "." };

        //private static string[] numbers = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
        private static string[] standarts = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "#", "¹", "_", "&", "|", "!", "%", "?" };

        private static string[] controls = { "\n", "\t", "\v", "\r", "\b", "\f" };

        private static ClassLexeme[] classLexemes = { ClassLexeme.control, ClassLexeme.keyword, ClassLexeme.type, ClassLexeme.operation, ClassLexeme.separator, ClassLexeme.standart };

        private static string[] Interpreter(ClassLexeme classLexeme)
        {
            if ((classLexeme == ClassLexeme.control))
                return controls;
            else if (classLexeme == ClassLexeme.keyword)
                return keywords;
            else if (classLexeme == ClassLexeme.type)
                return types;
            else if (classLexeme == ClassLexeme.operation)
                return operations;
            else if (classLexeme == ClassLexeme.separator)
                return separators;
            else
                return standarts;
        }

        public static ClassLexeme GetClass(string lexeme)
        {
            foreach (ClassLexeme classLexeme in classLexemes)
            {
                string[] lexemes = Interpreter(classLexeme);
                for (int i = 0; i < lexemes.Length; i++)
                {
                    if (lexeme.ToLower().Equals(lexemes[i]))
                    {
                        if (classLexeme == ClassLexeme.control)
                            return ClassLexeme.control;
                        else if (classLexeme == ClassLexeme.keyword)
                            return ClassLexeme.keyword;
                        else if (classLexeme == ClassLexeme.type)
                            return ClassLexeme.type;
                        else if (classLexeme == ClassLexeme.operation)
                            return ClassLexeme.operation;
                        else if (classLexeme == ClassLexeme.separator)
                            return ClassLexeme.separator;
                        else if (classLexeme == ClassLexeme.standart)
                            return ClassLexeme.standart;
                    }
                }
            }
            return ClassLexeme.NONAME;
        }
    }
}
