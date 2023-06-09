using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterMovement : MonoBehaviour
{
    [Header("Target Objects")]
    public Rigidbody avatar;


    [Header("Move Variables")]
    public Vector2 move;
    public float moveVertical;
    public float moveHorizontal;
    public float moveInputAmount;
    public Vector3 moveDirection;
    public float moveRotateSpeed;
    public float moveSpeed;

    [Header("Jump Variables")]
    public bool jumpHeld;
    public float jumpHeight;
    public float jumpVelocity;
    public float fallMultiplier;
    public float lowJumpMultiplier;
    public float jumpTime;
    public float jumpTimeRemains;

    // Start is called before the first frame update
    void Start()
    {
        avatar = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        Move();
        Jump();
    }
    
    public void OnMove(InputAction.CallbackContext context)
    {
        move = context.ReadValue<Vector2>();
    }
    
    public void OnJump(InputAction.CallbackContext context)
    {
        jumpHeld = context.ReadValueAsButton();
    }

    public bool isGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, 0.1f);
    }

    private void Move()
    {
        float moveVertical = move.y;
        float moveHorizontal = move.x;

        Vector3 correctedCameraForward = Camera.main.transform.forward;
        correctedCameraForward.y = 0;
        correctedCameraForward.Normalize();
        Vector3 correctedVertical = moveVertical * correctedCameraForward;
        Vector3 correctedHorizontal = moveHorizontal * Camera.main.transform.right;
        Vector3 combinedInput = correctedHorizontal + correctedVertical;
        float inputMagnitude = Mathf.Abs(moveHorizontal) + Math.Abs(moveVertical);
        float moveInputAmount = Mathf.Clamp01(inputMagnitude);
        Vector3 moveDirection = new Vector3((combinedInput).normalized.x, 0, (combinedInput).normalized.z);

        if (moveDirection.sqrMagnitude > float.Epsilon)
        {
            Quaternion rot = Quaternion.LookRotation(moveDirection);
            Quaternion targetRotation = Quaternion.Slerp(transform.rotation, rot, Time.fixedDeltaTime * moveInputAmount * moveRotateSpeed);
            transform.rotation = targetRotation;
        }

        avatar.position += transform.forward * moveInputAmount * moveSpeed / 1000f;
    }
    private void Jump()
    {
        if (jumpTimeRemains > 0)
        {
            if (jumpHeld)
            {
                avatar.velocity += Vector3.up * jumpVelocity;
                jumpTimeRemains -= Time.deltaTime;
            }
            else
            {
                jumpTimeRemains = jumpTime;
            }
        }
        else
        {
            jumpHeld = false;
            jumpTimeRemains = jumpTime;
        }

        if (avatar.velocity.y < 0)
        {
            avatar.velocity += Vector3.up * Physics.gravity.y * 10 * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (avatar.velocity.y > 0 && !jumpHeld)
        {
            avatar.velocity += Vector3.up * Physics.gravity.y * 10 * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }
}

