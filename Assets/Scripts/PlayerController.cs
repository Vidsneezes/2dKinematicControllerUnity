﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public bool isRightPressed;
    public bool isLeftPressed;
    public bool wasJumpPressed;

    public float moveSpeed;
    public float gravity;
    public float startJumpSpeed;
    public LayerMask groundMask;
    [HideInInspector]
    public BoxCollider2D hitBox;
    [HideInInspector]
    public SpriteRenderer mainSprite;
    public bool onGround;


    public Vector2 velocity;
    private Vector2 direction;
    [HideInInspector]
    public Rigidbody2D rBody2d;
    private float actualMoveSpeed;
    [HideInInspector]
    public GameObject playerSpawnPoint;

    private void Awake()
    {
        rBody2d = GetComponent<Rigidbody2D>();
        mainSprite = GetComponent<SpriteRenderer>();
        hitBox = GetComponent<BoxCollider2D>();
    }

    // Use this for initialization
    void Start () {
        direction = Vector2.right;
	}
	
	// Update is called once per frame
	void Update () {

        isRightPressed = Input.GetKey(KeyCode.D);
        isLeftPressed = Input.GetKey(KeyCode.A);
        wasJumpPressed = Input.GetKeyDown(KeyCode.Space);


        if (isRightPressed)
        {
            direction.x = 1;
            mainSprite.flipX = false;
            actualMoveSpeed = moveSpeed;
        }else if (isLeftPressed)
        {
            direction.x = -1;
            mainSprite.flipX = true;
            actualMoveSpeed = moveSpeed;
        }
        else
        {
            actualMoveSpeed = 0;
        }

       

        if (wasJumpPressed && onGround)
        {
            velocity.y = startJumpSpeed;
            onGround = false;
        }

    }

    private void FixedUpdate()
    {
        Vector2 currentPosition = rBody2d.position;
        velocity.y -= gravity * Time.deltaTime;


        //horizontal check
        velocity.x = direction.x * actualMoveSpeed;
        float final_hor_vel = velocity.x;
   

        if (PlaceMeet(currentPosition.x + final_hor_vel * Time.deltaTime, currentPosition.y, groundMask))
        {
            Vector2 topRight = currentPosition;
            topRight.x += hitBox.size.x * 0.5f + final_hor_vel * Time.deltaTime;
            topRight.y += hitBox.size.y;
            RaycastHit2D pointIntersection = Physics2D.Raycast(topRight, Vector2.down, hitBox.size.y * 2f, groundMask);
            bool isSlope = false;
            if (pointIntersection.collider != null)
            {
                float angle = Vector2.Angle(Vector2.up, pointIntersection.normal);
                currentPosition.y = pointIntersection.point.y;
                Debug.Log(angle);
                if(angle > 44.8888f && angle < 89.8999f)
                {
                    currentPosition.x = pointIntersection.point.x - (hitBox.size.x * 0.5f);
                    currentPosition.y = pointIntersection.point.y;
                    isSlope = true;
                    final_hor_vel = 0;
                }
                if(angle < 1)
                {
                    currentPosition.x = pointIntersection.point.x - (hitBox.size.x * 0.5f);
                    currentPosition.y = pointIntersection.point.y;
                }
            }

            if (!isSlope)
            {
                while (!PlaceMeet(currentPosition.x + ((final_hor_vel * Time.deltaTime) / 8), currentPosition.y, groundMask))
                {
                    currentPosition.x += ((final_hor_vel * Time.deltaTime) / 8);
                }
                final_hor_vel = 0;
            }
        }
        currentPosition.x += final_hor_vel * Time.deltaTime;


        //vertical check
        float final_ver_vel = velocity.y;
        if (PlaceMeet(currentPosition.x, currentPosition.y + final_ver_vel * Time.deltaTime, groundMask))
        {
            while (!PlaceMeet(currentPosition.x, currentPosition.y + ((final_ver_vel * Time.deltaTime) / 8), groundMask))
            {
                currentPosition.y += ((final_ver_vel * Time.deltaTime) / 8);
            }
            final_ver_vel = 0;
        }
        currentPosition.y += final_ver_vel * Time.deltaTime;

        rBody2d.MovePosition(currentPosition);

        if (PlaceMeet(currentPosition.x, currentPosition.y + (1f / 8f), groundMask))
        {
            velocity.y = 0;
        }

        if (PlaceMeet(currentPosition.x, currentPosition.y - (1f / 8f),groundMask))
        {
            velocity.y = 0;
            onGround = true;
        }
        else
        {
            onGround = false;
        }


    }

    private bool PlaceMeet(float xPos, float yPos, LayerMask mask)
    {
        Vector2 position = new Vector2(xPos+hitBox.offset.x, yPos+hitBox.offset.y);
        Collider2D[] collider2d = Physics2D.OverlapBoxAll(position , hitBox.size, 0, mask);
        return collider2d.Length > 0;
    }

}
