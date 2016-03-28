﻿using UnityEngine;
using System.Collections;

public class SoccerBall : MonoBehaviour {
    Rigidbody2D rb;
    Rigidbody ballrb;
    float max_speed = 50;
    public static GameObject Ball;
    // Use this for initialization
    void Awake() {
        Ball = gameObject;
    }
    void Start() {
        rb = GetComponent<Rigidbody2D>();
        ballrb = transform.GetChild(0).gameObject.GetComponent<Rigidbody>();
    }

    Vector3 WayToGo;

    void FixedUpdate() {
        WayToGo.x = rb.velocity.y;
        WayToGo.y = -rb.velocity.x;
        ballrb.angularVelocity = WayToGo;
    }

    // Update is called once per frame
    void Update() {
        while (rb.velocity.magnitude > max_speed) {
            rb.velocity *= 0.99f;
        }

    }
}