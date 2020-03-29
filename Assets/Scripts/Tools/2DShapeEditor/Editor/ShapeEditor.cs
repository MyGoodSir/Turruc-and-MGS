using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ShapeCreator))]
public class ShapeEditor : Editor
{
    ShapeCreator shapeCreator;
    bool needRepaint;
    SelectionInfo selectionInfo;


    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        int deleteIndex = -1;

        shapeCreator.showShapesList = EditorGUILayout.Foldout(shapeCreator.showShapesList, "Show Shapes List");
        if (shapeCreator.showShapesList)
        {
            for (int i = 0; i < shapeCreator.shapes.Count; i++)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Shape" + (i + 1));

                GUI.enabled = i != selectionInfo.shapeIndex;
                if (GUILayout.Button("Select"))
                {
                    selectionInfo.shapeIndex = i;
                }
                GUI.enabled = true;
                if (GUILayout.Button("Delete"))
                {
                    deleteIndex = i;

                }
                GUILayout.EndHorizontal();
            }
        }
        if (deleteIndex != -1)
        {
            Undo.RecordObject(shapeCreator, "Delete Shape");
            shapeCreator.shapes.RemoveAt(deleteIndex);
            selectionInfo.shapeIndex = Mathf.Clamp(selectionInfo.shapeIndex, 0, shapeCreator.shapes.Count - 1);
           
        }
        if (GUI.changed)
        {
            needRepaint = true;
            SceneView.RepaintAll();
        }
    }


    void OnSceneGUI()
    {
        
        Event guiEvent = Event.current;

        if(guiEvent.type == EventType.Repaint)
        {
            Draw();
        }
        else if(guiEvent.type == EventType.Layout)
        {
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
        }
        else
        {
            HandleInput(guiEvent);
            if (needRepaint)
            {
                HandleUtility.Repaint();

            }
        }
        
    }

    void CreateShape()
    {
        Undo.RecordObject(shapeCreator, "Create shape");
        shapeCreator.shapes.Add(new Shape());
        selectionInfo.shapeIndex = shapeCreator.shapes.Count - 1;
    }

    void CreatePoint(Vector3 mousePosition)
    {
        bool hoverCurrentShape = selectionInfo.hoverShapeIndex == selectionInfo.shapeIndex;
        int newPointIndex = (selectionInfo.hoverLine && hoverCurrentShape) ? selectionInfo.lineIndex + 1 : SelectedShape.points.Count;
        Undo.RecordObject(shapeCreator, "Add point");
        SelectedShape.points.Insert(newPointIndex, mousePosition);
        selectionInfo.pointIndex = newPointIndex;
        selectionInfo.hoverShapeIndex = selectionInfo.shapeIndex;
        needRepaint = true;

        SelectPoint();
    }

    void DeletePoint()
    {
        Undo.RecordObject(shapeCreator, "Delete Point");
        SelectedShape.points.RemoveAt(selectionInfo.pointIndex);
        selectionInfo.selectedPoint = false;
        selectionInfo.hoverPoint = false;
        needRepaint = true;
    }

    void SelectPoint()
    {
        selectionInfo.selectedPoint = true;
        selectionInfo.hoverPoint = true;
        selectionInfo.hoverLine = false;
        selectionInfo.lineIndex = -1;

        selectionInfo.previousPosition = SelectedShape.points[selectionInfo.pointIndex];
        needRepaint = true;
    }
    void SelectShape()
    {
        if(selectionInfo.hoverShapeIndex != -1)
        {
            selectionInfo.shapeIndex = selectionInfo.hoverShapeIndex;
            needRepaint = true;
        }
    }
    
    void HandleInput(Event guiEvent)
    {
        Ray mouseRay = HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition);
        float drawPlaneHeight = 0;
        float distToPlane = (drawPlaneHeight - mouseRay.origin.y) / mouseRay.direction.y;
        Vector3 mousePosition = mouseRay.GetPoint(distToPlane);


        if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0 && guiEvent.modifiers == EventModifiers.Shift)
        {
            HandleShiftM1Down(mousePosition);
        }
        if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0 && guiEvent.modifiers == EventModifiers.None)
        {
            HandleM1Down(mousePosition);
        }
        if (guiEvent.type == EventType.MouseUp && guiEvent.button == 0)
        {
            HandleM1Up(mousePosition);
        }
        if (guiEvent.type == EventType.MouseDrag && guiEvent.button == 0 && guiEvent.modifiers == EventModifiers.None)
        {
            HandleM1Drag(mousePosition);
        }
        if (!selectionInfo.selectedPoint)
        {
            UpdateHover(mousePosition);
        }

    }

    void HandleShiftM1Down(Vector3 mousePosition)
    {
        if (selectionInfo.hoverPoint)
        {
            SelectShape();
            DeletePoint();
        }
        else
        {
            CreateShape();
            CreatePoint(mousePosition);
        }
    }

    void HandleM1Down(Vector3 mousePosition)
    {
        if (shapeCreator.shapes.Count == 0)
        {
            CreateShape();
        }

        SelectShape();

        if (selectionInfo.hoverPoint)
        {
            SelectPoint();
        }
        else
        {
            CreatePoint(mousePosition);
        }

    }
    void HandleM1Up(Vector3 mousePosition)
    {
        if (selectionInfo.selectedPoint)
        {
            SelectedShape.points[selectionInfo.pointIndex] = selectionInfo.previousPosition;
            Undo.RecordObject(shapeCreator, "Move point");
            SelectedShape.points[selectionInfo.pointIndex] = mousePosition;
            selectionInfo.selectedPoint = false;
            selectionInfo.pointIndex = -1;
            needRepaint = true;
        }
    }
    void HandleM1Drag(Vector3 mousePosition)
    {
        if (selectionInfo.selectedPoint)
        {
            SelectedShape.points[selectionInfo.pointIndex] = mousePosition;
            needRepaint = true;
        }
    }

    void UpdateHover(Vector3 mousePosition)
    {
        int hoverPointIndex = -1;
        int hoverShapeIndex = -1;
        for (int shapeIndex = 0; shapeIndex < shapeCreator.shapes.Count; shapeIndex++)
        {
            Shape currentShape = shapeCreator.shapes[shapeIndex];
            for (int i = 0; i < currentShape.points.Count; i++)
            {
                if (Vector3.Distance(mousePosition, currentShape.points[i]) < shapeCreator.handleRadius)
                {
                    hoverPointIndex = i;
                    hoverShapeIndex = shapeIndex;
                    break;
                }
            }
        }
        if(hoverPointIndex != selectionInfo.pointIndex || hoverShapeIndex != selectionInfo.hoverShapeIndex)
        {
            selectionInfo.hoverShapeIndex = hoverShapeIndex;
            selectionInfo.pointIndex = hoverPointIndex;
            selectionInfo.hoverPoint = hoverPointIndex != -1;
            needRepaint = true;
        }
        if (selectionInfo.hoverPoint)
        {
            selectionInfo.hoverLine = false;
            selectionInfo.lineIndex = -1;
        }
        else
        {
            int lineIndex = -1;
            float closestLineDist = shapeCreator.handleRadius;
            for (int shapeIndex = 0; shapeIndex < shapeCreator.shapes.Count; shapeIndex++)
            {
                Shape currentShape = shapeCreator.shapes[shapeIndex];
                for (int i = 0; i < currentShape.points.Count; i++)
                {
                    Vector3 nextPoint = currentShape.points[(i + 1) % currentShape.points.Count];
                    float distToLine = HandleUtility.DistancePointToLineSegment(mousePosition.XZ(), currentShape.points[i].XZ(), nextPoint.XZ());
                    if (distToLine < closestLineDist)
                    {
                        closestLineDist = distToLine;
                        lineIndex = i;
                        hoverShapeIndex = shapeIndex;
                    }
                }
            }

            if(selectionInfo.lineIndex != lineIndex || hoverShapeIndex != selectionInfo.hoverShapeIndex)
            {
                selectionInfo.hoverShapeIndex = hoverShapeIndex;
                selectionInfo.lineIndex = lineIndex;
                selectionInfo.hoverLine = lineIndex != -1;
                needRepaint = true;
            }
        }
    }

    void Draw()
    {
        for (int shapeIndex = 0; shapeIndex < shapeCreator.shapes.Count; shapeIndex++)
        {
            Shape currentShape = shapeCreator.shapes[shapeIndex];
            bool shapeSelected = shapeIndex == selectionInfo.shapeIndex;
            bool hoverShape = shapeIndex == selectionInfo.hoverShapeIndex;
            Color deselectedColor = Color.gray;
            for (int i = 0; i < currentShape.points.Count; i++)
            {
                Vector3 nextPoint = currentShape.points[(i + 1) % currentShape.points.Count];
                if (i == selectionInfo.lineIndex && hoverShape)
                {
                    Handles.color = Color.red;
                    Handles.DrawLine(currentShape.points[i], nextPoint);
                }
                else
                {
                    Handles.color = shapeSelected ? Color.black : deselectedColor;
                    Handles.DrawDottedLine(currentShape.points[i], nextPoint, 4);
                }


                if (i == selectionInfo.pointIndex && hoverShape)
                {
                    Handles.color = selectionInfo.selectedPoint ? Color.black : Color.red;
                }
                else
                {
                    Handles.color = shapeSelected ? Color.white : deselectedColor;
                }

                Handles.DrawSolidDisc(currentShape.points[i], Vector3.up, shapeCreator.handleRadius);
            }
        }

        if (needRepaint)
        {
            shapeCreator.updateMeshDisplay();
        }

        needRepaint = false;
    }
    private void OnEnable()
    {
        needRepaint = true;
        shapeCreator = target as ShapeCreator;
        selectionInfo = new SelectionInfo();
        Undo.undoRedoPerformed += OnUndoOrRedo;
        Tools.hidden = true;
    }

    private void OnDisable()
    {
        Undo.undoRedoPerformed -= OnUndoOrRedo;
        Tools.hidden = false;
    }
    void OnUndoOrRedo()
    {
        if(selectionInfo.shapeIndex >= shapeCreator.shapes.Count || selectionInfo.shapeIndex == -1)
        {
            selectionInfo.shapeIndex = shapeCreator.shapes.Count - 1;
        }
        needRepaint = true;
    }

    Shape SelectedShape
    {
        get
        {
            return shapeCreator.shapes[selectionInfo.shapeIndex];
        }
    }

    public class SelectionInfo
    {
        public int shapeIndex;//index of currently selected shape
        public int hoverShapeIndex;//index of shape that is below the cursor

        public int pointIndex = -1;//index of current point
        public bool hoverPoint;//cursor is hovering above point
        public bool selectedPoint;//point is selected
        public Vector3 previousPosition;//previous position of a point that was moved

        public int lineIndex = -1;//index of current line
        public bool hoverLine;//cursor is hovering above line 
    }
}
