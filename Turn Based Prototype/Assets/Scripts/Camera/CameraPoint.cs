using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CameraPoint : MonoBehaviour
{
    [SerializeField] private Transform cameraPosition;
    [SerializeField] private Transform cameraLookAt;
    [SerializeField] private List<Transform> visibilityTestPoints;

    public bool TryGetCameraPosition(out Vector3 position)
    {
        position = cameraPosition != null ? cameraPosition.position : Vector3.negativeInfinity;
        return cameraPosition != null;
    } 
    
    public bool TryGetCameraLookAt(out Vector3 position)
    {
        position = cameraLookAt != null ? cameraLookAt.position : Vector3.negativeInfinity;
        return cameraLookAt != null;
    }
    
    public List<Transform> GetVisibilityTestPoints()
    {
        return visibilityTestPoints;
    }
}
