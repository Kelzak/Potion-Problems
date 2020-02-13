using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;
public class GameController : MonoBehaviour
{
    public enum Effects { ScreenShake, Maze, Drunk, IceFloor, Scream, None }; //When converted to int is the proper index for ingredient prefabs list and effect functions//List of prefabs, Matches up with effects in order - Volcanic Heart, Minotaur Horn, Golden Grapes, Frost Eye, Dark Crystal
    public GameObject[] ingredientPrefabs = new GameObject[TOTAL_INGREDIENTS];
    public float time;
    public float TimeRemaining
    {
        set
        {
            timeRemaining = value;
            ui.UpdateTimer(timeRemaining);
        }
        get { return timeRemaining;}
    }
    public static Effects currentEffect = Effects.None;

    private MazeBehaviour maze;
    private RockFallController rock;

    public PlayerBehaviour PB;

    //UI
    public UIHandler ui;

    //Ingredient Variables
    public delegate void IngredientDelegate(GameObject ingredient);
    public static event IngredientDelegate IngredientCollected;
    private const int TOTAL_INGREDIENTS = 5;
    public int ingredientsCollected = 0;
    public Vector3[] defaultSpawnPoints;
    private GameObject lastIngredientCollected = null;
    private List<bool> ingredientList; //List of ingredients. Each entry is true if that ingredient has been collected, false if it hasn't
    private float timeRemaining;

    //Effect Variables
    private delegate IEnumerator EffectFunction();
    private List<EffectFunction> effectFunctions;


    [Header("Cam Shake Effects ")]
    public Camera gameCam;
    Vector3 originalCamPos;
    public float shakeAmount;


    [Header("Ice Floor Effects")]
    public MeshRenderer floorRend;
    public Material iceMat;
    public Material floorMat;
    public GameObject snowParticleSystem;
    public GameObject icycles;

    [Header("Black Hole Effects")]
    public GameObject blackHoleObject;
    public GameObject[] Bookshelves;
    [HideInInspector] public bool blackHoleRunning;


    [Header("Win / Lose Game Stuff")]
    public GameObject endCam;
    public bool playerWins;

    [Header("Instructions / UI Stuff")]
    public TextMeshProUGUI instructionsText;
    public int instructionsTimeTillGone;
    public GameObject instructionsBox;
    public Animator ingredientsListAnim;
    public Animator instructionsAnim;
    public TextMeshProUGUI ingredientsCollectedText;


    private void Awake()
    {
        //Delegate
        IngredientCollected += InventoryIngredient;
    }
    // Start is called before the first frame update
    void Start()
    {
        PB = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerBehaviour>();
        currentEffect = Effects.None;

        //Gets and sets original position of the main camera to a Vector 3 variable
        originalCamPos = gameCam.transform.position;

        ui = GetComponent<UIHandler>();

        maze = GetComponent<MazeBehaviour>();
        rock = GetComponent<RockFallController>();
        //Start the timer
        TimeRemaining = time;

        ingredientList = new List<bool>();
        //Add list of ingredient prefabs into the list of ingredients that need to be collected.
        for(int i = 0; i < TOTAL_INGREDIENTS; i++)
        {
            ingredientList.Add(false);
        }

        #region addEffectFunctions
        //Add functions for each of the effects into a list
        effectFunctions = new List<EffectFunction>();
        effectFunctions.Add(ScreenShake);
        effectFunctions.Add(Maze);
        effectFunctions.Add(Drunk);
        effectFunctions.Add(IceFloor);
        effectFunctions.Add(BlackHole);
        #endregion

        StartCoroutine(Game());
    }

    // Update is called once per frame
    public bool timerOverride = false;
    void Update()
    {
        //Updates the remaining time based on Time.deltaTime
        if (TimeRemaining > 0 && gameRunning && ingredientsCollected >= 1 && !timerOverride)
        {
            TimeRemaining -= Time.deltaTime;
        }

    }


    bool gameRunning = false;
    /// <summary>
    /// Manages the core game loop and keeps track of whether the game continues or not
    /// </summary>
    /// <returns></returns>
    IEnumerator Game()
    {
        gameRunning = true;
        int tempIngredientsCollected;
        //Overall game condition, keeps playing while all ingredients haven't been collected or there's still time on the timer
        while((tempIngredientsCollected = ingredientsCollected) < TOTAL_INGREDIENTS && TimeRemaining > 0)
        {
            //Clear the Area
            DespawnIngredients();

            //Start an effect
            if (lastIngredientCollected == null) //If no ingredients collected, run a random effect
            {
                SpawnIngredient(defaultSpawnPoints,2);
               
            }
            else //Ingredients have been collected so start an effect based on previously collected ingredient
            {
                int index = GetIndexFromIngredient(lastIngredientCollected);
                currentEffect = (Effects)index;
                StartCoroutine(effectFunctions[index]());
            }

            while (ingredientsCollected == tempIngredientsCollected && TimeRemaining > 0) //Wait until ingredient is collected before starting another effect
            {
                yield return null;
            }

            yield return null;
        }

        gameRunning = false;
        currentEffect = Effects.None;

        //Main Game Loop Exited - Check to See how it turned out
        if(ingredientsCollected >= TOTAL_INGREDIENTS) //Player Collected all Ingredients
        {
            print("playerWins");
            endCam.SetActive(true);
            playerWins = true;
            instructionsText.text = "";
        }
        else //Player ran out of time
        {
            print("player loses");
            endCam.SetActive(true);
            playerWins = false;
            instructionsText.text = "";

        }
    }

