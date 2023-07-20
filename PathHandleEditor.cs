using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor (typeof (PathHandle))]
public class PathHandleEditor : Editor {

    PathHandle t;

    private void OnEnable () {
        t = (PathHandle) target;
    }

    public override void OnInspectorGUI () {
        base.OnInspectorGUI ();
    }

    private Vector2 selected;
    private int control;

    private List<int> ids = new List<int> ();

    private void OnSceneGUI () {
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

                EditorGUI.BeginChangeCheck ();

                //control = (EditorGUIUtility.hotControl != 0) ? EditorGUIUtility.hotControl : control;

                //if (!ids.Contains (control)) {
                //    ids.Add (control);
                //}

                // Draw Free Move Handle
                Handles.color = t.HandleColor;
                Vector2 newPosition = Handles.FreeMoveHandle (
                    //control,
                    t.Positions[i],
                    Quaternion.identity,
                    size,
                    Vector2.one * t.HandleSnap,
                    Handles.RectangleHandleCap
                );

                if (EditorGUI.EndChangeCheck ()) {
                    Undo.RecordObject (t, "Change Position");
                    t.Positions[i] = newPosition;
                    Debug.Log (control);
                }

                // Draw Line Handle
                Handles.color = t.LineColor;
                Handles.DrawLine (currentPosition, nextPosition);
            }

            // Draw GUI
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
                t.Positions.Remove (selected);
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
}
