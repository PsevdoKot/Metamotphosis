using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Animations;

public class Mecro296 : Player
{
    [Header("Jumping")]
    private bool isGrounded;
    private bool wasOnGround;
    private bool isCeilingHitted; 
    [SerializeField] private Transform feetPos;
    [SerializeField] private Vector2 feetDetectorSize;
    [SerializeField] private float radius;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private Transform headPos;
    [SerializeField] private Vector2 headDetectorSize;
    //public GameObject holder;

    private bool isChargingJump = false;
    private bool isJumpCharged = false;
    private float jumpTimer;
    [SerializeField] private float jumpForce;
    [SerializeField] private float timeBetweenJump;
    [SerializeField] private float minJumpTime;
    [SerializeField] private float maxJumpTime;
    [SerializeField] private float jumpChargeSpeed;
    private float betweenJumpTimer = 0f;
    [SerializeField] private HealthBarBehaviour healthBarBehaviour;
    [SerializeField] private float healthBarChangeSpeed;

    private void Update() 
    {
        if (isActive)
        {
            if (isAbleToMove)
            {
                moveInput = Input.GetAxisRaw("Horizontal");

                wasOnGround = isGrounded;
                isGrounded = Physics2D.OverlapBox(feetPos.position, feetDetectorSize, 0f, groundMask);
                if (!wasOnGround && isGrounded)
                {
                    betweenJumpTimer = timeBetweenJump;
                    jumpTimer = 0f;
                    isJumpCharged = false;
                    //StartCoroutine(JumpSqueeze(1.15f, 0.8f, 0.05f));
                }

                isCeilingHitted = Physics2D.OverlapBox(headPos.position, headDetectorSize, 0f, groundMask);
                if (isCeilingHitted)
                    jumpTimer = 0;

                if (Input.GetButtonDown("Jump"))
                {
                    healthBarBehaviour.ToggleJumpSlider(true);
                }
                isChargingJump = Input.GetButton("Jump");
                if (isGrounded && jumpTimer < maxJumpTime && isChargingJump)
                {
                    jumpTimer += jumpChargeSpeed; 
                    healthBarBehaviour.AddValueToJumpSlider((maxJumpTime - minJumpTime) * jumpChargeSpeed * healthBarChangeSpeed);
                }
                if (isGrounded && Input.GetButtonUp("Jump"))
                {
                    isJumpCharged = true;
                    healthBarBehaviour.ToggleJumpSlider(false);
                    healthBarBehaviour.AddValueToJumpSlider(-1000);
                }
                if (isGrounded && jumpTimer > 0f)
                    healthBarBehaviour.ToggleJumpSlider(true);
                else
                    healthBarBehaviour.ToggleJumpSlider(false);

                UpdateMovementAnimation();
            }
            UpdateLedegGrabbing();
            if (isTouchingLedge)
                isGrounded = false;
        }
    }

    private void FixedUpdate()
    {
        if (isActive)
        {
            if ((betweenJumpTimer < 0f || isJumpCharged) && !isChargingJump)
            {
                if (!isJumpCharged)
                {
                    jumpTimer = minJumpTime;
                    isJumpCharged = true;
                }

                if (jumpTimer > 0f)
                {
                    Jump();
                    jumpTimer -= Time.deltaTime;
                }
            }
            betweenJumpTimer -= Time.deltaTime;
            Move();
        }
    }

    private void UpdateMovementAnimation()
    {
        anim.SetFloat("xVelocity", Mathf.Abs(rigidBody.velocity.x));
        anim.SetFloat("yVelocity", rigidBody.velocity.y);

        if (isGrounded)
        {
            anim.SetBool("isJumping", false);
            anim.SetBool("landingMoment", false);
        }
        else
            anim.SetBool("isJumping", true);
        if (!wasOnGround && isGrounded)
            anim.SetBool("landingMoment", true);

        anim.SetBool("isCeilingHitted", isCeilingHitted);

        anim.SetBool("isMoving", Mathf.Abs(moveInput) > 0f);

        Flip();
    }

    private void Flip()
    {
        if (moveInput > 0f)
        {
            transform.localRotation = Quaternion.Euler(0, 0, 0);
            isFacingRight = true;
        }
        else if (moveInput < 0f)
        {
            transform.localRotation = Quaternion.Euler(0, 180, 0);
            isFacingRight = false;
        }
    }

    protected override void Move()
    {
        rigidBody.velocity = new Vector2(isGrounded ? 0 : moveInput * moveSpeed, isCeilingHitted ? 0 : rigidBody.velocity.y);
    }

    public void Jump()
    {
        rigidBody.velocity = new Vector2(rigidBody.velocity.x, 0);
        rigidBody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        //StartCoroutine(JumpSqueeze(0.8f, 1.15f, 0.05f));
    }

    //IEnumerator JumpSqueeze(float xSqueeze, float ySqueeze, float seconds)
    //{
    //    Vector3 originSize = Vector3.one;
    //    Vector3 newSize = new Vector3(xSqueeze, ySqueeze, originSize.z);
    //    float timer = 0f;
    //    while (timer <= 1f)
    //    {
    //        timer += Time.deltaTime / seconds;
    //        holder.transform.localScale = Vector3.Lerp(originSize, newSize, timer);
    //        yield return null;
    //    }
    //    timer = 0;
    //    while (timer <= 1f)
    //    {
    //        timer += Time.deltaTime / seconds;
    //        holder.transform.localScale = Vector3.Lerp(newSize, originSize, timer);
    //        yield return null;
    //    }
    //}
}
