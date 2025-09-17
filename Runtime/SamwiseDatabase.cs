using System;
using System.Collections.Generic;
using UnityEngine;

namespace Peevo.Samwise.Unity
{
    public class SamwiseDatabaseCache : MonoBehaviour, IDialogueSet
    {
        public bool GetDialogue(string dialogueSymbol, out Dialogue dialogue)
        {
            return dialogues.TryGetValue(dialogueSymbol, out dialogue);
        }

        public IDialogueNode GetNodeFromLabel(string dialogueSymbol, string label)
        {
            if (dialogues.TryGetValue(dialogueSymbol, out var dialogue))
            {
                return dialogue.GetNodeFromLabel(label);
            }
            return null;
        }

        public void Initialize(SamwiseDatabase db)
        {
            if (initialized)
                return;

            initialized = true;

            if (db.Assets == null)
                return;

            dialoguesSet.Clear();
            dialogues.Clear();

            foreach (var asset in db.Assets)
            {
                foreach (var dialogue in asset.Dialogues)
                {
                    if (dialogue == null)
                        continue;

                    if (dialogues.ContainsKey(dialogue.Label))
                    {
                        Debug.LogError("Duplicate dialogue " + dialogue.Label);
                    }
                    else
                    {
                        dialogues[dialogue.Label] = dialogue;
                        dialoguesSet.Add(dialogue);
                    }
                }
            }
        }

        public bool Contains(Dialogue dialogue)
        {
            return dialoguesSet.Contains(dialogue);
        }

        [NonSerialized] bool initialized = false;
        [NonSerialized] Dictionary<string, Dialogue> dialogues = new Dictionary<string, Dialogue>();
        [NonSerialized] HashSet<Dialogue> dialoguesSet = new HashSet<Dialogue>();

    }

    [CreateAssetMenu(fileName = "SamwiseDatabase", menuName = "Samwise/SamwiseDatabase", order = 1)]
    public class SamwiseDatabase : ScriptableObject, IDialogueSet
    {
        public List<SamwiseAsset> Assets;

        public bool GetDialogue(string dialogueSymbol, out Dialogue dialogue)
        {
            Initialize();
            return cache.GetDialogue(dialogueSymbol, out dialogue);

        }

        public IDialogueNode GetNodeFromLabel(string dialogueSymbol, string label)
        {
            Initialize();
            return cache.GetNodeFromLabel(dialogueSymbol,  label);
        }

        public bool Contains(Dialogue dialogue)
        {
            Initialize();
            return cache.Contains(dialogue);
        }
        
        public void ClearCache()
        {
            if (cache != null)
            {
                GameObject.Destroy(cache);
                cache = null;
            }
        }

        void Initialize()
        {
            if (cache)
                return;

            var go = new GameObject("_DBCache");
            cache = go.AddComponent<SamwiseDatabaseCache>();
            cache.Initialize(this);
        }

        private SamwiseDatabaseCache cache;
    }
}
