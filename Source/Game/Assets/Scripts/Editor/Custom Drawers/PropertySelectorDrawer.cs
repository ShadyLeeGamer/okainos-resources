using UnityEngine;
using UnityEditor;
using Story;

[CanEditMultipleObjects]
public abstract class PropertySelectorDrawer : PropertyDrawer
{
    protected virtual string[] GetPropertyHeightOfTypes => new string[0];
    protected abstract string SelectorPropertyName { get; }
    protected abstract string GetEnumString(int enumIndex, bool lowercaseFirstChar = false);

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty selectorProperty = GetSelectorProperty(property);
        var selectorRect = new Rect(position.x, position.y, position.width, EditorGUI.GetPropertyHeight(selectorProperty));
        EditorGUI.PropertyField(selectorRect, selectorProperty, GUIContent.none);

        SerializedProperty selectedProperty =
            property.FindPropertyRelative(GetEnumString(selectorProperty.intValue, true));
        if (selectedProperty != null)
            EditorGUI.PropertyField(position, selectedProperty, GUIContent.none, true);
        else
            Debug.LogWarning("Property " + selectedProperty.name + " does not exist");
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        SerializedProperty tempProperty;
        float totalHeight = 0;
        for (int i = 0; i < GetPropertyHeightOfTypes.Length; i++)
        {
            tempProperty = property.Copy();
            while (tempProperty.NextVisible(true))
                if (tempProperty.type == GetPropertyHeightOfTypes[i])
                {
                    totalHeight += EditorGUI.GetPropertyHeight(tempProperty, true);
                    break;
                }
        }
        tempProperty = property.Copy();
        while (tempProperty.NextVisible(true))
            if (tempProperty.type == GetEnumString(GetSelectorProperty(property).intValue))
            {
                totalHeight += EditorGUI.GetPropertyHeight(tempProperty, true);
                break;
            }
        return totalHeight;
    }

    protected void LowercaseFirstChar(ref string str)
    {
        str = char.ToLower(str[0]) + str.Substring(1);
    }
    protected SerializedProperty GetSelectorProperty(SerializedProperty property)
    {
        return property.FindPropertyRelative(SelectorPropertyName);
    }
}

[CustomPropertyDrawer(typeof(StoryEvent))]
public class StoryEventPropertySelectorDrawer : PropertySelectorDrawer
{
    protected override string SelectorPropertyName => "EventType";
    protected override string GetEnumString(int enumIndex, bool lowercaseFirstChar = false)
    {
        string enumString = ((StoryEvent.EventTypeEnum)enumIndex).ToString();
        return enumString;
    }
}

[CustomPropertyDrawer(typeof(CardEffectEdit))]
public class CardEffectEditPropertySelectorDrawer : PropertySelectorDrawer
{
    protected override string SelectorPropertyName => "effectTag";
    protected override string[] GetPropertyHeightOfTypes => new[] { typeof(CardEffectConditionEdit).ToString() };
    protected override string GetEnumString(int enumIndex, bool lowercaseFirstChar = false)
    {
        string enumString = ((CardEffect.Tag)enumIndex).ToString();
        if (lowercaseFirstChar)
            LowercaseFirstChar(ref enumString);
        return enumString;
    }
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        base.OnGUI(position, property, label); // SHOW CARD EFFECT TAG AND PROPRERTIES

        // SHOW CARD EFFECT CONDITIONS EDIT
        SerializedProperty selectedProperty =
            property.FindPropertyRelative(GetEnumString(GetSelectorProperty(property).intValue, true));
        position.y += EditorGUI.GetPropertyHeight(selectedProperty);
        EditorGUI.PropertyField(position, property.FindPropertyRelative("conditionsEdit"), new GUIContent("Conditions Edit"), true);
    }
}

[CustomPropertyDrawer(typeof(CardEffectConditionEdit))]
public class CardEffectConditionEditPropertySelectorDrawer : PropertySelectorDrawer
{
    protected override string SelectorPropertyName => "conditionTag";
    protected override string GetEnumString(int enumIndex, bool lowercaseFirstChar = false)
    {
        string enumString = ((CardEffect.Condition.Tag)enumIndex).ToString();
        if (lowercaseFirstChar)
            LowercaseFirstChar(ref enumString);
        return enumString;
    }
}