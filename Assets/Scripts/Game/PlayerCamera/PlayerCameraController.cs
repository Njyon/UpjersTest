using UnityEngine;
using UnityEngine.InputSystem;

//[RequireComponent(typeof(CameraMoveScript))]
[RequireComponent(typeof(Camera))]
public class PlayerCameraController : MonoBehaviour
{
    InputSystem_Actions playerInputs;

    [Header("Components")]//////////////////////////////////////////////////

    //[SerializeField] CameraMoveScript cameraMoveScript;
    //public CameraMoveScript CameraMoveScript
    //{
    //    get
    //    {
    //        if (cameraMoveScript == null)
    //            cameraMoveScript = GetComponent<CameraMoveScript>();
    //        return cameraMoveScript;
    //    }
    //}

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
        playerInputs.Player.DebugLevelUp.performed += ctx => DebugUp();
        playerInputs.Player.DebugLevelDown.performed += ctx => DebugDown();

        /// Player Actions ///
        //playerInputs.Player.MouseMove.performed += ctx => MousePos(ctx.ReadValue<Vector2>());
        playerInputs.Player.Interact.performed += ctx => Interact(Input.mousePosition);

        Cursor.lockState = CursorLockMode.Confined;
    }

    void Update()
    {
        MousePos(Input.mousePosition);
    }

    void MousePos(Vector2 mousePos)
    {
        //CameraMoveScript.MoveCamera(mousePos);
        PlayerSelection.TryToHover(mousePos);
    }

    void Interact(Vector2 interactPos)
    {
        PlayerSelection.PointSelection(interactPos);
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
