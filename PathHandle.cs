﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathHandle : MonoBehaviour {

    #region Serialized Fields

    [SerializeField] private float handleSnap = 1f;
    [SerializeField] private bool enableConstantScreenSize = true;
    [SerializeField] private float sizeFactor = 1f;
    [SerializeField] private float screenSizeFactor = 0.5f;
    [SerializeField] private List<Vector3> positions = new List<Vector3> ();
    [SerializeField] private bool closePath = true;
    [SerializeField] private Color lineColor = Color.white;
    [SerializeField] private Color handleColor = Color.red;

    #endregion

    #region Public Properties

    public float HandleSnap { get { return handleSnap; } }

    public bool EnableConstantScreenSize {
        get { return enableConstantScreenSize; }
    }

    public float SizeFactor { get { return sizeFactor; } }

    public float ScreenSizeFactor { get { return screenSizeFactor; } }

    public List<Vector3> Positions {
        get { return positions; }
    }

    public Vector2 Position {
        get { return transform.position; }
    }

    public bool ClosePath { get { return closePath; } }

    public Color LineColor { get { return lineColor; } }

    public Color HandleColor { get { return handleColor; } }

    #endregion
}
