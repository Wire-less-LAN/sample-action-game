using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class grapple : MonoBehaviour
{
    public Text txt;
    public Rigidbody2D body;
    public bool isOut = false;
    public bool isTied = false;
    public bool isWaved = false;
    public bool wasTied = false;
    public Vector2 v0;
    public creature owner;
    public LineRenderer line;
    public float existingTime = 0.5f;
    public float dragForce = 10f;
    float timer = 0;
    public GameObject chainPrefab;
    public GameObject chainNodePrefab;
    public ChainNode ownerNode;//用于附着链条的虚拟点
    public Chain chain;
    public float distance;
    public float mass;
    public float frequency;
    public float range = 10f;
    public float reducedChainDrag = 0.1f;
    public float retrieveRange = 0.5f;
    Coroutine focus, retrieve, release, f, back, force, wave;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (wasTied) return;
        body.constraints = RigidbodyConstraints2D.FreezeAll;
        StopCoroutine(release);
        release = null;

        force = StartCoroutine(AddForce());
        retrieve = StartCoroutine(RetrieveChain());
        wasTied = isTied = true;
    }
    private void OnEnable()
    {
        isOut = false;
        isTied = false;
        isWaved = false;
        wasTied = false;
        wave = focus = retrieve = release = f = back = force = null;
        chain = GameObject.Instantiate(chainPrefab, transform.position, new Quaternion(0, 0, 0, 0)).GetComponent<Chain>();
        chain.Initiate(2, distance, frequency, mass);
        chain.tail.obj.transform.position = transform.position;
        chain.tail.joint.enabled = false;
        chain.head.next.body.isKinematic = chain.tail.body.isKinematic = true;
        chain.head.next.obj.transform.position = owner.transform.position;

        release = StartCoroutine(ReleaseChain());

        body.transform.position = new Vector2(owner.transform.position.x, owner.transform.position.y);
        body.velocity = v0;

        line.SetPosition(0, owner.body.transform.position);
        line.SetPosition(1, body.transform.position);

        isOut = true;
        timer = 0;
        focus = StartCoroutine(FocusNode(0));
    }
    void Awake()
    {
        distance = 0.1f;
        frequency = 5;
        mass = 0.1f;
        dragForce = 3;
        chainPrefab = Resources.Load<GameObject>("Prefabs/chain");
        chainNodePrefab = Resources.Load<GameObject>("Prefabs/chainNode");

    }
    void Update()
    {
        txt.text = owner.transform.localPosition.y.ToString();
        line.SetPosition(0, owner.body.transform.position);
        line.SetPosition(1, body.transform.position);
    }
    public IEnumerator AddForce()
    {
        Vector2 e;
        //在限定时间内给人物施力
        float t = 0;
        e = transform.position - owner.transform.position;
        while ((t += Time.deltaTime) < existingTime && e.magnitude >= 0.1f)
        {
            if (wasTied && Input.GetKeyDown(KeyCode.Mouse0) && !isWaved)
            {
                wave = StartCoroutine(Wave());
                StopCoroutine(force);
            }
            e = transform.position - owner.transform.position;//为防止撞墙回弹，不使用owner.body
            owner.body.AddForce(e.normalized * dragForce);
            owner.virtualBody.AddForce(e.normalized * dragForce);
            yield return 0;
        }
        back = StartCoroutine(Back());
        StopCoroutine(force);
    }
    public IEnumerator Back()
    {
        Vector2 e;
        isTied = false;

        //修改链子阻力
        ChainNode p = chain.head.next;
        while (p != null)
        {
            p.body.drag = reducedChainDrag;
            p = p.next;
        }
        StopCoroutine(focus);
        chain.tail.body.isKinematic = false;
        chain.tail.body.drag = chain.drag;

        f = StartCoroutine(FocusNode(3));//抓钩跟随锁链
        e = transform.position - owner.transform.position;
        body.constraints = RigidbodyConstraints2D.None;

        while (e.magnitude >= retrieveRange && !isWaved)
        {
            e = transform.position - owner.transform.position;
            if (wasTied && Input.GetKeyDown(KeyCode.Mouse0) && !isWaved)
            {
                wave = StartCoroutine(Wave());
                StopCoroutine(back);
            }
            yield return 0;
        }
        isOut = isWaved = isTied = false;
        StopAllCoroutines();
        chain.Kill();
        chain = null;
        retrieve = release = focus = null;
        gameObject.SetActive(false);
    }
    public IEnumerator ReleaseChain()
    {
        float t = 0;
        float t0 = 0;
        while (!isTied && t0 < existingTime)
        {
            t += Time.deltaTime;
            t0 += Time.deltaTime;
            if (t / distance * v0.magnitude >= 10)
            {
                chain.Add((int)(t / distance * v0.magnitude / 10), 1);
                t = 0;
            }
            yield return 0;
        }
        retrieve = StartCoroutine(RetrieveChain());
        back = StartCoroutine(Back());
        StopCoroutine(ReleaseChain());
    }

    public IEnumerator RetrieveChain()
    {
        Vector2 e;
        while (isOut)
        {
            e = chain.head.next.obj.transform.position - chain.head.next.next.obj.transform.position;
            if (e.magnitude < retrieveRange && chain.length > 2)
            {
                chain.Delete(2);
                continue;
            }
            chain.head.next.next.body.AddForce(e.normalized * dragForce);
            yield return 0;
        }
    }
    public IEnumerator Wave()
    {
        owner.wasWaved = isWaved = true;
        isTied = false;
        Vector2 e;
        Vector3 s;
        s = UnityEngine.Camera.main.ScreenToWorldPoint(Input.mousePosition);
        e = s - transform.position;
        if (e.magnitude > range) e = e.normalized * range;

        if (back != null)
        {
            StopCoroutine(back);
            back = null;
        }
        if (focus != null)
        {
            StopCoroutine(focus);
            focus = StartCoroutine(FocusNode(0));
        }
        if (force != null)
        {
            StopCoroutine(force);
            force = null;
        }

        owner.Force(-(s - owner.transform.position).normalized * owner.jumpForce, owner.jumpTime);
        owner.Force(new Vector2(0, owner.jumpForce), owner.jumpTime);
        owner.Jump();
        body.constraints = RigidbodyConstraints2D.None;

        float t = 0;
        while (t <= e.magnitude / v0.magnitude)
        {
            body.velocity = e.normalized * v0.magnitude;
            t += Time.deltaTime;
            yield return 0;
        }
        body.velocity = -e.normalized * v0.magnitude / 5f;
        StartCoroutine(Back());
    }
    public IEnumerator FocusNode(int i)
    {
        switch (i)
        {
            case 0:
                while (isOut)
                {
                    chain.head.next.obj.transform.position = owner.body.transform.position;
                    chain.tail.obj.transform.position = transform.position;
                    yield return 0;
                }
                break;
            case 1:
                while (isOut)
                {
                    chain.head.next.obj.transform.position = owner.body.transform.position;
                    yield return 0;
                }
                break;
            case 2:
                while (isOut)
                {
                    chain.tail.obj.transform.position = transform.position;
                    yield return 0;
                }
                break;
            case 3:
                while (isOut)
                {
                    chain.head.next.obj.transform.position = owner.body.transform.position;
                    transform.position = chain.tail.obj.transform.position;
                    yield return 0;
                }
                break;
        }

    }
}
