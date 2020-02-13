using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemAIBehavior : MonoBehaviour
{

    public float moveSpeed = 10; // How fast the bot moves
    public bool beingHeld;

    public List<Transform> destinationNodes = new List<Transform>(new Transform[8]); //List of places bot can go to

    [Header("NAV POINT STUFF")]
    private int _nextNodeIndex; // The index currently traveling to.
    public Transform nextNode; // The node currently traveling to.

    private int _previousNodeIndex; //the index previously visited, really only used for non-linear maps
    public Transform previousNode; //the node previously visited, really only used for non-linear maps
    private bool isRandom = true;
 

    private Rigidbody _objectRigidbody;

    private float _startTime = 0f;
    private float _journeyLength = 1f;

    // Start is called before the first frame update
    void Start()
    {

        _objectRigidbody = gameObject.GetComponent<Rigidbody>();
        SelectFirstNode();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!beingHeld)
        {
            float distCovered = (Time.time - _startTime) * moveSpeed;
            // Fraction of journey completed = current distance divided by total distance.
            float fracJourney = distCovered / _journeyLength;
            transform.position = Vector3.Lerp(previousNode.position, nextNode.position, fracJourney);
            if(transform.position == nextNode.position)
            {
                SelectNextNode();
            }
            _objectRigidbody.isKinematic = true;
        }
        else
        {
            _objectRigidbody.isKinematic = false;
        }
    }

    private void SelectFirstNode()
    {
        previousNode = gameObject.transform;
        _previousNodeIndex = 0;
        nextNode = destinationNodes[0];
        _nextNodeIndex = 0;     
        gameObject.transform.LookAt(previousNode); //Placed here to avoid it happening in Update
        _startTime = Time.time;
        _journeyLength = Vector3.Distance(previousNode.position, nextNode.position);
    }

    private void SelectNextNode()
    {
        if (!isRandom)
        {
            previousNode = nextNode;
            _previousNodeIndex = _nextNodeIndex;
            if (_previousNodeIndex + 1 >= destinationNodes.Count)
            {
                //Checks to see if we've reached the end of the list, loops back around to the start.
                nextNode = destinationNodes[0];
                _nextNodeIndex = 0;
            }
            else
            {
                //Haven't reached the end of the list yet.
                nextNode = destinationNodes[_previousNodeIndex + 1];
                _nextNodeIndex = _previousNodeIndex + 1;
            }
            
        }
        else
        {
            int randNum = Random.Range(1, destinationNodes.Count - 1); //Chooses initial random node, avoids needing a non-recursive version of SelectRandomNextNode().
            previousNode = nextNode;
            _previousNodeIndex = _nextNodeIndex;

            SelectRandomNextNode(randNum); //Recursively finds a suitable random next node.          
        }
        gameObject.transform.LookAt(nextNode); //Placed here to avoid it happening in Update
        _startTime = Time.time;
        _journeyLength = Vector3.Distance(previousNode.position, nextNode.position);
    }

    private void SelectRandomNextNode(int nodeIndex)//Recursive function that ensures the hotspot bot doesn't go back and forth between nodes.
    {
        if(nodeIndex == _previousNodeIndex) //checks if the next random node was the same as the previous one (we don't want this)
        {
            int randNum = Random.Range(1, destinationNodes.Count - 1);
            SelectRandomNextNode(randNum);
        }
        else // Break condition;
        {
            nextNode = destinationNodes[nodeIndex];
            _nextNodeIndex = nodeIndex;
        }     
    }
}
