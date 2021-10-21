using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class creature : MonoBehaviour
{
    //values
    public float gravityScale;
    public float airDrag;
    public float ordinaryDrag;
    public float speed;
    public float airSpeed;
    public float ordinarySpeed;
    public float jumpForce;
    public float jumpTime;//施加弹跳力的时长(秒）
    public float initialHeight;//用于重置阴影和人物位置的初始点位
    public bool isGrounded;
    public bool isDashing;
    public bool wasWaved;

    //components
    public Rigidbody2D virtualBody;
    public Rigidbody2D body;
    public GameObject staticSpot;

    //others
    public Coroutine j;

    void Awake()
    {
        //initialize components
        virtualBody = GetComponent<Rigidbody2D>();

    }
    void Start()
    {

    }


    void Update()
    {

    }

    public void move(float angle, Rigidbody2D bdy)
    {
        float rad = angle * 2 * Mathf.PI / 360f;
        bdy.AddForce(new Vector2(speed * Mathf.Cos(rad), speed * Mathf.Sin(rad)));
        //Debug.Log(rad);
    }
    public void move(float angle)
    {
        float rad = angle * 2 * Mathf.PI / 360f;
        virtualBody.AddForce(new Vector2(speed * Mathf.Cos(rad), speed * Mathf.Sin(rad)));
        body.AddForce(new Vector2(speed * Mathf.Cos(rad), speed * Mathf.Sin(rad)));
    }
    virtual public IEnumerator jump()
    {
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
        body.gravityScale = gravityScale;
        while (body.transform.localPosition.y > initialHeight) yield return 0;
        isGrounded = true;
        body.gravityScale = 0;
        body.velocity = new Vector2(virtualBody.velocity.x, virtualBody.velocity.y);
        body.transform.localPosition = new Vector3(body.transform.localPosition.x, initialHeight, body.transform.localPosition.z);
        speed = ordinarySpeed;
        virtualBody.drag = body.drag = ordinaryDrag;
    }
    public IEnumerator constForce(Vector2 v, float time)
    {
        for (float t = 0; t <= time; t += Time.deltaTime)
        {
            virtualBody.AddForce(v);
            body.AddForce(v);
            yield return 0;
        }
    }
    public IEnumerator constForce(Vector2 v, float time, Rigidbody2D bdy)
    {
        for (float t = 0; t <= time; t += Time.deltaTime)
        {
            bdy.AddForce(v);
            yield return 0;
        }
    }
    public void Jump()
    {
        if (j != null) StopCoroutine(j);
        j = StartCoroutine(jump());
    }
    public void Force(Vector2 v,float time)
    {
        StartCoroutine(constForce(v, time));
    }
}
