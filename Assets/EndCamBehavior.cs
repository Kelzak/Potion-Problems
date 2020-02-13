using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class EndCamBehavior : MonoBehaviour
{
    public GameObject cauldron;
    public Animator canAnim;

    public TextMeshProUGUI winText;

    public GameObject buttons;

    public GameController GC;

    public Canvas mainUI;

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            canAnim.SetTrigger("GameEnd");
        }
    }
    public void ShakeCauldron()
    {
        //iTween.ShakePosition(cauldron, new Vector3(.3f, .3f, .3f), 3);
        iTween.ShakePosition(cauldron, iTween.Hash("amount", new Vector3(.3f, .3f, .3f), "time", 3, "oncompletetarget", gameObject, "oncomplete", "PanUp"));
    }

    public void PanUp()
    {
        print("yahhh");
        canAnim.SetTrigger("DisplayWinText");
    }

    public void DisplayText()
    {
        mainUI.GetComponent<Canvas>().worldCamera = gameObject.GetComponent<Camera>();
        buttons.SetActive(true);
        if (GC.playerWins)
        {
            winText.text = "You Win!";
        }

        else
        {
            winText.text = "You lose!";
        }
    }
}
