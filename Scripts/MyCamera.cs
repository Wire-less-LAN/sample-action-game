using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyCamera : MonoBehaviour
{
    public Text txt;
    public Rigidbody2D bdy;
    public creature c;
    Vector2 e;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        e = transform.position + (c.transform.position - transform.position) * 0.01f;
        transform.position = new Vector3(e.x, e.y, transform.position.z);
       // print(transform.position);
    }
}
