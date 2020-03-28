using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class GradientEditor : EditorWindow
{
    CustomGradient gradient;
    const int borderSize = 10;
    const float keyWidth = 10, keyHeight = 20;
    Rect[] keyRects;
    Rect gradientPreviewRect;
    bool mouseDownOnKey, needRepaint;
    int selectedKeyIndex;
    private void OnGUI()
    {

        Draw();
        HandleInput();

        

        if (needRepaint)
        {
            needRepaint = false;
            Repaint();
        }
    }

    void Draw()
    {
        //Gradient preview
        gradientPreviewRect = new Rect(borderSize, borderSize, position.width - borderSize * 2, 25);
        GUI.DrawTexture(gradientPreviewRect, gradient.GetTexture((int)gradientPreviewRect.width));

        //key points along the length of the gradient
        keyRects = new Rect[gradient.NumKeys];
        for (int i = 0; i < gradient.NumKeys; i++)
        {
            CustomGradient.ColorKey key = gradient.GetKey(i);
            Rect keyRect = new Rect(gradientPreviewRect.x + gradientPreviewRect.width * key.Time - keyWidth / 2f, gradientPreviewRect.yMax + borderSize, keyWidth, keyHeight);
            if (i == selectedKeyIndex)
            {
                EditorGUI.DrawRect(new Rect(keyRect.x - 2, keyRect.y - 2, keyRect.width + 4, keyRect.height + 4), Color.black);
            }

            EditorGUI.DrawRect(keyRect, key.Color);
            keyRects[i] = keyRect;
        }
        

        //Settings options 
        Rect settingsRect = new Rect(borderSize, keyRects[0].yMax + borderSize, position.width - borderSize * 2, position.height);
        GUILayout.BeginArea(settingsRect);
        EditorGUI.BeginChangeCheck();
        Color newCol = EditorGUILayout.ColorField(gradient.GetKey(selectedKeyIndex).Color);
        if (EditorGUI.EndChangeCheck())
        {
            gradient.UpdateKeyColor(selectedKeyIndex, newCol);
        }
        gradient.mode = (CustomGradient.BlendMode)EditorGUILayout.EnumPopup("Blend mode", gradient.mode);
        gradient.randomize = EditorGUILayout.Toggle("Randomize Color", gradient.randomize);
        GUILayout.EndArea();
    }

    void HandleInput()
    {
        Event guiEvent = Event.current;
        //left-click
        if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0)
        {
            //check if mouse is over a key
            for (int i = 0; i < keyRects.Length; i++)
            {
                if (keyRects[i].Contains(guiEvent.mousePosition))
                {
                    mouseDownOnKey = true;
                    selectedKeyIndex = i;
                    needRepaint = true;
                    break;
                }
            }
            //if mouse not over a key, make a new one
            if (!mouseDownOnKey)
            {
                
                float keyTime = Mathf.InverseLerp(gradientPreviewRect.x, gradientPreviewRect.xMax, guiEvent.mousePosition.x);
                Color interpolatedColor = gradient.Evaluate(keyTime);
                Color randomColor = new Color(Random.value, Random.value, Random.value);
                //if random color mode is off, the new key will be set to the interpolated color at that point on the gradient
                selectedKeyIndex = gradient.AddKey(gradient.randomize ? randomColor : interpolatedColor, keyTime);
                mouseDownOnKey = true;
                needRepaint = true;
            }
        }
        if (guiEvent.type == EventType.MouseUp && guiEvent.button == 0)
        {
            mouseDownOnKey = false;
        }
        //if mouse is being dragged, move the key point
        if (mouseDownOnKey && guiEvent.type == EventType.MouseDrag && guiEvent.button == 0)
        {
            float keyTime = Mathf.InverseLerp(gradientPreviewRect.x, gradientPreviewRect.xMax, guiEvent.mousePosition.x);
            selectedKeyIndex = gradient.UpdateKeyTime(selectedKeyIndex, keyTime);
            needRepaint = true;

        }
        //delete selected key when backspace is pressed
        if (guiEvent.type == EventType.KeyDown && guiEvent.keyCode == KeyCode.Backspace)
        {
            gradient.RemoveKey(selectedKeyIndex);
            if (selectedKeyIndex >= gradient.NumKeys)
            {
                selectedKeyIndex--;
            }
            needRepaint = true;
        }
    }
    public void SetGradient(CustomGradient gradient)
    {
        this.gradient = gradient;
    }
    //default window properties
    private void OnEnable()
    {
        titleContent.text = "Gradient Editor";
        position.Set(position.x, position.y, 400, 150);
        minSize = new Vector2(200, 150);
        maxSize = new Vector2(1920, 150);
    }
    private void OnDisable()
    {
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
    }


}
