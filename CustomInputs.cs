using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomInputs : MonoBehaviour
{
    public bool MoveRight;
    public bool MoveLeft;
    public bool Jump;
    public bool jumpHold;

    PlayerController2D Charachter;

    private void Start()
    {
        Charachter = FindObjectOfType<PlayerController2D>();
    }
    void Update()
    {
        MoveRight = (Input.GetKey("d") || Input.GetKey(KeyCode.RightArrow)&& Charachter.isControlled);
        MoveLeft = (Input.GetKey("q") || Input.GetKey("a") || Input.GetKey(KeyCode.LeftArrow) && Charachter.isControlled);
        Jump = (Input.GetKeyDown(KeyCode.Space)|| Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Z)) && Charachter.isControlled;
        jumpHold = (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.Z))&& Charachter.isControlled;
    }
}
