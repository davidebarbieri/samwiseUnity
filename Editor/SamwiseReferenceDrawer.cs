using UnityEngine;
using UnityEditor;
using System;
using Peevo.Samwise.Unity;
using System.Collections.Generic;

namespace Peevo.Samwise
{
    [CustomPropertyDrawer(typeof(SamwiseReferenceAttribute))]
    [CustomPropertyDrawer(typeof(SamwiseReference))]
    public class SamwiseReferenceDrawer : PropertyDrawer
    {

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var propertyType = property.propertyType;

            if (propertyType == SerializedPropertyType.String)
            {
                string stringValue = property.stringValue;
                if (OnGUI(position, ref stringValue, property.displayName))
                    property.stringValue = stringValue;
            }
            else if (property.type == "SamwiseReference")
            {
                string stringValue = (string)((SamwiseReference)property.boxedValue);
                if (OnGUI(position, ref stringValue, property.displayName))
                    property.boxedValue = (SamwiseReference) stringValue;
            }
        }

        bool OnGUI(Rect position, ref string value, string displayName)
        {
            if (SamwiseImporter.ToInvalidateDrawers)
            {
                dialoguesList = null;
                SamwiseImporter.ToInvalidateDrawers = false;
            }

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
                                var dialoguePath = asset.name + "/" + d.Label;

                                tempDialogueList.Add(d.Label);
                                tempDialogueListNested.Add(dialoguePath);

                                foreach (var label in d.GetLabels())
                                {
                                    tempDialogueList.Add(d.Label + "." + label.Key);
                                    tempDialogueListNested.Add(dialoguePath + "/" + label.Key);
                                }
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
                    return true;
                }
                else
                {
                    prevSelection = newSelection;
                    value = dialoguesList[newSelection];
                    return true;
                }
            }
            return false;
        }

        string[] dialoguesList;
        string[] dialoguesListNested;
    }
}