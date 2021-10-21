using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chain : MonoBehaviour
{

    public int length = 0;
    public float distance;
    public float frequency;
    public float mass;
    public float drag;
    public ChainNode head;
    public ChainNode tail;
    public GameObject prefabNode;
    // Start is called before the first frame update
    void Awake()
    {
        prefabNode = Resources.Load<GameObject>("Prefabs/chainNode");
        head = new ChainNode();//头结点
        tail = head;
        drag=20f;
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void Initiate(int n, float dis, float fre, float m)
    {
        length = 0;
        distance = dis;
        frequency = fre;
        mass = m;
        if (n < 1) return;
        Add(n, 0);
    }
    public void Add(int n, int index)
    {
        if (index > length)
        {
            Debug.Log("Error:quest out of range");
            return;
        }
        ChainNode p = head;
        ChainNode q;
        GameObject temp;
        for (int i = 1; i <= index; i++) p = p.next;
        for (int i = 1; i <= n; i++)
        {
            q = p.next;
            temp = GameObject.Instantiate(prefabNode, transform);
            temp.transform.position = p.body == null ? new Vector3(transform.position.x, transform.position.y, transform.position.z) : new Vector3(p.body.transform.position.x, p.body.transform.position.y, p.body.transform.position.z);
            p.next = new ChainNode(temp, temp.GetComponent<Rigidbody2D>(), temp.GetComponent<SpringJoint2D>(), q, p, mass);
            p.next.joint.autoConfigureDistance = false;
            p.next.body.drag = drag;
            p.next.joint.connectedBody = q == null ? null : q.body;
            p.next.joint.distance = distance;
            p.next.joint.frequency = frequency;
            p.next.next = q;
            p.next.prev = p;
            if (q != null) q.prev = p.next;
            p.next.obj.name = (++length).ToString();
            if (p.body != null) p.joint.connectedBody = p.next.body;
            p = p.next;
        }
        //更新tail
        if (p.next == null) tail = p;
    }
    public void Delete(int index)
    {
        if (index > length || index < 1)
        {
            Debug.Log("Error:quest out of range");
            return;
        }
        ChainNode p, q;
        p = head;
        for (int i = 1; i <= index; i++) p = p.next;

        q = p.next;
        p.prev.next = q;
        if (q != null)
        {
            if (p.prev.joint != null) p.prev.joint.connectedBody = q.body;
            q.prev = p.prev;
        }
        else
        {
            if (p.prev.joint != null) p.prev.joint.connectedBody = null;
            tail = p.prev;
        }

        length--;
        GameObject.Destroy(p.obj);
    }
    public void Kill()
    {
        while (length > 0) Delete(length);
        head = tail = null;
        GameObject.Destroy(gameObject);
    }
}
