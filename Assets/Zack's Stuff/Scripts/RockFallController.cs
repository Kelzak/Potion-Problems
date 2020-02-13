using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockFallController : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject rockPrefab;
    public GameObject highlightPrefab;
    public Transform floor;
    public Material innerHighlight;

    [Header("Variables")]
    private float fallSpeed = 30f;
    private float fallInterval = 1f;

    private PlayerBehaviour player;

    private MeshRenderer floorRend;
    private float minSpawnX, maxSpawnX;
    private float minSpawnZ, maxSpawnZ;
    private const float SPAWN_Y = 0.6f;
    private List<Meteor> activeMeteors;


    private struct Meteor
    {
        public GameObject obj;
        public GameObject highlight;
        public GameObject innerHighlight;
    }

    // Start is called before the first frame update
    void Start()
    {
        floorRend = floor.GetComponent<MeshRenderer>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerBehaviour>();

        activeMeteors = new List<Meteor>();

        minSpawnX = floor.position.x - floorRend.bounds.extents.x;
        maxSpawnX = floor.position.x + floorRend.bounds.extents.x;
        minSpawnZ = floor.position.z - floorRend.bounds.extents.z;
        maxSpawnZ = floor.position.z + floorRend.bounds.extents.z;
    }

    public void StartMeteorFall()
    {
        StartCoroutine(MeteorFall());
    }

    public void HitPlayer()
    {
        if(!fling)
        { 
            StartCoroutine(FlingPlayer());
        }
    }

    private IEnumerator MeteorFall()
    {
        StartCoroutine(RunTutorial());

        while (GameController.currentEffect == GameController.Effects.ScreenShake)
        {

            if (CanSpawn())
            {
                SpawnMeteor();
            }

            //Rocks fall
            foreach(Meteor x in activeMeteors)
            {
                x.obj.transform.position = Vector3.MoveTowards(x.obj.transform.position, x.highlight.transform.position, fallSpeed * Time.deltaTime);
                x.innerHighlight.transform.localScale = new Vector3(Mathf.Lerp(4, 0.1f, Vector3.Distance(x.highlight.transform.position, x.obj.transform.position)/50), x.innerHighlight.transform.localScale.y, Mathf.Lerp(4, 0.1f, Vector3.Distance(x.highlight.transform.position, x.obj.transform.position) / 50));
                if (x.obj.transform.position == x.highlight.transform.position)
                {
                    Destroy(x.obj);
                    Destroy(x.highlight);
                    Destroy(x.innerHighlight);
                    activeMeteors.Remove(x);
                    break;
                }
            }


            yield return null;
        }

        //Meteors are done, time to cleanup
        while (activeMeteors.Count != 0)
        {
            foreach (Meteor x in activeMeteors)
            {
                Destroy(x.obj);
                Destroy(x.highlight);
                Destroy(x.innerHighlight);
                activeMeteors.Remove(x);
                break;
            }
            yield return null;
        }
        yield return null;
    }

    public bool fling = false;
    public IEnumerator FlingPlayer()
    {
        fling = true;
        player.GetComponentInChildren<PlayerInteraction>().ForceDrop();
        player.GetComponentInChildren<PlayerInteraction>().canPickup = false;
        player.playerCanMove = false;
        player.GetComponent<Rigidbody>().isKinematic = true;
        GameObject cauldron = GameObject.FindGameObjectWithTag("Cauldron");
        Vector3 targetPos = cauldron.transform.position;
        targetPos.y = player.transform.position.y;
        targetPos.z -= 1.5f;

        float rotationSpeed = 1000f;
        Quaternion originalRotation = player.transform.rotation;

        float flySpeed = 10;
        float maxHeight = player.transform.position.y + 10;
        float originalDistance = Vector3.Distance(player.transform.position, targetPos);
        float currentDistance;

        Quaternion spinRotation = Random.rotation;
        //FLING PLAYER TO SPOT BY CAULDRON
        while (Mathf.Approximately(Vector3.Distance(player.transform.position, targetPos), 0) == false || player.transform.rotation != spinRotation)
        {
            if (player.transform.rotation == spinRotation)
            {
                Quaternion lastSpinRotation = spinRotation;
                while (Quaternion.Equals(spinRotation = Random.rotation, lastSpinRotation))
                {
                    lastSpinRotation = spinRotation;
                    yield return null;
                }
            }
            //while()
            //{
            //    if (Mathf.Approximately(Vector3.Distance(player.transform.position, targetPos), 0) == true)
            //    {
            //        break;
            //    }

                //Manipulate height in fling
                currentDistance = Vector3.Distance(player.transform.position, targetPos);
                Vector3 tempTarget = targetPos;
                tempTarget.y = Mathf.Lerp(targetPos.y, maxHeight, currentDistance/25);
                player.transform.position = Vector3.MoveTowards(player.transform.position, tempTarget, flySpeed * Time.deltaTime);
                
                


                //Manipulate Rotation
                if((currentDistance < 1))
                {
                    spinRotation = originalRotation;
                }
                player.transform.rotation = Quaternion.RotateTowards(player.transform.rotation, spinRotation, rotationSpeed * Time.deltaTime);
                if((Mathf.Approximately(currentDistance,0)))
                {
                    player.transform.rotation = spinRotation;
                }
            Debug.Log("Inner Loop Ending " + Time.fixedTime);
                yield return null;
            //}
        }
        Debug.Log("After Loop Running " + Time.fixedTime);
        player.transform.position = targetPos;
        player.transform.rotation = originalRotation;

        player.GetComponent<Rigidbody>().isKinematic = false;
        player.GetComponentInChildren<PlayerInteraction>().canPickup = true;
        player.playerCanMove = true;
        fling = false;
    }

    bool canSpawn = true;
    private IEnumerator SpawnTimerRefresh()
    {
        canSpawn = false;
        yield return new WaitForSeconds(fallInterval);
        canSpawn = true;
    }

    private bool CanSpawn()
    {
        return canSpawn && !tutorialInProgress;
    }

    private void SpawnMeteor(Vector3 pos)
    {
        Vector3 spawnPoint = pos;
        spawnPoint.y = SPAWN_Y;

        //If position is a funny haha joke then make up a real position instead
        if (pos.x == -420 && pos.y == -69)
        {
            int spawnOnPlayer = Random.Range((int)0, (int)3);

            if (spawnOnPlayer == 0)
                spawnPoint = new Vector3(Random.Range(minSpawnX, maxSpawnX), SPAWN_Y, Random.Range(minSpawnZ, maxSpawnZ));
            else
            {
                float currX = Input.GetAxis("Horizontal");
                float currY = Input.GetAxis("Vertical");

                Vector3 direction = new Vector3(currX, 0, currY).normalized;

                spawnPoint = GameObject.FindGameObjectWithTag("Player").transform.position;
                spawnPoint.y = 0;
                spawnPoint += direction * GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerBehaviour>().playerSpeed;
                spawnPoint.y = SPAWN_Y;
            }
        }


        Meteor newMeteor = new Meteor();
        newMeteor.highlight = Instantiate<GameObject>(highlightPrefab, spawnPoint, Quaternion.identity);
        spawnPoint.y += 0.1f;
        newMeteor.innerHighlight = Instantiate<GameObject>(highlightPrefab, spawnPoint, Quaternion.identity);
        newMeteor.innerHighlight.GetComponent<Renderer>().material = innerHighlight;
        spawnPoint.y += 50;
        newMeteor.obj = Instantiate<GameObject>(rockPrefab, spawnPoint, Quaternion.identity);
        newMeteor.obj.AddComponent<MeteorBehaviour>();
        newMeteor.obj.GetComponent<MeteorBehaviour>().controller = this;
        activeMeteors.Add(newMeteor);
        StartCoroutine(SpawnTimerRefresh());
    }

    private void SpawnMeteor()
    {
        SpawnMeteor(new Vector3 (-420, -69, 0));
    }

    private bool tutorialInProgress = false;
    private IEnumerator RunTutorial()
    {
        tutorialInProgress = true;
        SpawnMeteor(new Vector3(-7, 0, -6));
        yield return new WaitForSeconds(0.5f);
        SpawnMeteor(new Vector3(-7, 0, -11));
        yield return new WaitForSeconds(0.5f);
        SpawnMeteor(new Vector3(-7, 0, -15.5f));

        yield return new WaitForSeconds(fallInterval);
        tutorialInProgress = false;
    }
}
