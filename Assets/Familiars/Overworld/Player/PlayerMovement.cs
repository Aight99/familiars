using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float runSpeed = 5f;

    [SerializeField]
    private float rotationSpeed = 5f;

    private InputActions.PlayerActions playerControls;
    private CharacterController controller;
    private Vector3 runDirection;

    private Vector3 currentVelocity;
    private readonly float gravityValue = -9.81f;

    private void Awake()
    {
        playerControls = new InputActions().Player;
        controller = GetComponent<CharacterController>();
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    private void Update()
    {
        ReadInputs();
        HandleRun();
    }

    private void ReadInputs()
    {
        var runInput = playerControls.Move.ReadValue<Vector2>();
        runDirection = new Vector3(runInput.x, 0, runInput.y);
    }

    private void HandleRun()
    {
        controller.Move(runSpeed * Time.deltaTime * runDirection);

        if (runDirection != Vector3.zero)
        {
            var targetRotation = Quaternion.LookRotation(runDirection, Vector3.up);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }

        var isGrounded = controller.isGrounded;
        if (isGrounded && currentVelocity.y < 0)
        {
            currentVelocity.y = 0f;
        }

        currentVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(currentVelocity * Time.deltaTime);
    }
}
