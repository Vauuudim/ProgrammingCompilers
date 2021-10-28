namespace Programming_Compilers_Pascal
{
    public class LexemeData
    {
        public int indexLine;
        public int indexSymbol;
        public ClassLexeme classLexeme = ClassLexeme.standart;
        public string value;
        public string code;

        public LexemeData(int indexLine = 0, int indexInLine = 0, ClassLexeme classLexeme = ClassLexeme.standart, string value = "", string code = "")
        {
            this.indexLine = indexLine;
            this.indexSymbol = indexInLine;
            this.classLexeme = classLexeme;
            this.value = value;
            this.code = code;
        }

        public void SetClass(ClassLexeme newClass)
        {
            classLexeme = newClass;
        }
    }
}