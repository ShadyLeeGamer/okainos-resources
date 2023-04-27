using UnityEngine;
using UnityEditor;
using System.IO;

[CustomEditor(typeof(CardDatabase))]
public class CardIdManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        /*base.OnInspectorGUI();

        var cardIdManager = target as CardIdManager;

        if (GUILayout.Button("RESCAN CARD FOLDERS"))
            cardIdManager.RescanFolders();

        EditorUtility.SetDirty(target);*/
    }
}