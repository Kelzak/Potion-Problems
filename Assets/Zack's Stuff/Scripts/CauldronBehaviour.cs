using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CauldronBehaviour : MonoBehaviour
{
    PlayerInteraction playerInteract;
    public AudioSource audioSource;
    public AudioClip dropInSound;
    private bool sfxPlaying;

    // Start is called before the first frame update
    void Start()
    {
        GameObject[] playerObjectArr = GameObject.FindGameObjectsWithTag("Player");
        foreach(GameObject x in playerObjectArr)
        {
            if(x.GetComponent<PlayerInteraction>())
            {
                playerInteract = x.GetComponent<PlayerInteraction>();
            }
        }
        audioSource.clip = dropInSound;
    }

    // Update is called once per frame
    private void OnTriggerStay(Collider other)
    {
        if(other.tag == "Ingredient" && !playerInteract.GetHoldingObject())
        {
            other.tag = "Untagged";
            other.GetComponent<BoxCollider>().isTrigger = true;
            other.GetComponent<Rigidbody>().velocity -= other.GetComponent<Rigidbody>().velocity;
            other.transform.position = new Vector3(transform.position.x, transform.position.y + 3, transform.position.z);
            playerInteract.SetObjectToHold(null);
            GameController.TriggerIngredientCollected(other.gameObject);
            if(!sfxPlaying)
                StartCoroutine(SoundEffect());
            Destroy(other.gameObject, 2);
        }
    }

    private IEnumerator SoundEffect()
    {
        sfxPlaying = true;
        audioSource.Play();
        yield return new WaitForSeconds(4);
        sfxPlaying = false;
    }
}
