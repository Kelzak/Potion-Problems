using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    public float playerSpeed = 5;
    public float rotationSpeed = 500f;
    public Rigidbody rb;
    public PhysicMaterial icePhysics;
    public bool playerCanMove = true;
    public GameController GC;

    [Header("SnowMan Refrences")]
    public GameObject snowManModel;
    public GameObject playerModel;

 

    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        GC = GameObject.Find("GameController").GetComponent<GameController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerCanMove)
            Movement();
    }

    private void Movement()
    {
        Vector3 oldPos = transform.position;
        float xAxis = Input.GetAxis("Horizontal");
        float yAxis = Input.GetAxis("Vertical");
        Vector3 moveVector = new Vector3(xAxis, 0, yAxis);


        //normal way to move that happens as long as the ice effect is not present
        if (GameController.currentEffect != GameController.Effects.IceFloor)
        {
            transform.Translate(moveVector * playerSpeed * Time.deltaTime, Space.World);
        }


        else if (GameController.currentEffect == GameController.Effects.IceFloor)
        {
            int ingredientNumber;

            ingredientNumber = GC.ingredientsCollected;

            int icePlayerSpeed = 300;

            icePhysics.dynamicFriction = .2f / Mathf.Clamp(ingredientNumber, 1, Mathf.Infinity);

            Debug.Log("Add force based movement");
            rb.AddForce(moveVector * icePlayerSpeed * Time.deltaTime);
        }

        if (moveVector != Vector3.zero)
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(moveVector), 500f * Time.deltaTime);
    }


    void OnCollisionEnter(Collision other)
    {
       if (other.gameObject.CompareTag("Icycle"))  
        {
            playerCanMove = false;
            playerModel.SetActive(false);
            snowManModel.SetActive(true);
            StartCoroutine(PlayerCanMoveAgain());
        }
    }

    public IEnumerator PlayerCanMoveAgain()
    {
        yield return new WaitForSeconds(2);
        playerCanMove = true;
        playerModel.SetActive(true);
        snowManModel.SetActive(false);
    }
}
