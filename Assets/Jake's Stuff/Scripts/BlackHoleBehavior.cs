using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHoleBehavior : MonoBehaviour
{
    public float pullRadius = 2;
    public float pullForce = 1;

    public void FixedUpdate()
    {
        foreach (Collider collider in Physics.OverlapSphere(transform.position, pullRadius))
        {
            // calculate direction from target to me
            Vector3 forceDirection = transform.position - collider.transform.position;

            if (collider.GetComponent<Rigidbody>())
            {
                // apply force on target towards me
                collider.GetComponent<Rigidbody>().AddForce(forceDirection.normalized * pullForce * Time.fixedDeltaTime);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Ingredient") || other.gameObject.name == "Player")
        {
            other.gameObject.transform.position = new Vector3
            (Random.Range(-10, 10), 13, Random.Range(-17, -3));
        }
    }
}
