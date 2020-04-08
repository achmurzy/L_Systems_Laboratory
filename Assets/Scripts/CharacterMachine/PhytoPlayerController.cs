using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhytoPlayerController : MonoBehaviour
{
    public PhytoControllerMachine Character;
    public CharacterCamera CharacterCamera;

    protected const string MouseXInput = "Mouse X";
    protected const string MouseYInput = "Mouse Y";
    protected const string MouseScrollInput = "Mouse ScrollWheel";
    protected const string HorizontalInput = "Horizontal";
    protected const string VerticalInput = "Vertical";

    protected void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        // Tell camera to follow transform
        CharacterCamera.SetFollowTransform(Character.transform);

        // Ignore the character's collider(s) for camera obstruction checks
        CharacterCamera.IgnoredColliders.Clear();
        CharacterCamera.IgnoredColliders.AddRange(Character.GetComponentsInChildren<Collider>());
    }

    protected void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        HandleCameraInput();
        HandleCharacterInput();
    }

    protected void HandleCameraInput()
    {
        // Create the look input vector for the camera
        float mouseLookAxisUp = Input.GetAxisRaw(MouseYInput);
        float mouseLookAxisRight = Input.GetAxisRaw(MouseXInput);
        Vector3 lookInputVector = new Vector3(mouseLookAxisRight, mouseLookAxisUp, 0f);

        // Prevent moving the camera while the cursor isn't locked
        if (Cursor.lockState != CursorLockMode.Locked)
        {
            lookInputVector = Vector3.zero;
        }

        // Input for zooming the camera (disabled in WebGL because it can cause problems)
        float scrollInput = -Input.GetAxis(MouseScrollInput);
#if UNITY_WEBGL
        scrollInput = 0f;
#endif

        // Apply inputs to the camera
        CharacterCamera.UpdateWithInput(Time.deltaTime, scrollInput, lookInputVector);

        // Handle toggling zoom level
        if (Input.GetMouseButtonDown(1))
        {
            CharacterCamera.TargetDistance = (CharacterCamera.TargetDistance == 0f) ? CharacterCamera.DefaultDistance : 0f;
        }
    }

    protected void HandleCharacterInput()
    {
        PhytoControllerMachine.PhytoCharacterInputs characterInputs = new PhytoControllerMachine.PhytoCharacterInputs();

        // Build the CharacterInputs struct
        characterInputs.MoveAxisForward = Input.GetAxisRaw(VerticalInput);
        characterInputs.MoveAxisRight = Input.GetAxisRaw(HorizontalInput);
        characterInputs.CameraRotation = CharacterCamera.Transform.rotation;
        characterInputs.JumpDown = Input.GetKeyDown(KeyCode.Space);
        characterInputs.JumpPressed = Input.GetKey(KeyCode.Space);

        // Apply inputs to character
        Character.SetInputs(ref characterInputs);
    }
}
