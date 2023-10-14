using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    private const float MIN_FOLLOW_Y_OFFSET = 2f;
    private const float MAX_FOLLOW_Y_OFFSET = 15f;

    public static CameraController Instance { get; private set; }

    [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;
    [SerializeField] private Transform followTransform;
    [SerializeField] private float followSpeed = 15f;

    private CinemachineTransposer cinemachineTransposer;
    private Vector3 targetFollowOffset;
    private List<ObjectVisibility> _raycastHitObjects = new List<ObjectVisibility>();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        cinemachineTransposer = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        targetFollowOffset = cinemachineTransposer.m_FollowOffset;
    }

    private void Update()
    {
        HandleMovement();
        HandleRotation();
        // HandleZoom();
    }

    private void HandleMovement()
    {
        ToggleOccludingObjectsVisibilityOff();
        Vector2 inputMoveDir = InputManager.Instance.GetCameraMoveVector(out var mouseButtonHeld);

        float moveSpeed = 10f;
        var moved = false;

        Vector3 moveVector = transform.forward * inputMoveDir.y + transform.right * inputMoveDir.x;

        if (moveVector == Vector3.zero)
        {
            if (!mouseButtonHeld)
            {
                if (Mathf.Abs(Vector3.Distance(transform.position, followTransform.position)) > 1f)
                {
                    transform.position = Vector3.Slerp(transform.position, followTransform.position,
                        (followSpeed * 0.1f) * Time.deltaTime);
                }
                else
                {
                    transform.position = Vector3.Slerp(transform.position, followTransform.position,
                        followSpeed * Time.deltaTime);
                }
            }

            ToggleOccludingObjectsVisibilityOff();
        }
        else
        {
            transform.position = Vector3.Slerp(transform.position, transform.position + moveVector,
                moveSpeed * Time.deltaTime);
        }
    }

    private void HandleRotation()
    {
        Vector3 rotationVector = new Vector3(0, 0, 0);

        rotationVector.y = InputManager.Instance.GetCameraRotateAmount();

        float rotationSpeed = 100f;
        transform.eulerAngles += rotationVector * rotationSpeed * Time.deltaTime;
    }

    private void HandleZoom()
    {
        float zoomIncreaseAmount = 1f;
        targetFollowOffset.y += InputManager.Instance.GetCameraZoomAmount() * zoomIncreaseAmount;

        targetFollowOffset.y = Mathf.Clamp(targetFollowOffset.y, MIN_FOLLOW_Y_OFFSET, MAX_FOLLOW_Y_OFFSET);

        float zoomSpeed = 5f;
        cinemachineTransposer.m_FollowOffset =
            Vector3.Lerp(cinemachineTransposer.m_FollowOffset, targetFollowOffset, Time.deltaTime * zoomSpeed);
    }

    public float GetCameraHeight()
    {
        return targetFollowOffset.y;
    }

    private void ToggleOccludingObjectsVisibilityOff()
    {
        if (_raycastHitObjects.Count > 0)
        {
            foreach (var raycastHitObject in _raycastHitObjects)
            {
                raycastHitObject.Show();
            }

            _raycastHitObjects.Clear();
        }

        Ray ray = Camera.main.ScreenPointToRay(InputManager.Instance.GetMouseScreenPosition());
        var cameraPosition = Camera.main.transform.position;
        var direction = followTransform.position - cameraPosition;
        var distance = (direction).magnitude;
        direction = direction.normalized;
        RaycastHit[] raycastHitArray = Physics.RaycastAll(cameraPosition, direction, distance);
        Debug.DrawRay(cameraPosition, direction * distance, Color.magenta);
        System.Array.Sort(raycastHitArray,
            (RaycastHit raycastHitA, RaycastHit raycastHitB) =>
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