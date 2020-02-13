using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookThrowBehavior : MonoBehaviour
{
    public float speed;
    private Transform target;
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        target = GameObject.Find("BlackHole").transform;
        Destroy(gameObject, 3);
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(target);
        rb.AddForce(transform.forward * speed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name == "BlackHole")
        {
            Destroy(gameObject);
        }
    }
}
