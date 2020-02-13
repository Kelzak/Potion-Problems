using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{
    GameController gc;

    //UI Elements to Link
    public GameObject ingredientsList;
    public GameObject ingredientStrikethrough;
    public GameObject blurImage;
    public Text timer;

    private List<GameObject> strikethroughArr;

    // Start is called before the first frame update
    void Start()
    {
        gc = GetComponent<GameController>();
        strikethroughArr = new List<GameObject>();

        for(int i = 0; i < ingredientStrikethrough.transform.childCount; i++)
        {
            strikethroughArr.Add(ingredientStrikethrough.transform.GetChild(i).gameObject);
        }

        GameController.IngredientCollected += UpdateIngredientsList;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Updates the Timer UI accordingly based on the time passed as a parameter
    /// </summary>
    /// <param name="time">The time (in seconds) for the timer to be set to</param>
    public void UpdateTimer(float time)
    {
        int minutes = (int)time / 60;
        int seconds = (int)time % 60;

        timer.text = minutes + ":" + seconds.ToString("D2");
    }

    public void UpdateIngredientsList(GameObject ingredient)
    {
        int index = gc.GetIndexFromIngredient(ingredient);
        Debug.Log(index);
        strikethroughArr[index].SetActive(true);
    }

    private void OnDisable()
    {
        GameController.IngredientCollected -= UpdateIngredientsList;
    }

}
