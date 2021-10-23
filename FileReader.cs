using System.IO;

namespace Programming_Compilers_Pascal
{
    public class FileReader
    {
        private int indexSymbol = 0;
        private int indexLine = 1;

        private bool isNewLine = false;
        private StreamReader streamReader;

        public FileReader(StreamReader streamReader)
        {
            this.streamReader = streamReader;
        }

        public string ReadNextSymbolAndChangeIndexes()
        {
            string symbol = null;
            int charIndex = streamReader.Read();

            if (charIndex != -1)
            {
                ChangeIndexLineAndSymbol();
                symbol = "" + (char)charIndex;
                isNewLine = symbol.Equals("\n") ? true : false;
            }

            return symbol;
        }

        private void ChangeIndexLineAndSymbol()
        {
            indexSymbol++;
            if (isNewLine)
            {
                indexLine++;
                indexSymbol = 1;
                isNewLine = false;
            }
        }

        public int GetIndexSymbol()
        {
            return indexSymbol;
        }

        public int GetIndexLine()
        {
            return indexLine;
        }
    }
}