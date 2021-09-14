using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player : creature
{
    float t;
    float tempG;
    Vector2 movingVector;
    public hand Hand;
    public grapple Grapple;
    public Vector2 grappleVelocity;
    public float grappleVelocityMag = 100f;
    // Start is called before the first frame update
    void Awake()
    {
        //初始化数值
        speed = ordinarySpeed = 3f;
        airSpeed = 0.5f;
        isGrounded = true;
        jumpForce = 20f;
        jumpTime = 0.1f;
        initialHeight = 0;
        ordinaryDrag = 25;
        //airDrag = 10;
        gravityScale = 10;
        body.drag = virtualBody.drag = ordinaryDrag;
        isDashing = false;
        dashTime = 0.2f;
        dashForce = 10;
        movingVector = new Vector2(0, 0);
    }
    void Start()
    {
        Grapple.owner = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isDashing)
        {
            //移动
            if (Input.GetKey(KeyCode.W))
            {
                move(90f);
                movingVector += new Vector2(0, 1);
            }
            else movingVector = new Vector2(movingVector.x, 0);
            if (Input.GetKey(KeyCode.S))
            {
                move(270f);
                movingVector -= new Vector2(0, 1);
            }
            else movingVector = movingVector.y < 0 ? new Vector2(movingVector.x, 0) : movingVector;
            if (Input.GetKey(KeyCode.A))
            {
                move(180f);
                movingVector -= new Vector2(1, 0);
            }
            else movingVector = new Vector2(0, movingVector.y);
            if (Input.GetKey(KeyCode.D))
            {
                move(0f);
                movingVector += new Vector2(1, 0);
            }
            else movingVector = movingVector.x > 0 ? new Vector2(0, movingVector.y) : movingVector;

            //跳跃
            if (isGrounded && Input.GetKeyDown(KeyCode.Space)) StartCoroutine(jump());

            //todo:跳跃时禁用碰撞

            //发射钩爪
            if (!Grapple.isOut && Input.GetMouseButtonDown(0))
            {
                Grapple.v0 = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized * grappleVelocityMag;
                Grapple.gameObject.SetActive(true);
            }

            //冲刺
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                Debug.Log("Dashing");
                isDashing = true;
                t = 0;
                body.gravityScale = 0;
                body.velocity = new Vector2(virtualBody.velocity.x, virtualBody.velocity.y);
            }
        }
        else
        {
            body.AddForce(movingVector.normalized * dashForce);
            virtualBody.AddForce(movingVector.normalized * dashForce);
            if ((t += Time.deltaTime) > dashTime)
            {
                isDashing = false;
                body.gravityScale = isGrounded ? 0 : gravityScale;
            }
        }

    }

}
