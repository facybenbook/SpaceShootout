﻿using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
    float DRIBBLE_MAGNITUDE_THRESHOLD = 0.8f;
    Color PLAYER_1_COLOR = Color.red;
    Color PLAYER_2_COLOR = Color.blue;
    public bool RedTeam = true;
    struct controls {
        public string up, vert, hor,R_vert, R_hor;
        public string skate, Shoot, special;
    };

    public int my_number = 1;
    public float ball_offset_scale;
    public bool has_ball = false;
    public bool is_goalie = false;

    float max_speed = 5;
    float acceleration = 30;
    float dash_delay_time = 0.3f;
    float dash_delay = 0;
    float slow_delay_time = 1f;
    float slow_delay = 0;
   public float shot_force = 1000f;
    Vector2 current_ball_angle;

    int slowed = 0;
    float fatigue = 0;
    bool dash = false;

    GameObject ball;
    Rigidbody2D ball_rb;
    controls my_inputs;
    Rigidbody2D rigid;
    CircleCollider2D player_collider;
    CircleCollider2D ball_collider;
    Color default_color;

    void Start() {
        ball = SoccerBall.Ball;
        ball_rb = ball.GetComponent<Rigidbody2D>();
        player_collider = GetComponent<CircleCollider2D>();
        ball_collider = ball.GetComponent<CircleCollider2D>();

        my_inputs.vert = string.Format("Vertical{0}", my_number);
        my_inputs.hor = string.Format("Horizontal{0}", my_number);
        my_inputs.R_hor = string.Format("R_Horizontal{0}", my_number);
        my_inputs.R_vert = string.Format("R_Vertical{0}", my_number);
        my_inputs.skate = string.Format("Skate{0}", my_number);
        my_inputs.Shoot = string.Format("Shoot{0}", my_number);
        my_inputs.special = string.Format("Special{0}", my_number);
        rigid = gameObject.GetComponent<Rigidbody2D>();

        if (my_number == 1) {
            GetComponent<SpriteRenderer>().color = PLAYER_1_COLOR;
        } else {
            GetComponent<SpriteRenderer>().color = PLAYER_2_COLOR;
        }
        
        default_color = GetComponent<Renderer>().material.color;
    }

    void Update() {
        if (!HUD.S.GameStarted) {
            return;
        }
        checkDash();
        updateMovement();
        /*
        if (slowed == 0) {
            GetComponent<Renderer>().material.color = default_color;
        } else if(slowed == 1) {
            GetComponent<Renderer>().material.color = Color.yellow;
        } else if (slowed == 2) {
            GetComponent<Renderer>().material.color = Color.yellow/2+Color.red/2;
        }else if(slowed == 3) {
            GetComponent<Renderer>().material.color = Color.red;
        }
        */
       
        if (has_ball == true) {
            dribble();
        }
        if (getInputFire() && has_ball) {
            shoot();
        }
    }

    void updateMovement() {
        Vector2 move_vector = getInputMovementVector();
        move_vector *= acceleration;

        if (dash == true) {
            HUD.S.PlaySound("woosh", Random.Range(.5f, 1f));
            move_vector *= 20;
            dash = false;
        }/*
        if (slowed >0) {
            rigid.velocity = rigid.velocity/(1+slowed*0.2f);
        }
          */
        rigid.AddForce(move_vector / (1 + slowed * 0.75f));
        if (rigid.velocity.magnitude > max_speed && dash_delay < dash_delay_time * 0.8f) {
            rigid.velocity *= 0.97f;
        }
    }

    public void gainControlOfBall() {
        HUD.S.PlaySound("dribble", Random.Range(.5f,1f));
        ball.transform.parent = transform;
        has_ball = true;
        current_ball_angle = (ball.transform.position - transform.position).normalized;
        dribble();
        ball_rb.velocity = Vector2.zero;
        Physics2D.IgnoreCollision(player_collider, ball_collider, true);
    }

    public void loseControlOfBall() {
        if (ball.transform.parent == transform) {
            ball.transform.parent = null;
            has_ball = false;
            if (HUD.S.GameStarted) {
                Invoke("allowBallCollision", 0.5f);
            } else {
                Physics2D.IgnoreCollision(player_collider, ball_collider, false);
            }
        }
    }

    Vector2 getInputMovementVector() {
        Vector2 move_vector = Vector2.zero;
        move_vector.x += Input.GetAxis(my_inputs.hor);
        move_vector.y += Input.GetAxis(my_inputs.vert);

        return move_vector;
    }

    Vector2 getInputDribbleVector() {
        Vector2 dribble_vector = Vector2.zero;
        dribble_vector.x += Input.GetAxis(my_inputs.R_hor);
        dribble_vector.y += Input.GetAxis(my_inputs.R_vert);

        return dribble_vector;
    }

    bool getInputFire() {
        return Input.GetAxis(my_inputs.Shoot) > 0;
    }

    void checkDash() {
        if (slow_delay > 0) {
            slow_delay -= Time.deltaTime;
        } else if (slowed > 0) {
            slowed--;
            if (slowed > 0) {
                slow_delay = slow_delay_time;
            }
        }

        if (Input.GetAxis(my_inputs.skate) > 0 && dash_delay <= 0 && slowed < 3) {
            dash = true;
            dash_delay = dash_delay_time;
        } else if (dash_delay > 0) {
            dash_delay -= Time.deltaTime;
            if (dash_delay <= 0) {
                slowed++;
                slow_delay = slow_delay_time;
            }
        }
    }
    
    void dribble() {
        Vector2 dribble_vector = getInputDribbleVector();
        if (dribble_vector.magnitude > DRIBBLE_MAGNITUDE_THRESHOLD) {
            current_ball_angle = dribble_vector.normalized;
        }
        if (current_ball_angle != Vector2.zero) {
            ball.transform.localPosition = current_ball_angle * ball_offset_scale;
        }
    }

    void shoot() {
        HUD.S.PlaySound("kick", Random.Range(.5f,1f));
        loseControlOfBall();
        Vector2 shot = ball.transform.position-transform.position;
        Vector3.Normalize(shot);
        shot *= shot_force;
        ball_rb.AddForce(shot);
        ball_rb.isKinematic = false;
        has_ball = false;
        
    }

    bool getInputPower() {
        return Input.GetAxis(my_inputs.special) > 0;
    }

    void allowBallCollision() {
        Physics2D.IgnoreCollision(player_collider, ball_collider, false);
    }

    void OnCollisionEnter2D(Collision2D coll) {
        if (!is_goalie && coll.gameObject.tag == "Ball") {
            gainControlOfBall();
        }
    }
}
