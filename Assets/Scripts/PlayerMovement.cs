using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Vars to eventually be hard coded by character
    [SerializeField] private float jumpPower = 20f;
    [SerializeField] private float shortHopPower = 16f;
    [SerializeField] private float speed = 23f;
    [SerializeField] private float dashPower = 1.5f;


    private Rigidbody2D playerRB;
    private Animator anim;
    private SpriteRenderer sprite;
    private BoxCollider2D coll;

    [SerializeField] private LayerMask jumpableGround;
    
    private float dirX = 0f;
    private float lastDirX = 0f;
    private float activeSpeed;
    MovementState state;
    DashState dashState;

    private enum MovementState { 
        idle, 
        running, 
        jumping, 
        falling, 
        jumpsquat, 
        dashing 
    } 

    private enum DashState {
        still,
        right,
        left
    }

    [SerializeField] private AudioSource jumpSound;

    // Start is called before the first frame update
    private void Start()
    {
        playerRB = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        coll = GetComponent<BoxCollider2D>();
        state = MovementState.idle;
        dashState = DashState.still;
        activeSpeed = speed;
    }

    // Update is called once per frame
    private void Update()
    {
        // Use GetAxisRaw() to prevent player sliding after no imput
        lastDirX = dirX;
        dirX = Input.GetAxis("Horizontal");

        // If player 'slaps' stick then dash
        if(dirX - lastDirX > 0.5 && 
            isGrounded() && 
            state != MovementState.jumpsquat && 
            dashState == DashState.still)
        { ///Need to make dash stop when slide off plat
            activeSpeed = speed * dashPower;
            state = MovementState.dashing;
            dashState = DashState.right;
            sprite.flipX = false;
        }
        else if (dirX - lastDirX < -0.5 && 
            isGrounded() && 
            state != MovementState.jumpsquat &&
            dashState == DashState.still)
        {
            activeSpeed = speed * dashPower;
            state = MovementState.dashing;
            dashState = DashState.left;
            sprite.flipX = true;
        }

        // If mid-dash keep applying force 
        if (dashState == DashState.right)
        {
            dirX = 0.7f;
        }
        if (dashState == DashState.left)
        {
            dirX = -0.7f;
        }

        playerRB.velocity = new Vector2(dirX * activeSpeed, playerRB.velocity.y);

        if (Input.GetButtonDown("Jump") && isGrounded())
        {
            /// Major bug where animation gets stuck during dashing->jumpsquat transition
            resetSpeed();
            state = MovementState.jumpsquat;
        }
        if(state != MovementState.jumpsquat && state != MovementState.dashing)
        {
            state = updateAnimationState();
        }
        Debug.Log(state);
        anim.SetInteger("state", (int)state);
    }
    
    // Referenced in Player_Jumpsquat animation event
    private void applyJump()
    {
        jumpSound.Play();
        if (Input.GetButton("Jump"))
        {
            playerRB.velocity = new Vector2(0, jumpPower);
        }
        else
        {
            playerRB.velocity = new Vector2(0, shortHopPower);
        }
        state = MovementState.jumping;
    }

    // Referenced in Player_Dash animation event
    private void resetSpeed()
    {
        activeSpeed = speed;
        state = MovementState.running;
        dashState = DashState.still;
    }

    private MovementState updateAnimationState()
    {
        MovementState getState;
        if (dirX > 0f)
        {
            getState = MovementState.running;
            if(isGrounded())
            {
                sprite.flipX = false;
            }
        }
        else if (dirX < 0f)
        {
            getState = MovementState.running;
            if (isGrounded())
            {
                sprite.flipX = true;
            }
        }
        else
        {
            getState = MovementState.idle;
        }

        if (playerRB.velocity.y > 0.1f)
        {
            getState = MovementState.jumping;
        } 
        else if(playerRB.velocity.y < -0.1f)
        {
            getState = MovementState.falling;
        }

        return getState;
    }

    private bool isGrounded()
    {
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, 0.1f, jumpableGround);
    }
}
