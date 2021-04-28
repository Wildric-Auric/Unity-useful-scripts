using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Serialization;
using UnityEngine.SceneManagement;


/* TO MAKE THIS SCRIPT WORK ASSIGN IT TO AN OBJECT WITH SPRITE RENDERER, COLLIDER, RIGIDBODY, MAKE ALSO TWO CHILDREN (empty objects), ONE POSITIONNED
 * BELOW THE SPRITE RENDERER AND ONE UPDWARD. ALSO MAKE SURE TO ASSIGN THE SCRIPT "Custom Inputs" TO ONE OBJECT IN HIERARCHY
 */
public class PlayerController2D : MonoBehaviour
{
    [Header("Horizontal speed")]
    Rigidbody2D rb;
    SpriteRenderer sr;
    Transform trans;
    float spd = 10f;
    [FormerlySerializedAs("Speed")]
    [SerializeField] float actualSpd = 10f; 
    [SerializeField] float acceleration = 0.4f;
    [SerializeField] float airAcceleration = 1f;
    [SerializeField] float deceleration = 0.3f;
    [SerializeField] float airDeceleration = 1f;
    [SerializeField] float smoothFlip = 0.3f; //Put a big value and sprite will flip instantly

    float movement;
    int movement1;

    [Header("Checkers")]
    [FormerlySerializedAs("CenterOfGroundChecker")]
    [SerializeField] Transform center;
    [SerializeField] float radiusOfGroundChecking = 0.1f;
    [SerializeField] float upRadius = 0.1f;
    [SerializeField] LayerMask groundMask; //Put the layer containing ground here
    [FormerlySerializedAs("CenterOfUpChcecker")]
    [SerializeField] Transform center1;

    [Header("Air Parameters")]
    [SerializeField] float RatioAirSpeed_GroundSpeed = 0.5f; //A value below one will make the air speed higher
    [SerializeField] float jumpSpd = 25f; //Force or speed of each jump
    float vspd = 2f;
    [SerializeField] float gravity = 45;
    [SerializeField] float jumpBuffer = 1f; //For late inputs, the higher it is, the late the player can press jump button and jump after not being in contact with ground
    [SerializeField] int jumpsNumber = 1; // double jump, triple jump etc...
    [SerializeField] bool variableJumpHeight = true;//The more you press the jump button, the higher you jump
    float actualJumpBuffer;
    
    bool upCollision;
    bool inAir;

    CustomInputs CI;

    int canJump;
    bool grounded;
    float initialSize; //for fliping chachter

    public bool isControlled = true;
    void Start()
    {
       
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        trans = GetComponent<Transform>();
        CI = FindObjectOfType<CustomInputs>();
        actualJumpBuffer = jumpBuffer;

        initialSize = trans.localScale.x;

    }

    void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.R)) { SceneManager.LoadScene(SceneManager.GetActiveScene().name);}
        grounded = Physics2D.OverlapCircle(center.position, radiusOfGroundChecking, groundMask);
        if (!grounded) { actualJumpBuffer -= Time.fixedDeltaTime; } else if (actualJumpBuffer != jumpBuffer) { actualJumpBuffer = jumpBuffer; }

        //Jump with late input system
        upCollision = Physics2D.OverlapCircle(center1.position, upRadius, groundMask);
        if ((actualJumpBuffer >= 0 || canJump > 0) && CI.Jump)
        {
            canJump -= 1;
            actualJumpBuffer = -1f;
            vspd = jumpSpd;
        }
        if ((upCollision || (!CI.jumpHold && variableJumpHeight)) && vspd > 0)  vspd = 0;

        vspd -= gravity * Time.fixedDeltaTime;

        if (actualJumpBuffer >= 0 && (!CI.Jump))
        {
            vspd = 0;
        }
        if (!grounded && !inAir)
        {
            inAir = true;
        }
        else if (inAir) inAir = false;
        else if (grounded) canJump = jumpsNumber;
        rb.transform.position += new Vector3(0, vspd * Time.fixedDeltaTime, 0);

        //Horizontal movement
        movement = Convert.ToInt16(CI.MoveRight) - Convert.ToInt16(CI.MoveLeft);
        if (movement != 0) movement1 = (int)Mathf.Sign(movement);
        //To accelerate gradually
        spd = Mathf.Clamp(Mathf.Pow(spd,3) + acceleration * movement, -actualSpd, actualSpd) 
            * (Convert.ToInt16(movement != 0 && grounded && (Mathf.Sign(movement) == Mathf.Sign(spd) || spd == 0))) // speed in ground

            + Mathf.Clamp(spd + airAcceleration * movement, -actualSpd / (RatioAirSpeed_GroundSpeed), actualSpd / (RatioAirSpeed_GroundSpeed)) 
            * (Convert.ToInt16(movement != 0 && !grounded && (Mathf.Sign(movement) == Mathf.Sign(spd) || spd == 0))) // speed in air

            +(spd - deceleration * (movement1)) * Convert.ToInt16(grounded && movement == 0 && spd != 0 && Mathf.Sign(spd) == movement1) //deceleration
            +(spd - airDeceleration * (movement1)) * Convert.ToInt16(!grounded && movement == 0 && spd != 0 && Mathf.Sign(spd) == movement1); //Air deceleration
        
        //Flip Charachter
        trans.localScale = new Vector3(Mathf.Clamp(trans.localScale.x + smoothFlip * movement1, -initialSize, initialSize), trans.localScale.y, trans.localScale.z);
        //Update position
        trans.position += new Vector3(spd * Time.fixedDeltaTime, 0);
    }
}
