using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.UI;

public class InputNavigationSwitcher : MonoBehaviour
{
    [Header("UI Settings")]
    [SerializeField] private GameObject firstSelectedButton;
    [SerializeField] private EventSystem eventSystem;

    [Header("Navigation Mode")]
    [SerializeField] private bool showDebugLogs = true;

    private bool isUsingController = false;
    private GameObject lastSelectedObject;

    // Track last input device
    private InputDevice lastUsedDevice;

    private void Awake()
    {
        // Get EventSystem if not assigned
        if (eventSystem == null)
        {
            eventSystem = EventSystem.current;
        }

        if (eventSystem == null)
        {
            Debug.LogError("No EventSystem found! Add one to your scene.");
        }

        // Subscribe to input device changes
        InputSystem.onAnyButtonPress.CallOnce(ctrl => OnAnyInput(ctrl.device));
    }

    private void OnEnable()
    {
        // Listen for any input from any device
        InputSystem.onAnyButtonPress.CallOnce(ctrl => OnAnyInput(ctrl.device));
    }

    private void Start()
    {
        // Start with mouse navigation (no selection)
        SetMouseNavigation();
    }

    private void Update()
    {
        DetectInputType();

        // Handle controller navigation when active
        if (isUsingController)
        {
            EnsureSelectionExists();
        }
    }

    private void OnAnyInput(InputDevice device)
    {
        lastUsedDevice = device;

        // Re-register the callback for next input
        InputSystem.onAnyButtonPress.CallOnce(ctrl => OnAnyInput(ctrl.device));
    }

    private void DetectInputType()
    {
        // Check for mouse movement
        var mouse = Mouse.current;
        if (mouse != null)
        {
            Vector2 mouseDelta = mouse.delta.ReadValue();
            if (mouseDelta.sqrMagnitude > 0.1f)
            {
                if (isUsingController)
                {
                    SwitchToMouse();
                }
            }

            // Check for mouse clicks
            if (mouse.leftButton.wasPressedThisFrame ||
                mouse.rightButton.wasPressedThisFrame ||
                mouse.middleButton.wasPressedThisFrame)
            {
                if (isUsingController)
                {
                    SwitchToMouse();
                }
            }
        }

        // Check for gamepad input
        var gamepad = Gamepad.current;
        if (gamepad != null)
        {
            // Check left stick
            Vector2 leftStick = gamepad.leftStick.ReadValue();
            if (leftStick.sqrMagnitude > 0.1f)
            {
                if (!isUsingController)
                {
                    SwitchToController();
                }
            }

            // Check D-pad
            Vector2 dpad = gamepad.dpad.ReadValue();
            if (dpad.sqrMagnitude > 0.1f)
            {
                if (!isUsingController)
                {
                    SwitchToController();
                }
            }

            // Check buttons
            if (gamepad.buttonSouth.wasPressedThisFrame ||
                gamepad.buttonEast.wasPressedThisFrame ||
                gamepad.buttonNorth.wasPressedThisFrame ||
                gamepad.buttonWest.wasPressedThisFrame ||
                gamepad.startButton.wasPressedThisFrame ||
                gamepad.selectButton.wasPressedThisFrame)
            {
                if (!isUsingController)
                {
                    SwitchToController();
                }
            }
        }

        // Check for keyboard input (arrow keys, WASD, Enter, Escape)
        var keyboard = Keyboard.current;
        if (keyboard != null)
        {
            if (keyboard.upArrowKey.wasPressedThisFrame ||
                keyboard.downArrowKey.wasPressedThisFrame ||
                keyboard.leftArrowKey.wasPressedThisFrame ||
                keyboard.rightArrowKey.wasPressedThisFrame ||
                keyboard.enterKey.wasPressedThisFrame ||
                keyboard.escapeKey.wasPressedThisFrame ||
                keyboard.tabKey.wasPressedThisFrame)
            {
                if (!isUsingController)
                {
                    SwitchToController();
                }
            }
        }
    }

    private void SwitchToController()
    {
        isUsingController = true;

        if (showDebugLogs)
        {
            string deviceName = lastUsedDevice != null ? lastUsedDevice.displayName : "Keyboard/Gamepad";
            Debug.Log($"Switched to Controller Navigation ({deviceName})");
        }

        // Select the first button or last selected object
        if (eventSystem.currentSelectedGameObject == null)
        {
            SelectDefaultButton();
        }

        // Hide mouse cursor (optional)
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void SwitchToMouse()
    {
        isUsingController = false;

        if (showDebugLogs)
        {
            Debug.Log("Switched to Mouse Navigation");
        }

        // Store last selected for when we switch back to controller
        if (eventSystem.currentSelectedGameObject != null)
        {
            lastSelectedObject = eventSystem.currentSelectedGameObject;
        }

        // Deselect current selection
        eventSystem.SetSelectedGameObject(null);

        // Show mouse cursor
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void SelectDefaultButton()
    {
        GameObject toSelect = null;

        // Try to use last selected object
        if (lastSelectedObject != null && lastSelectedObject.activeInHierarchy)
        {
            toSelect = lastSelectedObject;
        }
        // Otherwise use the first selected button
        else if (firstSelectedButton != null && firstSelectedButton.activeInHierarchy)
        {
            toSelect = firstSelectedButton;
        }
        // Last resort: find any selectable
        else
        {
            Selectable selectable = FindObjectOfType<Selectable>();
            if (selectable != null)
            {
                toSelect = selectable.gameObject;
            }
        }

        if (toSelect != null)
        {
            eventSystem.SetSelectedGameObject(toSelect);

            if (showDebugLogs)
            {
                Debug.Log($"Selected: {toSelect.name}");
            }
        }
    }

    private void EnsureSelectionExists()
    {
        // If nothing is selected in controller mode, select something
        if (eventSystem.currentSelectedGameObject == null)
        {
            SelectDefaultButton();
        }
    }

    // Public methods to manually switch modes
    public void ForceMouseMode()
    {
        SwitchToMouse();
    }

    public void ForceControllerMode()
    {
        SwitchToController();
    }

    public void SetFirstSelectedButton(GameObject button)
    {
        firstSelectedButton = button;
    }

    private void SetMouseNavigation()
    {
        isUsingController = false;

        // Show the cursor
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // Deselect any UI element
        if (eventSystem != null)
        {
            eventSystem.SetSelectedGameObject(null);
        }

        if (showDebugLogs)
        {
            Debug.Log("Initialized with Mouse Navigation");
        }
    }

    // Public getters
    public bool IsUsingController => isUsingController;
    public GameObject CurrentSelection => eventSystem.currentSelectedGameObject;
    public InputDevice LastUsedDevice => lastUsedDevice;
}