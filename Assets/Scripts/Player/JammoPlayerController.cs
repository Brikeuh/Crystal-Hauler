using UnityEngine;
using UnityEngine.InputSystem;

public class JammoPlayerController : MonoBehaviour
{
    InputAction moveAction;
    InputAction jumpAction;
    InputAction sprintAction;

    Vector3 moveValue;
    Vector2 moveValueInput;

    CharacterController characterController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        characterController = GetComponent<CharacterController>();

        moveAction = InputSystem.actions.FindAction("Player/Move");
        jumpAction = InputSystem.actions.FindAction("Player/Jump");
        sprintAction = InputSystem.actions.FindAction("Player/Sprint");
    }

    // Update is called once per frame
    void Update()
    {
        moveValueInput = moveAction.ReadValue<Vector2>();
        
        moveValue.x = moveValueInput.x;
        moveValue.z = moveValueInput.y;

        characterController.Move(moveValue * Time.deltaTime);
    }
}
