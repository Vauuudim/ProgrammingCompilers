using System.IO;

namespace Programming_Compilers_Pascal
{
    public class FileReader
    {
        public int indexSymbol = 0;
        public int indexLine = 1;
        public bool isEnd = true;

        private bool isNewLine = false;
        private StreamReader streamReader;

        public FileReader(StreamReader streamReader)
        {
            this.streamReader = streamReader;
        }

        public string ReadSymbol()
        {
            if (isNewLine)
            {
                indexLine++;
                indexSymbol = 0;
                isNewLine = false;
            }
                
            indexSymbol++;

            int charIndex = streamReader.Read();
            if (charIndex == -1)
            {
                isEnd = false;
                return null;
            }

            string symbol = "" + (char)charIndex;

            if (((char)charIndex).Equals('\n'))
                isNewLine = true;

            return symbol;
        }
    }
}