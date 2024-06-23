using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class PlayerScript : Singleton<PlayerScript>
{
    //Components
    [Header("COMPONENTS")]
    [SerializeField] private Animator anim;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private VariableJoystick variableJoystick;
    [SerializeField] private Transform footTransform;
    [SerializeField] private Transform leftWallCheckTransform, rightWallCheckTransform;
    [SerializeField] private LayerMask layersCanJump;
    [SerializeField] private LayerMask layersCanWallJump;

    //Variables
    [Space(20)]
    [Header("VARIABLES")]
    [SerializeField] private float moveSpeed = 10;
    [SerializeField] private float jumpForce = 16;
    [SerializeField] private float dashSpeed = 1;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCoolDown = 1;
    [SerializeField] private float maxFallSpeed = -25f;
    [SerializeField] private float floatingSpeed = 8;
    [SerializeField] private float floatingSpeedUpward = 5;
    [SerializeField] private float wallCheckDistance = 0.12f;
    [SerializeField] private float wallSlideSpeed = 2f;
    [SerializeField] private float wallJumpForceX = 3;
    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] private float lowJumpMultiplier = 2f;
    [SerializeField] private float coyoteTime = 0.2f;
    [SerializeField] private float jumpBufferingTime = 0.2f;
    [SerializeField] private float interactRadius = 0.5f;

    private bool isInDialogue = false;
    private float moveX;
    private float coyoteTimeCounter;
    private float jumpBufferingCounter;
    private float fallSpeedDampingChangeThreshold;
    private Vector2 moveVector = Vector2.zero;
    private Vector2 jumpVector = Vector2.zero;
    private bool isFacingLeft = false, isFacingRight = true;
    private bool isDashing = false;
    private bool canDash = true;
    private bool hasDoubleJumped = true;
    private bool hasWallJumped = false;
    private bool onWallJumpCooldown = false;
    private bool isStillPressingJump = false;
    private bool rayLeftCheck, rayRightCheck;
    private bool isJumping = false;
    private bool isFalling = false;
    private bool isWallSliding = false;
    private bool isDead = false;
    private Transform checkpoint;
    private CustomInput input = null;
    int wallDirection;
    string currentAnimationName = string.Empty;

    [Space(10)]
    [Header("SFXs")]
    [SerializeField] private AudioClip jumpSFX;
    [SerializeField] private AudioClip deadSFX;
    [SerializeField] private AudioClip powerUpSFX;

    //PowerUpUnlock booleans
    [Space(10)]
    [Header("Power-ups")]
    [SerializeField] private bool isUnlockedDoubleJump = false;
    [SerializeField] private bool isUnlockedDash = false;
    [SerializeField] private bool isUnlockedFloating = false;
    [SerializeField] private bool isUnlockedWallSliding = false;

    //Interact variables
    GameObject interactingObject = null;
    Collider2D[] interactableObjectArray;
    private bool isInteracting = false;

    //Float variables
    private bool isFloating = false;
    private bool isInUpwardWindZone = false;
    private float initialDrag;
    private float initialMoveSpeed;
    
    void Awake()
    {
        initialDrag = rb.drag;
        initialMoveSpeed = moveSpeed;
        input = new CustomInput();    
        OnEnable();

        fallSpeedDampingChangeThreshold = CameraManager.Instance.fallSpeedYDampingChangeThreshold;
    }
    
    void Update()
    {
        LimitFallingSpeed();
        FallYDamping();
        //EdgeDetecting();
        LerpToCheckpoint();
        Jump();

        if((isFloating && IsGrounded()))
            DisableFloat();

        WallSliding();
        if(input.Player.Dash.IsPressed() && canDash)
            StartDash();

        HandleAnimation();
    }

    void HandleAnimation()
    {
        isFalling = !IsGrounded() && rb.velocity.y < -0.5f && isUnlockedWallSliding && !IsTouchingWall();
        isWallSliding = !IsGrounded() && IsTouchingWall() && isUnlockedWallSliding;

        anim.SetFloat("horizontalSpeed", Mathf.Abs(variableJoystick.Horizontal));
        anim.SetBool("isGrounded", IsGrounded());
        anim.SetBool("isFalling", isFalling);
        anim.SetBool("isWallSliding", isWallSliding);
        anim.SetBool("isJumping", isJumping);
    }

    void FixedUpdate()
    {
        Move();
        FloatUp();
    }

    // void ToggleOnAnimation(string name)
    // {
    //     //Make this method play once only
    //     if(name == currentAnimationName)
    //         return;

    //     Debug.Log(name);

    //     //Toggle off all of the animation
    //     for(int i = 0; i < anim.parameterCount; i++)
    //         anim.SetBool(anim.GetParameter(i).name, false);
        
    //     //Toggle on the input animation
    //     anim.SetBool(name, true);
    //     currentAnimationName = name;
    // }

    void Move()
    {
        //If the player in dialogue, stop the player
        if(isInDialogue)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        //Prioritize Dash and Wall Jump than Move
        if(isDashing || hasWallJumped)
            return;

        rb.velocity = new Vector2(variableJoystick.Horizontal * moveSpeed, rb.velocity.y);

        if(variableJoystick.Horizontal > 0 && isFacingLeft)
        {
            SetFacingRight();
            CameraFollowPoint.Instance.CallTurn(isFacingRight);
        }
        else if(variableJoystick.Horizontal < 0 && isFacingRight)
        {
            SetFacingLeft();
            CameraFollowPoint.Instance.CallTurn(isFacingRight);
        }
    }

    public void StartDash()
    {
        //If the player is not unlocking Dash, or is in dialogue, then not perform Dash
        if(!isUnlockedDash || isInDialogue)
            return;

        if (!isDashing && canDash)
        {
            DisableFloat();
            StartCoroutine(Dash());
        }
    }

    private IEnumerator Dash()
    {
        //Perform Dash
        isDashing = true;
        canDash = false;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0;
        rb.velocity = new Vector2((isFacingRight ? dashSpeed : -dashSpeed), 0); //Calculate Dash direction using inline
        yield return new WaitForSeconds(dashDuration);

        //Reset state
        rb.gravityScale = originalGravity;
        isDashing = false;
        yield return new WaitForSeconds(dashCoolDown); //Cooldown
        canDash = true;
    }

    void SetFacingRight()
    {
        transform.localRotation = new Quaternion(0, 0, 0, 0);
        isFacingRight = true;
        isFacingLeft = false;
    }

    void SetFacingLeft()
    {
        transform.localRotation = new Quaternion(0, 180, 0, 0);
        isFacingRight = false;
        isFacingLeft = true;
    }

    public void Jump()
    {
        if(isDashing || isInDialogue)
            return;

        CheckCoyoteTime();
        CheckJumpBuffering();

        //Jump                  touch the ground and still in the jumpBuffering phase
        //If                    still in coyoteTime phase and just press Jump key   |or| Double Jump is unlocked and have not Double Jump and just press Jump key
        if(!IsTouchingWall() && ((coyoteTimeCounter > 0 && jumpBufferingCounter > 0) || isUnlockedDoubleJump && !hasDoubleJumped && input.Player.Jump.IsPressed()))
        {
            //If still hold down the Jump key, then not perform the Jump
            if(isStillPressingJump)
                return;

            //Set the bool to not perform the Jumps repeatedly if the player is holding the Jump key.
            isStillPressingJump = true;

            //if the player presses Jump key while floating, perform the second Jump only
            if(isFloating)
            {
                hasDoubleJumped = true;
                DisableFloat();
            }
            
            //Add Jump force
            rb.velocity = Vector2.zero;
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
            isJumping = true;
            
            //Logics to perform the second Jump
            if(!IsGrounded() && coyoteTimeCounter <= 0)
                hasDoubleJumped = true;

            //Avoid the player performs too many jumps if the player press the Jump key too fast
            coyoteTimeCounter = 0;

            //To check if the player has released the Jump key
            if(jumpVector.y == 0 && isStillPressingJump)
                isStillPressingJump = false;

            //ToggleOnAnimation("isJumping");
            AudioManager.Instance.PlaySFX(jumpSFX);
        }
        //Logics to perform Wall Jump
        else if(IsTouchingWall() && input.Player.Jump.IsPressed() && isUnlockedWallSliding && !isStillPressingJump)
        {
            if(isStillPressingJump)
                return;
            
            if(onWallJumpCooldown)
                return;

            isStillPressingJump = true;
            WallJump();

            coyoteTimeCounter = 0;

            if(jumpVector.y == 0 && isStillPressingJump)
                isStillPressingJump = false;

            AudioManager.Instance.PlaySFX(jumpSFX);
        }
        
        VariableJumpHeight();

        //If released the Jump key then reset the flag to false.
        if(jumpVector.y == 0 && isStillPressingJump)
            isStillPressingJump = false;
    }

    #region MovementAddOns
    void CheckCoyoteTime()
    {
        //If the player is touching the ground, reset the coyoteTimeCounter
        //Allow the player to still perform the first jump after a short time of leaving the ground
        if(IsGrounded())
        {
            coyoteTimeCounter = coyoteTime;
            hasDoubleJumped = false;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }
    }

    void CheckJumpBuffering()
    {
        if(IsGrounded())
            jumpBufferingCounter = 0;

        //If the player presses the Jump key, reset the jumpBufferingCounter
        //Allow the player to still perform the jump even at a short distance with the ground
        if(jumpVector.y > 0)
            jumpBufferingCounter = jumpBufferingTime;
        else
            jumpBufferingCounter -= Time.deltaTime;
    }

    void VariableJumpHeight()
    {
        //If the player is falling while holding Jump key, the character will fall slower.
        if (rb.velocity.y < 0)
            rb.velocity += Vector2.up * Physics2D.gravity.y * fallMultiplier * Time.fixedDeltaTime;
        else if (rb.velocity.y > 0 && jumpVector.y == 0)
            rb.velocity += Vector2.up * Physics2D.gravity.y * lowJumpMultiplier * Time.fixedDeltaTime;
    }

    void EdgeDetecting()
    {
        if(IsGrounded() || IsTouchingWall())
            return;

        var hit = Physics2D.OverlapBox(transform.position, transform.localScale, 0);

        if(hit != null && hit.gameObject.layer == 6)
        {
            //Horizontal
            if(rb.velocity.y < 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, 0);

                var dir1 = transform.position - hit.transform.position;
                transform.position += dir1.normalized * moveVector.magnitude * Time.deltaTime;
            }

            //Vertical
            if(rb.velocity.y > 0)
            {
                rb.velocity = new Vector2(0, rb.velocity.y);

                var dir2 = transform.position - hit.transform.position;
                transform.position += dir2.normalized * moveVector.magnitude * Time.deltaTime;
            }
        }
    }
    #endregion

    #region WallJump_WallSlide
    void WallSliding()
    {
        if(isDashing)
            return;

        if(IsTouchingWall() && isUnlockedWallSliding)
        {
            isWallSliding = true;
            //Disable floating state to switch to wall sliding state
            if(isFloating)
                DisableFloat();
            
            //Allow the player to still perform 1 more jump after leaving the walls for a very short time
            coyoteTimeCounter = coyoteTime;

            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlideSpeed, float.MaxValue));

            //Allow the player to perform the double jump after leaving the wall
            if(hasDoubleJumped && !hasWallJumped)
                hasDoubleJumped = false;

            //ToggleOnAnimation("isWallSliding");
        }
        else
            isWallSliding = false;
    }

    void WallJump()
    {
        //ToggleOnAnimation("isJumping");
        //Set the bool to true to disable the Move() function, 
        //else the player's velocity is set immediately to moveSpeed
        hasWallJumped = true;
        onWallJumpCooldown = true;
        
        //Reset the bool to enable the Move() function again
        StartCoroutine(ResetWallJumpFlag());
        
        //Return the direction to perform Wall Jump
        if(isFacingLeft)
            wallDirection = 1;
        else if(isFacingRight)
            wallDirection = -1;

        rb.velocity = Vector2.zero;
        rb.velocity = new Vector2(wallJumpForceX * wallDirection, jumpForce *0.95f);
    }

    IEnumerator ResetWallJumpFlag()
    {
        yield return new WaitForSeconds(0.3f);
        hasWallJumped = false;
        yield return new WaitForSeconds(0.3f);
        onWallJumpCooldown = false;
    }
    #endregion

    void Interact(InputAction.CallbackContext context)
    {
        Interact();
    }

    public void Interact()
    {
        //If the player press interact button while in dialogue, then Interact Button acts as Next Button
        if(isInDialogue)
            DialogueManager.Instance.NextButton();

        //If the player is interacting, then not perform Interact
        if(isInteracting)
            return;

        interactableObjectArray = Physics2D.OverlapCircleAll(transform.position, interactRadius, 1<<8);

        if(interactableObjectArray.Length > 0)
        {
            //Get the closest object that is interactable (belongs to InteractableObjects layer)
            float distance = Mathf.Infinity;

            for(int i = 0; i < interactableObjectArray.Length; i++)
            {
                if(Vector2.Distance(transform.position, interactableObjectArray[i].transform.position) < distance)
                {
                    distance = Vector2.Distance(transform.position, interactableObjectArray[i].transform.position);
                    interactingObject = interactableObjectArray[i].gameObject;
                }
            }

            isInteracting = true;

            //Call Interact() method
            IInteractable interactable = interactingObject.GetComponent<IInteractable>();
            interactable.Interact();
        }
    }

    #region Falling_Floating
    void Float(InputAction.CallbackContext context)
    {
        Float();
    }
    
    public void Float()
    {
        if(!isUnlockedFloating || isDashing || isInDialogue)
            return;

        if(!isFloating)
        {
            isFloating = true;
            
            //Increase the linear drag
            initialDrag = rb.drag;
            rb.drag = 20f;

            //Change the speed
            initialMoveSpeed = moveSpeed;
            moveSpeed = floatingSpeed;
        }
        else
            DisableFloat();
    }

    void DisableFloat()
    {
        isFloating = false;

        //Set stats to be the same as initial values
        rb.drag = initialDrag;
        moveSpeed = initialMoveSpeed;
    }

    void FloatUp()
    {
        if(isDashing || isInDialogue)
            return;

        if(isFloating && isInUpwardWindZone)
            rb.velocity = new Vector2(rb.velocity.x, floatingSpeedUpward);
    }

    void LimitFallingSpeed()
    {
        //Limit the falling speed
        if(rb.velocity.y <= maxFallSpeed)
            rb.velocity = new Vector2(rb.velocity.x, maxFallSpeed);
    }

    void FallYDamping()
    {
        //If the player is falling, reveal more space underneath the player
        if(rb.velocity.y < fallSpeedDampingChangeThreshold && !CameraManager.Instance.IsLerpingYDamping && !CameraManager.Instance.LerpedFromPlayerFalling)
            CameraManager.Instance.LerpYDamping(true);
        
        if(rb.velocity.y >= 0f && !CameraManager.Instance.IsLerpingYDamping && CameraManager.Instance.LerpedFromPlayerFalling)
        {
            CameraManager.Instance.LerpedFromPlayerFalling = false;
            CameraManager.Instance.LerpYDamping(false);
        }
    }
    #endregion

    void UnlockPowerUp(PowerUp powerUp)
    {
        //Check the type of power-up to award the player
        switch(powerUp)
        {
            case PowerUp.DoubleJump:
                isUnlockedDoubleJump = true;
                break;
            case PowerUp.Dash:
                isUnlockedDash = true;
                break;
            case PowerUp.Floating:
                isUnlockedFloating = true;
                break;
            case PowerUp.WallSliding:
                isUnlockedWallSliding = true;
                break;
        }

        isInteracting = false;
        interactingObject = null;

        AudioManager.Instance.PlaySFX(powerUpSFX);
    }

    bool IsGrounded() //Ground check
    {
        return Physics2D.OverlapBox(footTransform.position, new Vector2(footTransform.localScale.x, footTransform.localScale.y), 0f, layersCanJump);
    }

    bool IsTouchingWall() //Wall check
    {
        if(!isUnlockedWallSliding || isDashing)
            return false;

        if(isFacingRight)
        {
            rayLeftCheck = Physics2D.Raycast(leftWallCheckTransform.position, Vector2.left, wallCheckDistance, layersCanWallJump);
            rayRightCheck = Physics2D.Raycast(rightWallCheckTransform.position, Vector2.right, wallCheckDistance, layersCanWallJump);
        }
        else if(!isFacingRight)
        {
            rayLeftCheck = Physics2D.Raycast(leftWallCheckTransform.position, Vector2.right, wallCheckDistance, layersCanWallJump);
            rayRightCheck = Physics2D.Raycast(rightWallCheckTransform.position, Vector2.left, wallCheckDistance, layersCanWallJump);
        }
        if(!IsGrounded() && (rayLeftCheck || rayRightCheck))
            isJumping = false;

        return (!IsGrounded() && (rayLeftCheck || rayRightCheck));
    }

    #region Checkpoint
    public void SetCheckpoint(Transform cp) => checkpoint = cp;

    void LerpToCheckpoint()
    {
        if(isDead)
            transform.position = Vector3.Lerp(transform.position, checkpoint.position, 10 * Time.deltaTime);
    }
    #endregion

    public void TakeDamage()
    {
        StartCoroutine(TakeDamageCoroutine());
    }

    IEnumerator TakeDamageCoroutine()
    {
        AudioManager.Instance.PlaySFX(deadSFX);
        CameraManager.Instance.ShakeCamera();
        anim.SetBool("isDead", true);

        rb.isKinematic = true;
        rb.simulated = false;
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0.25f);
        isDead = true;
        input.Disable();
        DisableFloat();
        
        yield return new WaitForSeconds(0.75f);
        
        anim.SetBool("isDead", false);
        input.Enable();
        rb.velocity = Vector2.zero;
        isDead = false;
        rb.simulated = true;
        rb.isKinematic = false;
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1f);
    }

    public void SetIsInteractingFlag(bool newBool) => isInteracting = newBool;
    
    public void SetIsInDialogueFlag(bool newBool) => isInDialogue = newBool;

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log(collision.GetContact(0).normal.y);
        if (isJumping && collision.GetContact(0).normal.y > 0f)
        {
            
            isJumping = false;
            anim.SetBool("isJumping", false);
        }
    }

    void OnEnable()
    {
        PowerUpUnlocker.onUnlockPowerUp += UnlockPowerUp;

        //Movement
        input.Enable();
        input.Player.Float.performed += Float;
        input.Player.Interact.performed += Interact;
        input.Player.Jump.started += OnJumpPerformed;
        input.Player.Jump.canceled += OnJumpCanceled;
        input.Player.Movement.performed += OnMovementPerformed;
        input.Player.Movement.canceled += OnMovementCanceled;
    }

    void OnDisable()
    {
        PowerUpUnlocker.onUnlockPowerUp -= UnlockPowerUp;

        //Movement
        input.Disable();
        input.Player.Float.performed -= Float;
        input.Player.Interact.performed -= Interact;
        input.Player.Jump.performed -= OnJumpPerformed;
        input.Player.Jump.canceled -= OnJumpCanceled;
        input.Player.Movement.performed -= OnMovementPerformed;
        input.Player.Movement.canceled -= OnMovementCanceled;
    }

    void OnMovementPerformed(InputAction.CallbackContext context) => moveVector = context.ReadValue<Vector2>();

    void OnMovementCanceled(InputAction.CallbackContext context) => moveVector = Vector2.zero;

    void OnJumpPerformed(InputAction.CallbackContext context) => jumpVector = context.ReadValue<Vector2>();

    void OnJumpCanceled(InputAction.CallbackContext context) => jumpVector = Vector2.zero;

    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(footTransform.position, new Vector3(footTransform.localScale.x, footTransform.localScale.y, 0));
        Gizmos.DrawWireSphere(transform.position, interactRadius);
        Debug.DrawRay(leftWallCheckTransform.position, Vector3.left * wallCheckDistance, Color.red);
        Debug.DrawRay(rightWallCheckTransform.position , Vector3.right * wallCheckDistance, Color.red);
    }

    public bool IsInteracting { get{ return isInteracting; } set{ isInteracting = value; } }
    public bool IsInUpwardWindZone{ set{ isInUpwardWindZone = value; } }
}

