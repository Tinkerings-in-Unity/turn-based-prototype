#define USE_NEW_INPUT_SYSTEM
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{

    public static InputManager Instance { get; private set; }

    private PlayerInputActions playerInputActions;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one InputManager! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;

        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
    }


    public Vector2 GetMouseScreenPosition()
    {
#if USE_NEW_INPUT_SYSTEM
        return Mouse.current.position.ReadValue();
#else
        return Input.mousePosition;
#endif
    }

    public bool IsMouseButtonDownThisFrame()
    {
// #if USE_NEW_INPUT_SYSTEM
//         return playerInputActions.Player.Click.WasPressedThisFrame();
// #else
        return Input.GetMouseButtonDown(0);
// #endif
    }

    public Vector2 GetCameraMoveVector(out bool mouseHeld)
    {
        Vector2 inputMoveDir = new Vector2(0, 0);
        mouseHeld = false;
        
        if (Input.GetMouseButton(2))
        {
            mouseHeld = true;
            var xValue = Input.GetAxis("Mouse X");
            var yValue = Input.GetAxis("Mouse Y");
            var xSign = Mathf.Sign(xValue);
            var ySign = Mathf.Sign(yValue);
            
            inputMoveDir.y = Mathf.Abs(yValue) > 0f ? ySign * 1f : 0f;
            inputMoveDir.x = Mathf.Abs(xValue) > 0f ? xSign * 1f : 0f;
        }

        if (Input.GetKey(KeyCode.W))
        {
            inputMoveDir.y = +1f;
        }
        if (Input.GetKey(KeyCode.S))
        {
            inputMoveDir.y = -1f;
        }
        if (Input.GetKey(KeyCode.A))
        {
            inputMoveDir.x = -1f;
        }
        if (Input.GetKey(KeyCode.D))
        {
            inputMoveDir.x = +1f;
        }

        return inputMoveDir;
    }

    public float GetCameraRotateAmount()
    {
// #if USE_NEW_INPUT_SYSTEM
//         return playerInputActions.Player.CameraRotate.ReadValue<float>();
// #else
        float rotateAmount = 0f;

        if (Input.GetMouseButton(1))
        {
            var value = Input.GetAxis("Mouse X");
            var sign = Mathf.Sign(value);
            rotateAmount = Mathf.Abs(value) > 0f ? sign * 1f : 0f;
        }

        if (Input.GetKey(KeyCode.Q))
        {
            rotateAmount = +1f;
        }
        if (Input.GetKey(KeyCode.E))
        {
            rotateAmount = -1f;
        }

        return rotateAmount;
// #endif
    }

    public float GetCameraZoomAmount()
    {
#if USE_NEW_INPUT_SYSTEM
        return playerInputActions.Player.CameraZoom.ReadValue<float>();
#else
        float zoomAmount = 0f;

        if (Input.mouseScrollDelta.y > 0)
        {
            zoomAmount = -1f;
        }
        if (Input.mouseScrollDelta.y < 0)
        {
            zoomAmount = +1f;
        }

        return zoomAmount;
#endif
    }


}
