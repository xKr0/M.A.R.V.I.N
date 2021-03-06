﻿using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 6f;

    Vector3 movement;
    Animator anim;

    public bool movementRelative = false;



    Rigidbody playerRigidbody;
    int floorMask; // A layer mask so that a ray can be cast just at gameobjects on the floor
    float camRayLength = 100f;

    // Allow the player to move or not
    bool canMove;

    private void Awake()
    {
        //setup references
        floorMask = LayerMask.GetMask("Floor");
        anim = GetComponent<Animator>();

        // At the begining the player has to read the story
        ForbidToMove();
        playerRigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        // call on every physics update
        float h = Input.GetAxisRaw("Horizontal"); // getraw to have lees variation, 0 or 1 (more reaction because of full speed)
        float v = Input.GetAxisRaw("Vertical");

        // move the player on the scene
        Move(h, v);

        // to align the weapon to the move cursor
        Turning();

        // animate the player body
        Animating(h, v);
    }

    void Move(float h, float v)
    {   
        if (!canMove)
        {
            return;
        }
        if (!movementRelative)
        {
            // Movement in teh camera referentiel
            movement.Set(h, 0f, v);

            // normalize the movement, to avoid the problem of diagonali advantage
            movement = movement.normalized * speed * Time.deltaTime;

            // finaly move the player
            playerRigidbody.MovePosition(transform.position + movement);
        } else
        {
            // movement in the player referentiel
            if (Input.GetKey(KeyCode.Z))
            {
                transform.position += transform.forward * Time.deltaTime * speed;
            }
            else if (Input.GetKey(KeyCode.S))
            {
                transform.position -= transform.forward * Time.deltaTime * speed;
            }
            else if (Input.GetKey(KeyCode.Q))
            {
                transform.position -= transform.right * Time.deltaTime * speed;
            }
            else if (Input.GetKey(KeyCode.D))
            {
                transform.position += transform.right * Time.deltaTime * speed;
            }
        }       
    }

    void Turning()
    {
        // create a ray from the mouse curosor on screen in the direction of the camera
        Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        // get the information from where the ray is gonna hit the floor
        RaycastHit floorHit;

        if (Physics.Raycast (camRay, out floorHit, camRayLength, floorMask))
        {
            Vector3 playerToMouse = floorHit.point - transform.position;

            playerToMouse.y = 0f;

            // quaternion is a way of storing a rotation
            Quaternion newRotation = Quaternion.LookRotation(playerToMouse);

            playerRigidbody.MoveRotation(newRotation);
        }
    }

    void Animating(float h, float v)
    {
        bool walking = h != 0f || v != 0f;

        if (walking)
        {
            anim.SetFloat("Forward", 1.0f);
        } else
        {
            anim.SetFloat("Forward", 0.0f);
        }
        
    }

    public void AllowToMove()
    {
        canMove = true;
    }
    public void ForbidToMove()
    {
        canMove = false;
    }
}
