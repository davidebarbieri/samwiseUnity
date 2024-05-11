using UnityEditor;
using UnityEngine;

namespace Peevo.Samwise.Unity
{
    [CustomEditor(typeof(SamwiseAsset))]
    public class SamwiseAssetEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var asset = this.target as SamwiseAsset;

            var count = (asset.Dialogues != null ? asset.Dialogues.Count : 0);

            if (count == 0)
                EditorGUILayout.LabelField("No dialogues found.");
            else
            {
                asset.Statistics.GetTotalInfo(out int nodeCount, out int wordCount);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Statistics", EditorStyles.boldLabel, GUILayout.ExpandWidth(false));
                EditorGUILayout.LabelField("[" + nodeCount + " node(s), " + wordCount + " word(s)]", GUILayout.ExpandWidth(true));
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.LabelField("Dialogues:");
            }

            for (int i = 0; i < count; ++i)
            {
                var dialogue = asset.Dialogues[i];

                asset.Statistics.GetInfo(dialogue, out int nodeCount, out int wordCount);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("- " + "(" + dialogue.Label + ") " + dialogue.Title, EditorStyles.boldLabel, GUILayout.ExpandWidth(false));
                EditorGUILayout.LabelField("[" + nodeCount + " node(s), " + wordCount + " word(s)]", GUILayout.ExpandWidth(true));
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("File content");
            EditorGUILayout.TextArea(asset.Text);
        }
    }
}