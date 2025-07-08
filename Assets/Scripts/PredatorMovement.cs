using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PredatorMovement : MonoBehaviour
{
    [SerializeField] private GameObject pred;
    private UnityEngine.Rigidbody rigidBody;
    private float posX;
    private float posZ;
    public float fov = 10;
    public float angle = 45;
    private Vector3 newPosition;
    private Vector3 direction;
    private Quaternion rotation;
    private float chaseX;
    private float chaseZ;
    // Start is called before the first frame update
    void Start()
    {
        //Arbitrarily define a new vector. Will be changed after if statement runs
        newPosition = new Vector3(pred.transform.position.x, pred.transform.position.y, pred.transform.position.z);
        rigidBody = GetComponent<Rigidbody>();
    }
    private void OnDrawGizmos()
    {
        //Draw FOV
        fov = 10;
        angle = 45;
        Handles.color = new Color(1, 0, 0, .3f);
        Handles.DrawSolidArc(transform.position, transform.up, Quaternion.AngleAxis(angle * 1.5f, transform.up) * transform.right, angle, fov);
    }
    private void FindNewPosition()
    {
        //Find a new destination
        posX = Random.Range(-20f, 20);
        posZ = Random.Range(-20f, 20f);
        newPosition = new Vector3(posX, 0.75f, posZ);
    }

    public int vision()
    {
        bool wallInFOV = false;
        bool preyInFOV = false;
        //Detects all collisions within FOV
        Collider[] tragetsInFOV = Physics.OverlapSphere(pred.transform.position, fov);
        foreach (Collider collider in tragetsInFOV)
        {
            //Classify collisions detected
            if (collider.CompareTag("MovingWall"))
            {
                wallInFOV = true;
            }
            if (collider.CompareTag("Prey"))
            {
                preyInFOV = true;
                //Check to see if predator is close enough to eat prey
                if (Vector3.Distance(collider.transform.position, pred.transform.position) <= 1.5f)
                {
                    Destroy(collider.gameObject);
                } else
                {
                    //If not close enough to destroy, chase. Set the x and z coords to be used later
                    chaseX = collider.transform.position.x;
                    chaseZ = collider.transform.position.z; 
                }
            }

        }
        //Return proper values
        if (preyInFOV)
        {
            return 2;
        }
        else if(wallInFOV)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Check to see what the pred sees
        int alert = vision();
        //Check to see if pred reached desired destination
        if (((transform.position.x - newPosition.x) < 0.01f) && (transform.position.z - newPosition.z < .01f))
        {
            FindNewPosition();
        }
        //If the pred sees a wall
        if (alert == 1)
        {
            //Turn around
            newPosition = new Vector3(-pred.transform.position.x, 0.75f, posZ);
        }
        //If the pred sees a prey
        if (alert == 2)
        {
            //Chase the prey
            newPosition = new Vector3(chaseX, 0.75f, chaseZ);
        }
        //Move pred
        pred.transform.position = Vector3.MoveTowards(pred.transform.position, newPosition, .01f);
        //Rotate pred to direction moving
        direction = (pred.transform.position - newPosition).normalized;
        rotation = Quaternion.LookRotation(direction);
        pred.transform.rotation = Quaternion.Slerp(pred.transform.rotation, rotation, 1);

    }
}

