using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using JohnStairs.RCC.Character;
using UnityEngine;
using Random = UnityEngine.Random;

public class CameraManager : MonoBehaviour
{
    
    [SerializeField] private GameObject actionCameraGameObject;
    [SerializeField] private GameObject actionCamera2GameObject;
    [SerializeField] private GameObject actionCamera3GameObject;
    [SerializeField] private float actionCamera1Height = 1.7f;
    [SerializeField] private bool showCamera;
    [SerializeField] private CameraChoice cameraToShow;
    [SerializeField] private int cameraPointToShow;
    
    
    public List<string> TagsForFading = new List<string>() { "FadeOut" };

    private RPGViewFrustum _rpgViewFrustum;
    private List<ObjectVisibility> _raycastHitObjects = new List<ObjectVisibility>();

    enum CameraChoice
    {
        Camera1,
        Camera2,
        Camera3
    }

    private void Awake()
    {
        _rpgViewFrustum = GetComponent<RPGViewFrustum>();
    }

    private void Start()
    {
        BaseAction.OnAnyActionStarted += BaseAction_OnAnyActionStarted;
        BaseAction.OnAnyActionCompleted += BaseAction_OnAnyActionCompleted;

        HideActionCameras();
    }

   

    private void ShowActionCamera(CameraChoice cameraChoice)
    {
        switch (cameraChoice)
        {
            case CameraChoice.Camera1:
                actionCameraGameObject.SetActive(true);
                break;
            case CameraChoice.Camera2:
                actionCamera2GameObject.SetActive(true);
                break;
            case CameraChoice.Camera3:
                actionCamera3GameObject.SetActive(true);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(cameraChoice), cameraChoice, null);
        }
    }

    private void HideActionCameras()
    {
        actionCameraGameObject.SetActive(false);
        actionCamera2GameObject.SetActive(false);
        actionCamera3GameObject.SetActive(false);
    }

    private void BaseAction_OnAnyActionStarted(object sender, EventArgs e)
    {
        switch (sender)
        {
            case ShootAction shootAction:
                Unit shooterUnit = shootAction.GetUnit();
                Unit targetUnit = shootAction.GetTargetUnit();

                Vector3 cameraCharacterHeight = Vector3.up * actionCamera1Height;

                Vector3 shootDir = (targetUnit.GetWorldPosition() - shooterUnit.GetWorldPosition()).normalized;

                float shoulderOffsetAmount = 0.5f;
                Vector3 shoulderOffset = Quaternion.Euler(0, 90, 0) * shootDir * shoulderOffsetAmount;

                Vector3 actionCameraPosition =
                    shooterUnit.GetWorldPosition() +
                    cameraCharacterHeight +
                    shoulderOffset +
                    (shootDir * -1);

                actionCameraGameObject.transform.position = actionCameraPosition;
                actionCameraGameObject.transform.LookAt(targetUnit.GetWorldPosition() + cameraCharacterHeight);
                
                ShowActionCamera(CameraChoice.Camera1);
                break;
             case SwordAction swordAction:
                var attackingUnit = swordAction.GetUnit();
                targetUnit = swordAction.GetTargetUnit();

                var preferredCameras = swordAction.GetPreferredCameras();

                var cameraPoints = attackingUnit.GetCameraPoints;

                var preferredCameraPoints = new List<CameraPoint>();
                
                preferredCameras.ForEach(p => preferredCameraPoints.Add(cameraPoints[p]));
                
                var cameraPoint = preferredCameraPoints[Random.Range(0, preferredCameraPoints.Count)];
               
                var cameraChoice = CameraChoice.Camera2;

                if (cameraPoint.name == "Point7")
                {
                    cameraChoice = CameraChoice.Camera3;
                }

                SwitchCamera(cameraPoint, cameraChoice);
               
                break;
        }
    }

    private void SwitchCamera(CameraPoint cameraPoint, CameraChoice cameraChoice)
    {
        GameObject camera = null;
        
        switch (cameraChoice)
        {
            case CameraChoice.Camera1:
                camera = actionCameraGameObject;
                break;
            case CameraChoice.Camera2:
                camera = actionCamera2GameObject;
                break;
            case CameraChoice.Camera3:
                camera = actionCamera3GameObject;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(cameraChoice), cameraChoice, null);
        }
        
        if (cameraPoint.TryGetCameraPosition(out var cameraPosition))
        {
            camera.transform.position = cameraPosition;

            if (cameraPoint.TryGetCameraLookAt(out var cameraLookAt))
            {
                camera.transform.LookAt(cameraLookAt);
            }
                    
            ToggleOccludingObjectsVisibilityOff(cameraPoint);
            camera.SetActive(true);
        }
    }

    private void BaseAction_OnAnyActionCompleted(object sender, EventArgs e)
    {
        HideActionCameras();

        if (_raycastHitObjects.Count > 0)
        {
            foreach (var raycastHitObject in _raycastHitObjects)
            {
                raycastHitObject.Show();
            }
            _raycastHitObjects.Clear();
        }
    }
    
    private void ToggleOccludingObjectsVisibilityOff(CameraPoint cameraPoint)
    {
        if (cameraPoint.TryGetCameraPosition(out var cameraPosition))
        {
            var visibilityTestPoints = cameraPoint.GetVisibilityTestPoints();

            _raycastHitObjects = new List<ObjectVisibility>();
            
            if (visibilityTestPoints.Count > 0)
            {
                foreach (var visibilityTestPoint in visibilityTestPoints)
                {
                    var direction = visibilityTestPoint.position - cameraPosition;
                    var distance = (direction).magnitude;
                    direction = direction.normalized;
                    RaycastHit[] raycastHitArray = Physics.RaycastAll(cameraPosition, direction, distance);
                    Debug.DrawRay(cameraPosition, direction * distance, Color.magenta);
                    System.Array.Sort(raycastHitArray, (RaycastHit raycastHitA, RaycastHit raycastHitB) =>
                    {
                        return Mathf.RoundToInt(raycastHitA.distance - raycastHitB.distance);
                    });

                    foreach (RaycastHit raycastHit in raycastHitArray)
                    {
                        if (raycastHit.transform.TryGetComponent(out ObjectVisibility objectVisibility))
                        {
                            _raycastHitObjects.Add(objectVisibility);
                            objectVisibility.Hide();
                        }
                    }
                }
            
            }
        }
    }

    private void ShowCamera(CameraChoice cameraChoice, int cameraPointKey)
    {
        var cameraPoint = UnitActionSystem.Instance.GetSelectedUnit().GetCameraPoints[cameraPointKey];
        
        if (cameraPoint.TryGetCameraPosition(out var cameraPosition))
        {
            actionCamera2GameObject.transform.position = cameraPosition;

            if (cameraPoint.TryGetCameraLookAt(out var cameraLookAt))
            {
                actionCamera2GameObject.transform.LookAt(cameraLookAt);
            }
            // else
            // {
            //     actionCamera2GameObject.transform.LookAt(targetUnit.GetWorldPosition());
            // }
                    
            ToggleOccludingObjectsVisibilityOff(cameraPoint);
            ShowActionCamera(CameraChoice.Camera2);
        }
    }
}
