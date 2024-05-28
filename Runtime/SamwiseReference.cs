namespace Peevo.Samwise.Unity
{
    [System.Serializable]
    public struct SamwiseReference
    {
        public string Dialogue;
        public string Label;

        public override string ToString()
        {
            if (string.IsNullOrEmpty(Label))
                return Dialogue;
            return Dialogue + "." + Label;
        }

        public static implicit operator SamwiseReference(string stringValue)
        {
            if (stringValue.Contains("."))
            {
                var idx = stringValue.IndexOf(".");
                return new SamwiseReference() { Dialogue = stringValue.Substring(0, idx), Label = stringValue.Substring(idx + 1) };
            }
            return new SamwiseReference() { Dialogue = stringValue };
        }

        public static implicit operator string(SamwiseReference structValue)
        {
            if (string.IsNullOrEmpty(structValue.Label))
                return structValue.Dialogue;
            return structValue.Dialogue + "." + structValue.Label;
        }
    }
}