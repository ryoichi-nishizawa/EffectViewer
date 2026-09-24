using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class TouchManager : MonoBehaviour
{
    public static TouchManager Instance { get; private set; } = null;

    #region Events
    /// <summary>
    /// Event fired when a touch or click begins. (Param: Screen Position)
    /// </summary>
    public event Action<Vector2> OnTouchBegan = null;

    /// <summary>
    /// Event fired while a touch or click is being held. (Param: Screen Position)
    /// </summary>
    public event Action<Vector2> OnTouchMoved = null;

    /// <summary>
    /// Event fired when a touch or click ends. (Param: Screen Position)
    /// </summary>
    public event Action<Vector2> OnTouchEnded = null;
    #endregion

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        if (Instance != null)
        {
            return;
        }

        // Automatically generate a GameObject for the TouchManager
        GameObject go = new GameObject("[TouchManager]");
        Instance = go.AddComponent<TouchManager>();
        DontDestroyOnLoad(go);
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Update()
    {
        DetectTouchEvents();
    }

    /// <summary>
    /// Internal method to process and fire touch events each frame.
    /// </summary>
    private void DetectTouchEvents()
    {
        // Touchscreen Input
        if (Touchscreen.current != null)
        {
            var touch = Touchscreen.current.primaryTouch;

            if (touch.press.wasPressedThisFrame)
            {
                OnTouchBegan?.Invoke(touch.position.ReadValue());
            }
            else if (touch.press.isPressed)
            {
                OnTouchMoved?.Invoke(touch.position.ReadValue());
            }
            else if (touch.press.wasReleasedThisFrame)
            {
                OnTouchEnded?.Invoke(touch.position.ReadValue());
            }

            return;
        }

        // Mouse Input (for PC / Editor debugging)
        if (Mouse.current != null)
        {
            var mouse = Mouse.current;

            if (mouse.leftButton.wasPressedThisFrame)
            {
                OnTouchBegan?.Invoke(mouse.position.ReadValue());
            }
            else if (mouse.leftButton.isPressed)
            {
                OnTouchMoved?.Invoke(mouse.position.ReadValue());
            }
            else if (mouse.leftButton.wasReleasedThisFrame)
            {
                OnTouchEnded?.Invoke(mouse.position.ReadValue());
            }
        }
    }

    /// <summary>
    /// Checks if a touch or click started in the current frame.
    /// </summary>
    public bool IsTouchBegan(out Vector2 screenPosition)
    {
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            screenPosition = Touchscreen.current.primaryTouch.position.ReadValue();
            return true;
        }

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            screenPosition = Mouse.current.position.ReadValue();
            return true;
        }

        screenPosition = Vector2.zero;
        return false;
    }

    /// <summary>
    /// Checks if a touch or click is currently active.
    /// </summary>
    public bool TryGetTouchPosition(out Vector2 screenPosition)
    {
        // Touchscreen input
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
        {
            screenPosition = Touchscreen.current.primaryTouch.position.ReadValue();
            return true;
        }

        // Mouse input (for debugging on PC / Editor)
        if (Mouse.current != null && Mouse.current.leftButton.isPressed)
        {
            screenPosition = Mouse.current.position.ReadValue();
            return true;
        }

        screenPosition = Vector2.zero;
        return false;
    }

    /// <summary>
    /// Checks if a touch or click was released in the current frame.
    /// </summary>
    public bool IsTouchEnded(out Vector2 screenPosition)
    {
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasReleasedThisFrame)
        {
            screenPosition = Touchscreen.current.primaryTouch.position.ReadValue();
            return true;
        }

        if (Mouse.current != null && Mouse.current.leftButton.wasReleasedThisFrame)
        {
            screenPosition = Mouse.current.position.ReadValue();
            return true;
        }

        screenPosition = Vector2.zero;
        return false;
    }
}