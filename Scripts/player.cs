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
    public float reducedGravityScale;
    public float jumpDashForce;
    public float jumpDashForceTime;
    //todo:用struct封装常恒力
    // Start is called before the first frame update
    void Awake()
    {
        //初始化数值
        speed = ordinarySpeed = 2f;
        airSpeed = 0.01f;
        isGrounded = true;
        jumpForce = 3f;
        jumpTime = 0.1f;
        initialHeight = 0;
        ordinaryDrag = 25;
        reducedGravityScale = 0.8f;
        airDrag = 1;
        gravityScale = 40;
        body.drag = virtualBody.drag = ordinaryDrag;
        isDashing = false;
        movingVector = new Vector2(0, 0);
        jumpDashForce = 10f;
        jumpDashForceTime = 0.1f;
    }
    void Start()
    {
        Grapple.owner = this;
    }

    // Update is called once per frame
    void Update()
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
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(constForce(movingVector.normalized*jumpDashForce,jumpDashForceTime));
            StartCoroutine(jump());
        }

        //todo:跳跃时禁用碰撞

        //发射钩爪
        if (!Grapple.isOut && Input.GetMouseButtonDown(0))
        {
            Grapple.v0 = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized * grappleVelocityMag;
            Grapple.gameObject.SetActive(true);
        }
    }
    public override IEnumerator jump()
    {
        print("override");
        isGrounded = false;
        speed = airSpeed;
        virtualBody.drag = body.drag = airDrag;
        Vector2 v = new Vector2(0, jumpForce);

        //持续给人物施力
        for (float t = 0; t <= jumpTime; t += Time.deltaTime)
        {
            body.AddForce(v);
            yield return 0;
        }

        //使人物受重力直到掉回原点
        body.gravityScale = Grapple.isOut ? reducedGravityScale : gravityScale;
        while (body.transform.localPosition.y > initialHeight)
        {
            if (Grapple.isOut) body.gravityScale = reducedGravityScale;
            yield return 0;
        }
        isGrounded = true;
        body.gravityScale = 0;
        body.velocity = new Vector2(virtualBody.velocity.x, virtualBody.velocity.y);
        body.transform.localPosition = new Vector3(body.transform.localPosition.x, initialHeight, body.transform.localPosition.z);
        speed = ordinarySpeed;
        virtualBody.drag = body.drag = ordinaryDrag;
    }
}
