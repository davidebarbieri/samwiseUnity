using System;
using System.Collections.Generic;

#if UNITY_EDITOR
namespace Peevo.Samwise.Unity
{
    [Serializable]
    public class SamwiseAssetStatistics
    {
        public List<DialogueStatistics> Stats = new List<DialogueStatistics>();

        public void GetInfo(Dialogue dialogue, out int nodeCount, out int wordCount)
        {
            nodeCount = 0;
            wordCount = 0;

            foreach (var s in Stats)
            {
                if (s.Label == dialogue.Label)
                {
                    nodeCount = s.Nodes;
                    wordCount = s.Words;
                    break;
                }
            }
        }
        public void GetTotalInfo(out int nodeCount, out int wordCount)
        {
            nodeCount = 0;
            wordCount = 0;

            foreach (var s in Stats)
            {
                nodeCount += s.Nodes;
                wordCount += s.Words;
            }
        }

        [Serializable]
        public class DialogueStatistics
        {
            public string Label;
            public int Nodes;
            public int Words;
        }
    }
}
#endif