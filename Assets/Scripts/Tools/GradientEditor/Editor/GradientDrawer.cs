using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
//handles the gradient rectangle that's visible in the object inspector

[CustomPropertyDrawer(typeof(CustomGradient))]
public class GradientDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        Event guiEvent = Event.current;

        CustomGradient gradient = (CustomGradient)fieldInfo.GetValue(property.serializedObject.targetObject);
        float labelWidth = GUI.skin.label.CalcSize(label).x + 5;

        //rectangle that has the gradient drawn to it
        Rect texRect = new Rect(position.x + labelWidth, position.y, position.width - labelWidth, position.height);

        if (guiEvent.type == EventType.Repaint)
        {
            
            GUI.Label(position, label);

            GUIStyle gradientStyle = new GUIStyle();
            gradientStyle.normal.background = gradient.GetTexture((int)position.width);
            GUI.Label(texRect, GUIContent.none, gradientStyle);

        }
        else
        {
            //left-click
            if(guiEvent.type == EventType.MouseDown && guiEvent.button == 0)
            {
                //cursor over the gradient
                if(texRect.Contains(guiEvent.mousePosition))
                {
                    //open editor window
                    GradientEditor gradientEditorWindow = EditorWindow.GetWindow<GradientEditor>();
                    gradientEditorWindow.SetGradient(gradient);
                }
            }
        }
    }
}
