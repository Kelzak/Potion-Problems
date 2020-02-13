using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public Transform objectHolder;
    public float followSpeed = 1;
    public AudioSource audioSource;
    private GameObject objectToHold;
    private GameObject objectInHand;
    private Rigidbody objectInHandRb;
    private bool holdingObject;
    private GameObject gc;
    public GameObject buttonPromptPrefab;
    private GameObject buttonPromptInstance;

    private void Start()
    {
        gc = GameObject.Find("GameController");
    }

    // Update is called once per frame
    public bool canPickup = true;
    void Update()
    {
        if (Input.GetButtonDown("Pickup") && canPickup)
        {
            if (!holdingObject && objectInHand == null && objectToHold != null)
            {
                Pickup();
            }
            else if(holdingObject && objectInHand != null)
            {
                Drop();
            }
        }

        if (holdingObject && objectInHand != null)
        {
            Vector3 direction = (objectHolder.position - objectInHandRb.transform.position).normalized;
            objectInHandRb.MovePosition(objectHolder.transform.position + direction * followSpeed * Time.deltaTime);
            objectInHand.transform.rotation = objectHolder.rotation;
        }

        //Debug.Log("Object To Hold" + objectToHold);
        //Debug.Log("Object Currently Holding" + objectInHand);
    }

    public void ForceDrop()
    {
        if(holdingObject && objectInHand != null)
        Drop();
    }

    private void Pickup()
    {
        holdingObject = true;
        objectInHand = objectToHold;
        objectInHandRb = objectInHand.GetComponent<Rigidbody>();
        objectInHandRb.useGravity = false;

        if(GameController.currentEffect == GameController.Effects.Maze && holdingObject)
        {
            gc.GetComponent<MazeBehaviour>().TriggerFirstPickup();
        }
        if(objectInHand.GetComponent<ItemAIBehavior>() != null)
        {
            objectInHand.GetComponent<ItemAIBehavior>().beingHeld = true;
        }
        audioSource.Play();
    }

    private void Drop()
    {
        holdingObject = false;
        objectInHandRb.useGravity = true;
        objectInHand = null;
        objectInHandRb = null;
        audioSource.Play();
    }

    public bool GetHoldingObject()
    {
        return holdingObject;
    }

    public void SetObjectToHold(GameObject x)
    {
        objectToHold = x;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ingredient"))
        {
            if(buttonPromptInstance != null)
            {
                Destroy(buttonPromptInstance);
                buttonPromptInstance = null;
            }
            objectToHold = other.gameObject;

            if (buttonPromptInstance == null && !holdingObject && objectInHand == null)
            {
                buttonPromptInstance = Instantiate<GameObject>(buttonPromptPrefab, other.gameObject.transform.position, Quaternion.identity);
                buttonPromptInstance.transform.SetParent(other.transform);
                Debug.Log("Prompt Instantiated");
                buttonPromptInstance.transform.position = new Vector3(buttonPromptInstance.transform.position.x, buttonPromptInstance.transform.position.y + 1.25f, buttonPromptInstance.transform.position.z);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.CompareTag("Ingredient") && buttonPromptInstance != null && holdingObject && objectInHand != null)
        {
            buttonPromptInstance.SetActive(false);
        }
        else if(other.gameObject.CompareTag("Ingredient") && buttonPromptInstance != null && !holdingObject && objectInHand == null)
        {
            buttonPromptInstance.SetActive(true);
            buttonPromptInstance.transform.LookAt(Camera.main.transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Ingredient"))
        {
            if (objectToHold != null)
            {
                objectToHold = null;
            }

            if(buttonPromptInstance != null)
            {
                Destroy(buttonPromptInstance);
                buttonPromptInstance = null;
            }
        }
    }

}
