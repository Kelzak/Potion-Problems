using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class ButtonBehavior : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject instructionsMenu;
    public GameObject mainMenu;




    public void loadGameScene()
    {
        SceneManager.LoadScene(1);
    }


    public void loadInstructions()
    {
        instructionsMenu.SetActive(true);
        mainMenu.SetActive(false);
    }

    public void BackButton()
    {
        mainMenu.SetActive(true);
        instructionsMenu.SetActive(false);

    }

    public void loadTitleScreen()
    {
        SceneManager.LoadScene(0);
    }

    public void Quit()
    {
        Application.Quit();

    }






}
