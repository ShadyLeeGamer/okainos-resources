using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CardAsset))]
public class CardAssetEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var cardAsset = target as CardAsset;

        if (GUILayout.Button("REFRESH READ-ONLY DATA"))
            cardAsset.Refresh();

        if (GUILayout.Button("SEND DATA TO CLOUD"))
            CardDataCargo.SendCardDataToCloud(cardAsset);

        if (GUILayout.Button("RETRIEVE DATA FROM CLOUD"))
            CardDataCargo.RetrieveCardDataFromCloud(cardAsset);

        EditorUtility.SetDirty(target);
    }
}