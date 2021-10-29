using System.Collections.Generic;

namespace Programming_Compilers_Pascal
{
    public static class LexemeVerification
    {
        private static string[] keywords = { "write", "writeln", "read", "readln", "if", "while", "then", "else", "to", "downto", "do", "program", "var", "begin", "end", "random", "randomize" };
        private static string[] types = { "integer", "real", "string", "char", "boolean", "double", "single", "decimal", "shortint", "smallint", "longint", "int64", "byte", "word", "longword", "cardinal", "uint64", "biginteger" };
        private static string[] operations = { "+", "-", "*", "/", "=", ":", ":=", "<", ">", ">=", "<=", "<>", "div", "mod", "abs", "cos", "sin", "exp", "ln", "sqr", "sqrt", "@", "not", "^", "and", "shl", "shr", "or", "xor", "as", "is", "in", "?:" };
        private static string[] separators = { " ", ";", "(", ")", "\'", "\\", "\"", "[", "]", "{", "}", ",", "." };
        private static string[] standarts = { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "#", "¹", "_", "&", "|", "!", "%", "?" };
        private static string[] numbers = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
        private static string[] controls = { "\n", "\t", "\v", "\r", "\b", "\f" };

        private static Dictionary<ClassLexeme, string[]> massives = new Dictionary<ClassLexeme, string[]>(7)
        {
            { ClassLexeme.control, controls },
            { ClassLexeme.keyword, keywords },
            { ClassLexeme.type, types },
            { ClassLexeme.operation, operations },
            { ClassLexeme.separator, separators },
            { ClassLexeme.standart, standarts },
            { ClassLexeme.number, numbers }
        };

        public static ClassLexeme GetClass(string lexeme)
        {
            if (lexeme != null)
            {
                foreach (KeyValuePair<ClassLexeme, string[]> element in massives)
                {
                    for (int i = 0; i < element.Value.Length; i++)
                    {
                        if (lexeme.ToLower().Equals(element.Value[i]))
                        {
                            return element.Key;
                        }
                    }
                }
            }
            return ClassLexeme.NONAME;
        }
    }
}
