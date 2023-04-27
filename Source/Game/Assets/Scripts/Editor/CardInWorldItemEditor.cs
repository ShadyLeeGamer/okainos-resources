using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CardInWorldItem))]
public class CardInWorldItemEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var cardInWorldItem = target as CardInWorldItem;
        if (GUILayout.Button("Refresh Distortion Renderers"))
            cardInWorldItem.RefreshDistortionRenderersToRelativeScale();
    }
}