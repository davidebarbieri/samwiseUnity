using UnityEngine;
using UnityEditor.AssetImporters;
using System.IO;
using UnityEditor;

namespace Peevo.Samwise.Unity
{
    [ScriptedImporter(1, "sam")]
    public class SamwiseImporter : ScriptedImporter
    {
        public static bool ToInvalidateDrawers;

        public class SamwiseImporterProcessor : AssetPostprocessor
        {
            static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths, bool didDomainReload)
            {
                foreach (string p in deletedAssets)
                {
                    if (Path.GetExtension(p) == ".sam")
                    {
                        var database = LookUpDatabase(p);

                        // Prune all non valid assets
                        if (database != null)
                        {
                            var assets = database.Assets;

                            if (assets.RemoveAll((a) => a == null) > 0)
                            {
                                EditorUtility.SetDirty(database);
                                Debug.Log("Deleting no more valid asset from database");
                            }
                        }
                    }
                }

                foreach (string p in importedAssets)
                {
                    if (Path.GetExtension(p) == ".sam")
                    {
                        var database = LookUpDatabase(p);

                        if (database == null)
                        {
                            Debug.LogError("Warning, no asset database found for the asset " + p);
                            continue;
                        }

                        var assets = database.Assets;

                        var asset = AssetDatabase.LoadAssetAtPath<SamwiseAsset>(p);
                        if (!assets.Contains(asset))
                        {
                            assets.Add(asset);
                            EditorUtility.SetDirty(database);
                        }
                    }
                }
            }
        }

        public override void OnImportAsset(AssetImportContext ctx)
        {
            var assetPath = Path.GetDirectoryName(ctx.assetPath);
            //var root = Directory.GetDirectoryRoot(assetPath);

            SamwiseAsset asset = ScriptableObject.CreateInstance<SamwiseAsset>();

            var text = File.ReadAllText(ctx.assetPath, System.Text.Encoding.UTF8);
            asset.Parse(text);
            AddStatistics(asset);

            ctx.AddObjectToAsset("dialogues", asset);
            ctx.SetMainObject(asset);

            ToInvalidateDrawers = true;
        }

        void AddStatistics(SamwiseAsset asset)
        {
            asset.Statistics = new SamwiseAssetStatistics();
            foreach (var dialogue in asset.Dialogues)
            {
                GetInfo(dialogue, out var nodeCount, out var wordCount);
                asset.Statistics.Stats.Add(
                    new SamwiseAssetStatistics.DialogueStatistics
                    {
                        Label = dialogue.Label,
                        Nodes = nodeCount,
                        Words = wordCount
                    });
            }
        }

        void GetInfo(IDialogueBlock dialogue, out int nodeCount, out int wordCount)
        {
            nodeCount = 0;
            wordCount = 0;

            foreach (var node in dialogue.Traverse())
            {
                ++nodeCount;

                if (node is ITextContent textNode)
                    wordCount += CountWords(textNode.Text);

                if (node is IChoosableNode)
                {
                    var choosableNode = (IChoosableNode)node;

                    for (int i = 0, count = choosableNode.OptionsCount; i < count; ++i)
                    {
                        var option = choosableNode.GetOption(i);
                        wordCount += CountWords(option.Text);
                    }
                }
            }
        }

        int CountWords(string text)
        {
            int wordCount = 0, i = 0;

            TokenUtils.SkipWhitespaces(text, ref i);

            while (i < text.Length)
            {
                while (i < text.Length && text[i] != ' ')
                    i++;

                wordCount++;

                TokenUtils.SkipWhitespaces(text, ref i);
            }

            return wordCount;
        }
        static SamwiseDatabase LookUpDatabase(string assetPath)
        {
            var projectPath = Directory.GetParent(Application.dataPath);
            while (!string.IsNullOrEmpty(assetPath))
            {
                string[] guids = AssetDatabase.FindAssets("t:SamwiseDatabase", new[] { assetPath });

                if (guids != null && guids.Length > 0)
                {
                    var path = AssetDatabase.GUIDToAssetPath(guids[0]);
                    return AssetDatabase.LoadAssetAtPath<SamwiseDatabase>(path);

                }

                var parent = Directory.GetParent(assetPath);

                if (parent != null)
                {
                    assetPath = parent.FullName.Replace(projectPath.FullName, "");

                    if (string.IsNullOrEmpty(assetPath))
                        return null;

                    if (assetPath[0] == '/' || assetPath[0] == '\\')
                        assetPath = assetPath.Substring(1);
                }
                else
                    return null;

            }

            return null;
        }
    }
}