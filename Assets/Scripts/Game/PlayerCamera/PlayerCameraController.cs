using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Camera))]
public class PlayerCameraController : MonoBehaviour
{
    InputSystem_Actions playerInputs;

    [Header("Components")]//////////////////////////////////////////////////
    [SerializeField] Camera playerCamera;
    public Camera PlayerCamera
    {
        get
        {
            if (playerCamera == null)
                playerCamera = GetComponent<Camera>();
            return playerCamera;
        }
    }

    PlayerSelectionComponent playerSelection;
    PlayerSelectionComponent PlayerSelection
    {
        get
        {
            if (playerSelection == null)
                playerSelection = new PlayerSelectionComponent(PlayerCamera, interactionRange);
            return playerSelection;
        }
    }

    [Header("Interaction")]////////////////////////////////////////////////

    [SerializeField] float interactionRange = 1000f;
    public float InteractionRange
    {
        get { return interactionRange; }
    }


    private void Awake()
    {

    }

    void Start()
    {

        playerInputs = new InputSystem_Actions();
        playerInputs.Enable();

        /// Debug ///
        playerInputs.Player.DebugLevelUp.performed += OnDebugUp;
        playerInputs.Player.DebugLevelDown.performed += OnDebugDown;

        /// Player Actions ///
        playerInputs.Player.Interact.performed += OnInteract;
        playerInputs.Player.ContextAction.performed += OnContextAction;

        Cursor.lockState = CursorLockMode.Confined;
    }

    void OnDestroy()
    {
        /// Debug ///
        playerInputs.Player.DebugLevelUp.performed -= OnDebugUp;
        playerInputs.Player.DebugLevelDown.performed -= OnDebugDown;

        /// Player Actions ///
        playerInputs.Player.Interact.performed -= OnInteract;
        playerInputs.Player.ContextAction.performed -= OnContextAction;

        playerInputs.Disable();

        playerSelection = null;
    }

    void Update()
    {
        MousePos(Mouse.current.position.ReadValue());
    }

    void OnInteract(InputAction.CallbackContext context)
    {
        Interact(Mouse.current.position.ReadValue());
    }

    void OnContextAction(InputAction.CallbackContext context)
    {
        ContextAction(Mouse.current.position.ReadValue());
    }

    void OnDebugUp(InputAction.CallbackContext context)
    {
        DebugUp();
    }

    void OnDebugDown(InputAction.CallbackContext context)
    {
        DebugDown();
    }

    void MousePos(Vector2 mousePos)
    {
        PlayerSelection.TryToHover(mousePos);
    }

    void Interact(Vector2 interactPos)
    {
        PlayerSelection.PointSelection(interactPos);
    }

    void ContextAction(Vector2 interactPos)
    {
        PlayerSelection.ContextAction(interactPos);
    }

    //////////////////// DEBUG //////////////////////////

    void DebugUp()
    {
        Ultra.Utilities.Instance.debugLevel += 100;
    }
    void DebugDown()
    {
        Ultra.Utilities.Instance.debugLevel -= 100;
    }


}
