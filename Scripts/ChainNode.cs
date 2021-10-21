using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainNode
{
    public GameObject obj;
    public Rigidbody2D body;
    public SpringJoint2D joint;
    public ChainNode next;
    public ChainNode prev;
    public ChainNode(GameObject o = null, Rigidbody2D r = null, SpringJoint2D j = null, ChainNode n = null, ChainNode p = null, float mass = 0)
    {
        obj = o;
        body = r;
        joint = j;
        next = n;
        prev = p;

        if (obj != null)
        {
            body = obj.GetComponent<Rigidbody2D>();
            joint = obj.GetComponent<SpringJoint2D>();
            body.mass = mass;
            body.gravityScale = 0;
        }

    }

}