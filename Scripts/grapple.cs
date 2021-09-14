using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class grapple : MonoBehaviour
{
    public Text txt;
    public Rigidbody2D body;
    public bool isOut = false;
    public Vector2 v0;
    public creature owner;
    public LineRenderer line;
    public float existingTime = 0.5f;
    public float dragForce = 10f;
    float timer = 0;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        txt.text = "2222";
        body.constraints = RigidbodyConstraints2D.FreezeAll;
        StartCoroutine(countDown());
    }
    private void OnEnable()
    {

        body.transform.position = new Vector2(owner.transform.position.x, owner.transform.position.y);
        body.velocity = v0;
        txt.text = "grapple enabled v0=" + body.velocity.ToString();
        line.SetPosition(0, owner.body.transform.position);
        line.SetPosition(1, body.transform.position);
        isOut = true;
        timer = 0;
    }
    void Awake()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if ((timer += Time.deltaTime) > existingTime*2)
        {
            isOut = false;
            body.constraints = RigidbodyConstraints2D.None;
            gameObject.SetActive(false);
        }
        line.SetPosition(0, owner.transform.position);
        line.SetPosition(1, body.transform.position);
    }
    public IEnumerator countDown()
    {
        float t = 0;
        Vector2 e = Vector2.Lerp(transform.position, owner.transform.position, 1f);
        while ((t += Time.deltaTime) < existingTime && e.magnitude >= 0.1f)
        {
            e = transform.position - owner.body.transform.position;
            owner.body.AddForce(e.normalized * dragForce);
            owner.virtualBody.AddForce(e.normalized * dragForce);
            txt.text = (dragForce).ToString();
            yield return 0;
        }
        e = Vector2.Lerp(transform.position, owner.transform.position, 1f);
        body.constraints = RigidbodyConstraints2D.None;
        t = 0;
        while (e.magnitude >= 0.1f)
        {
            if (t <= 20) t++;
            e = transform.position - owner.transform.position;
            body.velocity = -e.normalized * (20 + t * 0.1f);
            txt.text = "back" + body.velocity.ToString();
            yield return 0;
        }
        isOut = false;

        gameObject.SetActive(false);
    }
}
