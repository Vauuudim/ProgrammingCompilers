namespace Programming_Compilers_Pascal
{
    public class ErrorData
    {
        public int errorLineIndex;
        public int errorSymbolIndex;
        public string errorText;

        public ErrorData (int errorLineIndex = 0, int errorSymbolIndex = 0, string errorText = null)
        {
            this.errorLineIndex = errorLineIndex;
            this.errorSymbolIndex = errorSymbolIndex;
            this.errorText = errorText;
        }

        public ErrorData GetError()
        {
            return new ErrorData(errorLineIndex, errorSymbolIndex, errorText);
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