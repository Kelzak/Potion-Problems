using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorBehaviour : MonoBehaviour
{
    public RockFallController controller;



    private void OnTriggerEnter(Collider collision)
    {
        if(collision.tag == "Player")
        {
            if (!controller.fling)
            {
                controller.HitPlayer();
            }
        }
    }
}
