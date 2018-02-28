using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class walkY : MonoBehaviour {


    public float walkSpeed = 1.0f;      // Walkspeed
    public float wallUp = 0.0f;       // Define wallLeft
    public float wallDown = 5.0f;      // Define wallRight
    float walkingDirection = 1.0f;
    Vector2 walkAmount;
    float originalY; // Original float value


    void Start()
    {
        this.originalY = this.transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        walkAmount.y = walkingDirection * walkSpeed * Time.deltaTime;
        if (walkingDirection > 0.0f && transform.position.y >= wallDown)
        {
            walkingDirection = -1.0f;
        }
        else if (walkingDirection < 0.0f && transform.position.y <= wallUp)
        {
            walkingDirection = 1.0f;
        }
        transform.Translate(walkAmount);
    }
}

