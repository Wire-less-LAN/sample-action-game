using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class creature : MonoBehaviour
{
    //values
    float speed;

    //components
    Rigidbody2D body;

    void Awake()
    {
        //initialize components
        body = GetComponent<Rigidbody2D>();
    }
    void Start()
    {

    }


    void Update()
    {
        
    }

    public void move(float angle)
    {
        
    }
}
