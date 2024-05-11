using UnityEngine;
using UnityEditor;
using System;
using Peevo.Samwise.Unity;
using System.Collections.Generic;

namespace Peevo.Samwise
{
    [CustomPropertyDrawer(typeof(SamwiseReferenceAttribute))]
    public class SamwiseReferenceDrawer : PropertyDrawer
    {

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var propertyType = property.propertyType;

            if (propertyType == SerializedPropertyType.String)
            {
                string stringValue = property.stringValue;
                OnGUI(position, ref stringValue, property.displayName);
                property.stringValue = stringValue;
            }
        }

        public void OnGUI(Rect position, ref string value, string displayName)
        {
            if (dialoguesList == null)
            {
                var tempDialogueList = new List<string>();
                var tempDialogueListNested = new List<string>();

                var databases = AssetDatabase.FindAssets("t: SamwiseDatabase");

                foreach (string database in databases)
                {
                    string path = AssetDatabase.GUIDToAssetPath(database);
                    var databaseObject = AssetDatabase.LoadAssetAtPath<SamwiseDatabase>(path.Replace("\\", "/"));

                    if (databaseObject != null)
                    {
                        foreach (var asset in databaseObject.Assets)
                        {
                            foreach (var d in asset.Dialogues)
                            {
                                tempDialogueList.Add(d.Label);
                                tempDialogueListNested.Add(asset.name + "/" + d.Label);
                            }
                        }
                    }
                }

                if (tempDialogueList.Count > 0)
                {
                    dialoguesList = tempDialogueList.ToArray();
                    tempDialogueListNested.Add("[None]");
                    dialoguesListNested = tempDialogueListNested.ToArray();
                }
                else
                {
                    dialoguesList = new string[] { "No Dialogues found" };
                    dialoguesListNested = new string[] { "No Dialogues found" };
                }
            }

            var prevSelection = Array.IndexOf(dialoguesList, value);

            var newSelection = EditorGUI.Popup(position, displayName, prevSelection, dialoguesListNested);

            if (newSelection != prevSelection)
            {
                if (newSelection > dialoguesList.Length - 1 || newSelection < 0)
                {
                    prevSelection = -1;
                    value = "";
                }
                else
                {
                    prevSelection = newSelection;
                    value = dialoguesList[newSelection];
                }
            }
        }

        string[] dialoguesList;
        string[] dialoguesListNested;
    }
}