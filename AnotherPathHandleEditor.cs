using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public enum HandleType {
    XY,
    XYZ
}

public class HandleSelection {
    public int index = -1;

    public bool over;
    public bool selected;

    public Vector3 startDragPosition;
}

[CustomEditor (typeof (AnotherPathHandle))]
public class AnotherPathHandleEditor : Editor {

    private AnotherPathHandle t;
    private HandleSelection s;
    private bool doRepaint;

    private void OnEnable () {
        t = (AnotherPathHandle) target;
        s = new HandleSelection ();
    }

    public override void OnInspectorGUI () {
        base.OnInspectorGUI ();
    }

    private void OnSceneGUI () {
        Event e = Event.current;

        switch (e.type) {
            case EventType.Repaint:
                DrawHandle ();
                break;
            case EventType.Layout:
                HandleUtility.AddDefaultControl (GUIUtility.GetControlID (FocusType.Passive));
                break;
            default:
                InputHandle (e);
                if (doRepaint) {
                    HandleUtility.Repaint ();
                }
                break;
        }

        DrawMenu ();
    }

    private void InputHandle (Event e) {
        Ray r = HandleUtility.GUIPointToWorldRay (e.mousePosition);
        float h = 0;
        float d = t.HType == HandleType.XYZ ? (h - r.origin.y) / r.direction.y : 0;
        Vector3 point = r.GetPoint (d);
        Vector3 mousePosition = new Vector3 (
            point.x,
            point.y,
            t.HType == HandleType.XYZ ? point.z : 0
        );

        if (e.type == EventType.MouseDown && e.button == 0 && e.modifiers == EventModifiers.None) {
            OnHandleDown (mousePosition);
        }

        if (e.type == EventType.MouseUp && e.button == 0 && e.modifiers == EventModifiers.None) {
            OnHandleUp (mousePosition);
        }

        if (e.type == EventType.MouseDrag && e.button == 0 && e.modifiers == EventModifiers.None) {
            OnHandleDrag (mousePosition);
        }

        if (!s.selected) {
            OnMouseOver (mousePosition);
        }
    }

    private void OnHandleDown (Vector3 mousePosition) {
        if (!s.over) {
            Undo.RecordObject (t, "New Point");
            t.Positions.Add (mousePosition);
            s.index = t.Positions.Count;
        }
        s.selected = true;
        s.startDragPosition = mousePosition;
        doRepaint = true;
    }

    private void OnHandleUp (Vector3 mousePosition) {
        if (s.selected) {
            t.Positions[s.index] = s.startDragPosition;
            Undo.RecordObject (t, "Move Position");
            t.Positions[s.index] = mousePosition;
            s.selected = false;
            s.index = -1;
            doRepaint = true;
        }
    }

    private void OnHandleDrag (Vector3 mousePosition) {
        if (s.selected) {
            t.Positions[s.index] = mousePosition;
            doRepaint = true;
        }
    }

    private void OnMouseOver (Vector3 mousePosition) {
        int index = -1;
        for (int i = 0; i < t.Positions.Count; i++) {
            // TODO: Check Vector2.Distance when change type to XY
            // TODO: Check for Size based on screen size and constant size
            if (Vector3.Distance (mousePosition, t.Positions[i]) < t.SizeFactor) {
                index = i;
                break;
            }
        }

        if (index != s.index) {
            s.index = index;
            s.over = index != -1;
            doRepaint = true;
        }
    }

    private void DrawHandle () {
        float size = t.SizeFactor;

        if (t.EnableConstantScreenSize) {
            size = HandleUtility.GetHandleSize (t.Position) * t.ScreenSizeFactor;
        }

        if (t.Positions != null & t.Positions.Count > 0) {
            for (int i = 0; i < t.Positions.Count; i++) {
                Vector2 currentPosition = t.Positions[i];
                Vector2 nextPosition = Vector2.zero;
                if (i < t.Positions.Count - 1) {
                    nextPosition = t.Positions[i + 1];
                } else {
                    if (t.ClosePath) {
                        nextPosition = t.Positions[0];
                    } else {
                        nextPosition = t.Positions[t.Positions.Count - 1];
                    }
                }

                // Draw Disc Handle
                if (i == s.index) {
                    Handles.color = s.selected ?
                        t.HandleSelectedColor :
                        t.HandleOverColor;
                } else {
                    Handles.color = t.HandleColor;
                }
                Handles.DrawSolidDisc (
                    t.Positions[i],
                    t.HType == HandleType.XYZ ? Vector3.up : Vector3.forward,
                    size
                );

                // Draw Line Handle
                // Fix Line to Draw in 2D and 3D
                Handles.color = t.LineColor;
                Handles.DrawLine (currentPosition, nextPosition);
            }

            doRepaint = false;
        }
    }

    private void DrawMenu () {
        Handles.BeginGUI ();
        Rect bgRect = new Rect (20, 20, 200, 100);
        GUILayout.BeginArea (bgRect);

        Rect mainRect = EditorGUILayout.BeginVertical ();
        GUI.Box (mainRect, GUIContent.none);

        GUILayout.BeginHorizontal ();
        GUILayout.FlexibleSpace ();
        string labelText = "Object 1";
        GUILayout.Label (labelText, EditorStyles.centeredGreyMiniLabel);
        GUILayout.FlexibleSpace ();
        GUILayout.EndHorizontal ();

        GUILayout.BeginHorizontal ();
        if (GUILayout.Button ("<")) { }
        if (GUILayout.Button ("Align to View")) { }
        if (GUILayout.Button (">")) { }
        GUILayout.EndHorizontal ();

        GUILayout.BeginHorizontal ();
        if (GUILayout.Button ("+ Add")) { }
        if (GUILayout.Button ("- Rem")) {
            //t.Positions.Remove (selected);
        }
        GUILayout.EndHorizontal ();

        GUILayout.BeginHorizontal ();
        GUILayout.FlexibleSpace ();
        string labelPosition = t.Positions[0].ToString ();
        GUILayout.Label (labelPosition, EditorStyles.centeredGreyMiniLabel);
        GUILayout.FlexibleSpace ();
        GUILayout.EndHorizontal ();

        EditorGUILayout.EndVertical ();

        GUILayout.EndArea ();
        Handles.EndGUI ();
    }
}
