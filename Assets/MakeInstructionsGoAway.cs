using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeInstructionsGoAway : MonoBehaviour
{
    public Animator instructionsButtonAnimation;
    public bool canGoAway;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Pickup") && canGoAway)
        {
            canGoAway = false;
            print("eat my ass");
            instructionsButtonAnimation.SetTrigger("ScrollOut");

        }
    }
}