    public static void TriggerIngredientCollected(GameObject ingredient)
    {
        IngredientCollected?.Invoke(ingredient);

    }

    public int GetIndexFromIngredient(GameObject ingredient)
    {
        if(ingredient.name.Contains("Heart"))
        {

            instructionsText.text = "Dodge the Meteors!";
            PB.playerCanMove = false;
            ingredientsListAnim.SetBool("TabIngredients", true);
            instructionsAnim.SetBool("ShowInstructions", true);
            StartCoroutine(StopShowingInstructions());
            TimeRemaining += 1;
            return 0;
        }
        else if(ingredient.name.Contains("Horn"))
        {

            instructionsText.text = "Navigate the Maze!";
            PB.playerCanMove = false;
            ingredientsListAnim.SetBool("TabIngredients", true);
            instructionsAnim.SetBool("ShowInstructions", true);
            StartCoroutine(StopShowingInstructions());
            TimeRemaining += 1;
            return 1;
        }
        else if(ingredient.name.Contains("Grape"))
        {

            instructionsText.text = "Catch the Ingredient!";
            PB.playerCanMove = false;
            ingredientsListAnim.SetBool("TabIngredients", true);
            instructionsAnim.SetBool("ShowInstructions", true);
            StartCoroutine(StopShowingInstructions());
            TimeRemaining += 1;
            return 2;
        }
        else if(ingredient.name.Contains("Eye"))
        {
    
            instructionsText.text = "Avoid the icycles!";
            PB.playerCanMove = false;
            ingredientsListAnim.SetBool("TabIngredients", true);
            instructionsAnim.SetBool("ShowInstructions", true);
            StartCoroutine(StopShowingInstructions());
            TimeRemaining += 1;

            return 3;
        }
        else if(ingredient.name.Contains("Crystal"))
        {

            instructionsText.text = "Don't Get Sucked!";
            PB.playerCanMove = false;
            ingredientsListAnim.SetBool("TabIngredients", true);
            instructionsAnim.SetBool("ShowInstructions", true);
            StartCoroutine(StopShowingInstructions());
            TimeRemaining += 1;
            return 4;
        }
        else
        {
            Debug.Log("Ingredient index not found");
            return 0;
        }
        
    }

    //EFFECT METHODS
    #region Effects
    IEnumerator ScreenShake()
    {
        //Format Ex: { defaultSpawnpoint, new Vector3(x, y, z), new Vector3(x,y,z) } , Any coordinates you want an ingredient to be able to spawn
        Vector3[] tempSpawnPoints = { defaultSpawnPoints[0], defaultSpawnPoints[1] };
        SpawnIngredient(tempSpawnPoints,2); //Second parameter determines max number of ingredients to spawn, defaults to 1. Ex: SpawnIngredient(spawnPoints, 2) would allow 2 ingredients to spawn instead of 1

        Debug.Log("ScreenShake is running");
        yield return new WaitForSeconds(2);
        rock.StartMeteorFall();
        while (currentEffect == Effects.ScreenShake)
        {
            gameCam.transform.position = originalCamPos + Random.insideUnitSphere * shakeAmount * Time.deltaTime * (ingredientsCollected * 2);
            gameCam.GetComponent<Animator>().SetBool("canPlayDrunk", false);
            gameCam.GetComponent<Animator>().enabled = false;
            yield return null;
        }
    }
    
    IEnumerator IceFloor()
    {
         //Format Ex: { defaultSpawnpoint, new Vector3(x, y, z), new Vector3(x,y,z) } , Any coordinates you want an ingredient to be able to spawn
        SpawnIngredient(defaultSpawnPoints,2/*, 2*/); //Second parameter determines max number of ingredients to spawn, defaults to 1. Ex: SpawnIngredient(spawnPoints, 2) would allow 2 ingredients to spawn instead of 1

        floorRend.material = iceMat;
        snowParticleSystem.SetActive(true);





        Debug.Log("IceFloor is running");
        while (currentEffect == Effects.IceFloor)
        {
            icycles.SetActive(true);
            yield return null;
        }

        floorRend.material = floorMat;
        icycles.SetActive(false);
        snowParticleSystem.SetActive(false);

        // floorRend.materials[0] = floorMat;
    }

