using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookSpawnerBehavior : MonoBehaviour
{
    public GameObject[] books;
    public float seconds;
    private GameController gameController;

    // Start is called before the first frame update
    void Start()
    {
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
    }

    public void Run()
    {
        StartCoroutine(WaitToStartSpawning());
    }

    IEnumerator WaitToStartSpawning()
    {
        yield return new WaitForSeconds(Random.Range(0f,5f));
        StartCoroutine(SpawnBook());
    }

    IEnumerator SpawnBook()
    {
        Instantiate(books[Random.Range(0,books.Length)], transform.position, transform.rotation);
        yield return new WaitForSeconds(seconds);
        if(gameController.blackHoleRunning)
        {
            StartCoroutine(SpawnBook());
        }
    }
}
