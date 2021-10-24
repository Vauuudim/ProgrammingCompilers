namespace Programming_Compilers_Pascal
{
    public class ErrorData
    {
        private int errorLineIndex;
        private int errorSymbolIndex;
        private string errorText;

        public ErrorData (int errorLineIndex = 0, int errorSymbolIndex = 0, string errorText = null)
        {
            this.errorLineIndex = errorLineIndex;
            this.errorSymbolIndex = errorSymbolIndex;
            this.errorText = errorText;
        }

        public int GetLineIndex()
        {
            return errorLineIndex;
        }

        public int GetSymbolIndex()
        {
            return errorSymbolIndex;
        }

        public string GetErrorText()
        {
            return errorText;
        }

        public bool IsProblemIsNull()
        {
            return errorText == null ? true : false;
        }
    }
}