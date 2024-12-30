using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
public class CharacterKeyboardMover : MonoBehaviour {
    [Tooltip("Speed of player keyboard-movement, in meters/second")]
    [SerializeField] float speed = 3.5f;
    [SerializeField] float gravity = 9.81f;
    [SerializeField] float jumpHeight = 3.0f;
    [SerializeField] float runMultiplier = 2.0f;
    [SerializeField] float crouchHeight = 1.0f;
    [SerializeField] float normalHeight = 2.0f;
    [SerializeField] float crouchSpeedMultiplier = 0.2f;
    [SerializeField] Text speedText; // UI Text element to display speed

    private bool isJumping = false;
    private bool isCrouching = false;

    private CharacterController cc;
    private Camera playerCamera;

    [SerializeField] InputAction moveAction;
    [SerializeField] InputAction jumpAction;
    [SerializeField] InputAction runAction;
    [SerializeField] InputAction crouchAction;

    private void OnEnable() {
        moveAction.Enable();
        jumpAction.Enable();
        runAction.Enable();
        crouchAction.Enable();
    }

    private void OnDisable() {
        moveAction.Disable();
        jumpAction.Disable();
        runAction.Disable();
        crouchAction.Disable();
    }

    void OnValidate() {
        // Movement Action - Only add bindings if none exist
        if (moveAction == null || moveAction.bindings.Count == 0) {
            moveAction = new InputAction(type: InputActionType.Value);
            moveAction.AddCompositeBinding("2DVector")
                .With("Up", "<Keyboard>/w")
                .With("Down", "<Keyboard>/s")
                .With("Left", "<Keyboard>/a")
                .With("Right", "<Keyboard>/d");
        }

        // Jump Action - Check if bindings already exist
        if (jumpAction == null || jumpAction.bindings.Count == 0) {
            jumpAction = new InputAction(type: InputActionType.Button);
            jumpAction.AddBinding("<Keyboard>/space");
        }

        // Run Action - Check if bindings already exist
        if (runAction == null || runAction.bindings.Count == 0) {
            runAction = new InputAction(type: InputActionType.Button);
            runAction.AddBinding("<Keyboard>/leftShift");
        }

        // Crouch Action - Check if bindings already exist
        if (crouchAction == null || crouchAction.bindings.Count == 0) {
            crouchAction = new InputAction(type: InputActionType.Button);
            crouchAction.AddBinding("<Keyboard>/leftCtrl");
        }
    }

    void Start() {
        cc = GetComponent<CharacterController>();
        playerCamera = Camera.main;
    }

    Vector3 velocity = new Vector3(0, 0, 0);

    void Update() {
        if (cc.isGrounded) {
            isJumping = false;
            velocity.y = 0;

            Vector3 movement = moveAction.ReadValue<Vector2>();
            velocity.x = movement.x * speed;
            velocity.z = movement.y * speed;

            if (runAction.ReadValue<float>() > 0 && !isCrouching) {
                velocity.x *= runMultiplier;
                velocity.z *= runMultiplier;
            }

            if (jumpAction.triggered) {
                velocity.y = Mathf.Sqrt(2 * jumpHeight * gravity);
                isJumping = true;
            }

            if (crouchAction.ReadValue<float>() > 0) {
                if (!isCrouching) {
                    cc.height = crouchHeight;
                    isCrouching = true;
                }
                velocity.x *= crouchSpeedMultiplier;
                velocity.z *= crouchSpeedMultiplier;
            } else {
                if (isCrouching) {
                    cc.height = normalHeight;
                    isCrouching = false;
                }
            }
        } else {
            Vector3 movement = moveAction.ReadValue<Vector2>();
            velocity.x = movement.x * speed;
            velocity.z = movement.y * speed;
            velocity.y -= gravity * Time.deltaTime;
        }

        velocity = transform.TransformDirection(velocity);
        cc.Move(velocity * Time.deltaTime);

        // Update the speed text
        if (speedText != null) {
            float currentSpeed = new Vector3(velocity.x, 0, velocity.z).magnitude;
            speedText.text = $"Speed: {currentSpeed:F2} m/s";
        }
    }
}