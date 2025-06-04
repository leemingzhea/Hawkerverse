/*
 * author: unity forum contributors, mark
 * date: 31/01/25
 * description: adds a help box to the inspector window
 */
// https://discussions.unity.com/t/helpattribute-allows-you-to-use-helpbox-in-the-unity-inspector-window/659414/22

using System;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
public enum HelpBoxMessageType
{
    None,
    Info,
    Warning,
    Error
}

public class HelpBoxAttribute : PropertyAttribute
{
    public readonly HelpBoxMessageType MessageType;
    public readonly string Text;

    public HelpBoxAttribute(string text, HelpBoxMessageType messageType = HelpBoxMessageType.None)
    {
        Text = text;
        MessageType = messageType;
    }
}

[CustomPropertyDrawer(typeof(HelpBoxAttribute))]
public class HelpBoxAttributeDrawer : DecoratorDrawer
{
    public override float GetHeight()
    {
        try
        {
            if (attribute is not HelpBoxAttribute helpBoxAttribute) return base.GetHeight();
            var helpBoxStyle = GUI.skin != null ? GUI.skin.GetStyle("helpbox") : null;
            return helpBoxStyle == null
                ? base.GetHeight()
                : Mathf.Max(40f,
                    helpBoxStyle.CalcHeight(new GUIContent(helpBoxAttribute.Text),
                        EditorGUIUtility.currentViewWidth) +
                    4);
        }
        catch (ArgumentException)
        {
            // handle unity 2022.2 bug by returning default value
            return 3 * EditorGUIUtility.singleLineHeight;
        }
    }

    public override void OnGUI(Rect position)
    {
        if (attribute is not HelpBoxAttribute helpBoxAttribute) return;
        EditorGUI.HelpBox(position, helpBoxAttribute.Text, GetMessageType(helpBoxAttribute.MessageType));
    }

    private static MessageType GetMessageType(HelpBoxMessageType helpBoxMessageType)
    {
        switch (helpBoxMessageType)
        {
            default:
            case HelpBoxMessageType.None: return MessageType.None;
            case HelpBoxMessageType.Info: return MessageType.Info;
            case HelpBoxMessageType.Warning: return MessageType.Warning;
            case HelpBoxMessageType.Error: return MessageType.Error;
        }
    }
}
#endif