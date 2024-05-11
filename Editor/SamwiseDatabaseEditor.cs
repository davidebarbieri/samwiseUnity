using UnityEditor;
using UnityEngine;

namespace Peevo.Samwise.Unity
{
    [CustomEditor(typeof(SamwiseDatabase))]
    public class SamwiseDatabaseEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var asset = this.target as SamwiseDatabase;

            var count = (asset.Assets != null ? asset.Assets.Count : 0);

            if (count == 0)
                EditorGUILayout.LabelField("No dialogues found.");
            else
            {
                int totalNodeCount = 0;
                int totalWordCount = 0;

                for (int i = 0; i < count; ++i)
                {
                    var file = asset.Assets[i];

                    if (file != null)
                    {
                        file.Statistics.GetTotalInfo(out int nodeCount, out int wordCount);
                        totalNodeCount += nodeCount;
                        totalWordCount += wordCount;
                    }
                }

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Statistics", EditorStyles.boldLabel, GUILayout.ExpandWidth(false));
                EditorGUILayout.LabelField("[" + totalNodeCount + " node(s), " + totalWordCount + " word(s)]", GUILayout.ExpandWidth(true));
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space();

                showStats = EditorGUILayout.Foldout(showStats, "Assets Statistics:");
            }

            if (showStats)
            { 
                for (int i = 0; i < count; ++i)
                {
                    var file = asset.Assets[i];

                    if (file != null)
                    { 
                        file.Statistics.GetTotalInfo(out int nodeCount, out int wordCount);
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("- " + file.name, EditorStyles.boldLabel, GUILayout.ExpandWidth(false));
                        EditorGUILayout.LabelField("[" + nodeCount + " node(s), " + wordCount + " word(s)]", GUILayout.ExpandWidth(true));
                        EditorGUILayout.EndHorizontal();
                    }
                }
            }

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            base.OnInspectorGUI();
        }

        bool showStats;
    }
}