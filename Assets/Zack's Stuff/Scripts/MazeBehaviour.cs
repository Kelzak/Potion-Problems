using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class MazeBehaviour : MonoBehaviour
{
    public GameObject environment;
    public GameObject[] mazeHolders; //Array of All mazes and inverses in scene
    private List<Maze> mazeList; //List of Maze objects made up of maze and inverse gameObjects
    private int activeMazeIndex; //Current Maze
    private bool firstPickup = false;

    public float shiftSpeed = 5f;

    public class Maze 
    {
        public void AssignVariables(GameObject maze, GameObject original, GameObject inverse)
        {
            this.maze = maze;
            this.original = original;
            this.inverse = inverse;
            mazeParts = new List<GameObject>();
            originalParts = new List<GameObject>();
            inverseParts = new List<GameObject>();
            //Assign Maze Parts
            for (int i = 0; i < maze.transform.childCount; i++)
            {
                mazeParts.Add(maze.transform.GetChild(i).gameObject);
            }

            //Assign Original Parts
            for (int i = 0; i < original.transform.childCount; i++)
            {
                originalParts.Add(original.transform.GetChild(i).gameObject);
            }

            //Assign Inverse Parts
            for (int i = 0; i < inverse.transform.childCount; i++)
            {
                inverseParts.Add(inverse.transform.GetChild(i).gameObject);
            }
        }

        void Start()
        {

        }


        GameObject maze;
        public List<GameObject> mazeParts;

        GameObject original;
        List<GameObject> originalParts;

        GameObject inverse;
        List<GameObject> inverseParts;


        public GameObject GetMaze()
        {
            return maze;
        }


        public bool Shifted(GameObject part, GameObject goal)
        {
            bool equal = true;
            if (!Vector3.Equals(part.transform.position,goal.transform.position) || !Vector3.Equals(part.transform.rotation,goal.transform.rotation))
            {
                equal = false;
            }

            return equal;
        }

        bool shifting = false;
        private IEnumerator ShiftMaze(List<GameObject> from, List<GameObject> to, float shiftSpeed)
        {
            shifting = true;
            for (int i = 0; i < from.Count && i < to.Count; i++)
            {
                while (!Shifted(from[i], to[i]))
                {
                    from[i].transform.position = Vector3.MoveTowards(from[i].transform.position, to[i].transform.position, shiftSpeed * Time.deltaTime);
                    from[i].transform.rotation = Quaternion.RotateTowards(from[i].transform.rotation, to[i].transform.rotation, 500 * Time.deltaTime);
                    yield return null;
                }
                yield return null;
            }
            shifting = false;
        }

        public bool isShifting()
        {
            return shifting;
        }

        public void ShiftTo(string mazeToShiftTo, float shiftSpeed, MonoBehaviour mono)
        {
            switch (mazeToShiftTo)
            {
                case "original": //Original
                    mono.StartCoroutine(ShiftMaze(mazeParts, originalParts, shiftSpeed));
                    break;
                case "inverse": //Inverse
                    mono.StartCoroutine(ShiftMaze(mazeParts, inverseParts, shiftSpeed));
                    break;
            }
        }
        
    }

   

    // Start is called before the first frame update
    void Start()
    {
        mazeList = new List<Maze>();

        for (int i = 0; i < mazeHolders.Length && i + 1 < mazeHolders.Length && i + 2 < mazeHolders.Length; i+=3)
        {
            mazeList.Add(new Maze());
            mazeList[i].AssignVariables(mazeHolders[i], mazeHolders[i + 1], mazeHolders[i + 2]);
        }

        Debug.Log("MazeList Count = " + mazeList.Count);

        for(int i = 0; i < mazeList.Count; i++)
        {
            if(mazeList[i] != null)
            {
                activeMazeIndex = i;
            }
        }
    }

    public void TriggerFirstPickup()
    {
       firstPickup = true;
    }

    public void StartMaze()
    {
        StartCoroutine(RunMaze());
    }

    bool running = false;
    private IEnumerator RunMaze()
    {
        running = true;

        //Remove Scenery
        StartCoroutine(ClearEnvironment());
        //Activate maze parts
        foreach (GameObject x in mazeList[activeMazeIndex].mazeParts)
        {
            x.SetActive(true);
        }

        //Put the Maze Together
        mazeList[activeMazeIndex].ShiftTo("original", shiftSpeed, this);
        while(mazeList[activeMazeIndex].isShifting())
        {
            yield return null;
        }

        //Wait for player to 
        while(firstPickup == false)
        {
            yield return null;
        }

        mazeList[activeMazeIndex].ShiftTo("inverse", shiftSpeed, this);
        while (mazeList[activeMazeIndex].isShifting())
        {
            yield return null;
        }

        while(GameController.currentEffect == GameController.Effects.Maze)
        {
            yield return null;
        }

        


        running = false;
        StartCoroutine(ClearMaze());
        while(!cleanup1 || !cleanup2)
        {
            yield return null;
        }
        firstPickup = true;
    }

    bool cleanup1 = false;
    private IEnumerator ClearMaze()
    {
        float goalY = 50;

        for(int i = 0; i < mazeList[activeMazeIndex].mazeParts.Count; i++)
        {
            while(mazeList[activeMazeIndex].mazeParts[i].transform.position.y != goalY)
            {
                mazeList[activeMazeIndex].mazeParts[i].transform.position = Vector3.MoveTowards(mazeList[activeMazeIndex].mazeParts[i].transform.position, new Vector3(
                mazeList[activeMazeIndex].mazeParts[i].transform.position.x,
                goalY,
                mazeList[activeMazeIndex].mazeParts[i].transform.position.z), 500 * Time.deltaTime);
                yield return null;
            }

            mazeList[activeMazeIndex].mazeParts[i].SetActive(false);
            yield return null;
        }

        cleanup1 = true;
    }

    bool cleanup2 = false;
    private IEnumerator ClearEnvironment()
    {
        float originY = environment.transform.position.y;
        float goalY = environment.transform.position.y + 50;

        //Get Rid of Environment
        while(environment.transform.position.y != goalY)
        {
            environment.transform.position = Vector3.MoveTowards(environment.transform.position, new Vector3(
                environment.transform.position.x,
                goalY,
                environment.transform.position.z), 30 * Time.deltaTime);
            yield return null;
        }

        while(running)
        {
            yield return null;
        }

        //Return Environment
        while (environment.transform.position.y != originY)
        {
            environment.transform.position = Vector3.MoveTowards(environment.transform.position, new Vector3(
                environment.transform.position.x,
                originY,
                environment.transform.position.z), 30 * Time.deltaTime);
            yield return null;
        }

        cleanup2 = true;
    }

    public bool doneCleaning()
    {
        return cleanup1 && cleanup2;
    }
}