    IEnumerator Drunk()
    {
        //Format Ex: { defaultSpawnpoint, new Vector3(x, y, z), new Vector3(x,y,z) } , Any coordinates you want an ingredient to be able to spawn
        SpawnIngredient(defaultSpawnPoints,2); //Second parameter determines max number of ingredients to spawn, defaults to 1. Ex: SpawnIngredient(spawnPoints, 2) would allow 2 ingredients to spawn instead of 1

        GameObject[] ingredientsInScene = GameObject.FindGameObjectsWithTag("Ingredient");
        for (int i = 0; i < ingredientsInScene.Length; i++)
        {
            ingredientsInScene[i].AddComponent<ItemAIBehavior>();
            for (int j = 0; j < ingredientsInScene[i].GetComponent<ItemAIBehavior>().destinationNodes.Count; j++)
            {
                ingredientsInScene[i].GetComponent<ItemAIBehavior>().destinationNodes[j] = GameObject.Find("Node" + j).transform;
            }
        }

        Debug.Log("Drunk is running");
        while (currentEffect == Effects.Drunk)
        {
            gameCam.GetComponent<Animator>().enabled = true;
            gameCam.GetComponent<Animator>().SetBool("canPlayDrunk", true);
            ui.blurImage.SetActive(true);
            yield return null;
        }
        ui.blurImage.SetActive(false);
        gameCam.GetComponent<Animator>().SetBool("canPlayDrunk", false);
         
        yield return null;
    }

    IEnumerator Maze()
    {
        Vector3[] spawnPoints = { new Vector3(-9.85f, 1.82f, -3.79f), new Vector3(-11.63f, 2.46f, -17.49f) };
        

        Debug.Log("Maze is running");
        maze.StartMaze();

        yield return new WaitForSeconds(0.5f);

        SpawnIngredient(spawnPoints, 2);

        while (currentEffect == Effects.Maze && !maze.doneCleaning())
        {
            //place code here
            yield return null;
        }
    }

    IEnumerator BlackHole()
    {
        SpawnIngredient(defaultSpawnPoints,2);

        Debug.Log("Black Hole is running");

        foreach (GameObject item in Bookshelves)
        {
            item.GetComponent<BookSpawnerBehavior>().Run();
        }

        while (currentEffect == Effects.Scream)
        {
            blackHoleObject.SetActive(true);
            blackHoleRunning = true;
            yield return null;
        }
        blackHoleObject.SetActive(false);
        blackHoleRunning = false;
        
    }

    #endregion

    private void SpawnIngredient(Vector3[] spawnPoints, int maxNumToSpawn = 1)
    {
        //Find Which ingredients still need to be collected
        List<int> ingredientPoolIndexes = new List<int>();
        for(int i = 0; i < ingredientList.Count; i++)
        {
            if(ingredientList[i] == false && (int)currentEffect != i)
            {
                ingredientPoolIndexes.Add(i);
            }
        }

        //Get List of Possible Spawn Points from Array
        List<Vector3> availableSpawnPoints = new List<Vector3>();
        for(int i = 0; i < spawnPoints.Length; i++)
        {
            availableSpawnPoints.Add(spawnPoints[i]);
        }

        //Spawn in at spawn points
        for(int i = 0; i < spawnPoints.Length && i < maxNumToSpawn && i < ingredientPoolIndexes.Count; i++)
        {
            //Pick Random Indexes from available ingredients and spawn points
            int randomIngredientIndex = Random.Range(0, ingredientPoolIndexes.Count);
            int randomSpawnPointIndex = Random.Range(0, availableSpawnPoints.Count);
            //Spawn ingredient at point
            Instantiate<GameObject>(ingredientPrefabs[ingredientPoolIndexes[randomIngredientIndex]], availableSpawnPoints[randomSpawnPointIndex], Quaternion.identity);
            //Remove both from 
            ingredientPoolIndexes.RemoveAt(randomIngredientIndex);
            availableSpawnPoints.RemoveAt(randomSpawnPointIndex);
        }
    }

    private void SpawnIngredient(Vector3 spawnPoint, int maxNumToSpawn = 1)
    {
        Vector3[] spawnPoints = { spawnPoint };
        SpawnIngredient(spawnPoints, maxNumToSpawn);
    }

    private void DespawnIngredients()
    {
        GameObject[] activeIngredients = GameObject.FindGameObjectsWithTag("Ingredient");
        foreach(GameObject x in activeIngredients)
        {
            Destroy(x);
        }
    }

   

    void InventoryIngredient(GameObject ingredient)
    {
        int index = GetIndexFromIngredient(ingredient);
        lastIngredientCollected = ingredient;
        ingredientList[index] = true;
        ingredientsCollected++;
    }

    private void OnDisable()
    {
        IngredientCollected -= InventoryIngredient;
    }



    public IEnumerator StopShowingInstructions()
    {
        ingredientsCollectedText.text = "Ingredients Collected:   " + ingredientsCollected + " / 5";
        timerOverride = true;
        yield return new WaitForSeconds(instructionsTimeTillGone);
       // instructionsBox.SetActive(false);
        ingredientsListAnim.SetBool("TabIngredients", false);
        instructionsAnim.SetBool("ShowInstructions", false);
        PB.playerCanMove = true;
        timerOverride = false;


    }
}
