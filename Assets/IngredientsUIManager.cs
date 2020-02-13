using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class IngredientsUIManager : MonoBehaviour
{
    public Animator uiAnim;
    public TextMeshProUGUI textBox;
    public Animator scrollAnimator;
    //Store all your text in this string array
    string[] letterText = new string[] { "Hello my Wizard disciple. I am out to lunch for the day so      you'll have to create our" +
        " daily   potion without me. It's a        dangerous one, so put all the   ingredients in quickly or you    might doom us all! No pressure!" };
    int currentlyDisplayingText = 0;
    void Awake()
    {
        StartCoroutine(AnimateText());
    }
    //This is a function for a button you press to skip to the next text
    public void SkipToNextText()
    {
        StopAllCoroutines();
        currentlyDisplayingText++;
        //If we've reached the end of the array, do anything you want. I just restart the example text
        if (currentlyDisplayingText > letterText.Length)
        {
            currentlyDisplayingText = 0;
        }
        StartCoroutine(AnimateText());
    }
    //Note that the speed you want the typewriter effect to be going at is the yield waitforseconds (in my case it's 1 letter for every 0.03 seconds, replace this with a public float if you want to experiment with speed in from the editor)
    IEnumerator AnimateText()
    {

        for (int i = 0; i < (letterText[currentlyDisplayingText].Length + 1); i++)
        {
            textBox.text = letterText[currentlyDisplayingText].Substring(0, i);
            yield return new WaitForSeconds(.03f);
        }

        yield return new WaitForSeconds(4);
        scrollAnimator.SetTrigger("ScrollOut");


    }





    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Tab"))
        {
            uiAnim.SetBool("TabIngredients", true);
        }

        else if(Input.GetButtonUp("Tab"))
        {
            uiAnim.SetBool("TabIngredients", false);
        }
    }
}
