using System;
using System.Collections.Generic;
using UnityEngine;

namespace Peevo.Samwise.Unity
{
    public class SamwiseAsset : ScriptableObject, ISerializationCallbackReceiver
    {
        public IReadOnlyList<Dialogue> Dialogues => dialogues;
        public string Text => serializedText;

#if UNITY_EDITOR
        public SamwiseAssetStatistics Statistics;
#endif

        public void Parse(string text)
        {
            var externalType = Utils.FindTypeWithAttribute<SamwiseExternalParserAttribute>();

            IExternalCodeParser externalCodeParser = null;

            if (externalType == null)
                externalCodeParser = new DummyCodeParser();
            else
                externalCodeParser = (IExternalCodeParser)Activator.CreateInstance(externalType);

            var parser = new SamwiseParser(externalCodeParser);

            dialogues.Clear();
            if (!string.IsNullOrEmpty(text) && !parser.Parse(dialogues, text))
            {
                Debug.LogError("Cannot compile Samwise asset:");
                foreach (var e in parser.Errors)
                {
                    Debug.LogError(e.Error + "\n" + "---- (" + e.Line + ") " + GetLine(text, e.Line));
                }
            }
            else
                serializedText = text;
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            if (!string.IsNullOrEmpty(serializedText))
                Parse(serializedText);
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
#if UNITY_EDITOR
            if (UnityEditor.EditorUtility.IsDirty(this))
            { 
                serializedText = "";
                foreach (var dialogue in dialogues)
                    serializedText += dialogue.ToString();
            }
#endif
        }

        string GetLine(string text, int lineNo)
        {
            int lastStart = 0;

            for (int i=0; i< lineNo - 1; ++i)
            {
                lastStart = text.IndexOf("\n", lastStart) + 1;

                if (lastStart == 0)
                    return "";
            }

            var endPos = text.IndexOf("\n", lastStart);

            if (endPos < 0)
            {
                return text.Substring(lastStart);
            }
            else
                return text.Substring(lastStart, endPos - lastStart);
        }

        [SerializeField] string serializedText;
        List<Dialogue> dialogues = new List<Dialogue>();
    }
}