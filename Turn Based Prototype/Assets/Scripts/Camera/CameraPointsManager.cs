using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPointsManager : MonoBehaviour
{
    private Dictionary<int, CameraPoint> _cameraPoints = new Dictionary<int, CameraPoint>();

    private void Start()
    {
        var points = GetComponentsInChildren<CameraPoint>();

        for (var i = 0; i < points.Length; i++)
        {
            _cameraPoints.Add(i + 1, points[i]);
        }
    }

    public Dictionary<int, CameraPoint> GetCameraPoints()
    {
        return _cameraPoints;
    }
}
