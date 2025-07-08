using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;


public class PreyMovement : MonoBehaviour
{

    [SerializeField] private GameObject prey;
    private UnityEngine.Rigidbody rigidBody;
    private float posX;
    private float posZ;
    public float fov = 5;
    public float angle = 90;
    private Vector3 newPosition;
    private Vector3 direction;
    private Quaternion rotation;
    private Vector3 predPos;
    private Vector3 preyPos;
    public float predFOV = 10f;
    public float predAngle = 45f;
    public float targetTime;
    // Start is called before the first frame update
    void Start()
    {
        //Arbitrarily define a new vector. Will be changed after if statement runs
        newPosition = new Vector3(prey.transform.position.x, prey.transform.position.y, prey.transform.position.z);
        rigidBody = GetComponent<Rigidbody>();
    }
    private void OnDrawGizmos()
    {
        //Draw FOV
        fov = 5;
        angle = 90;
        Handles.color = new Color(0, 1, 0, .3f);
        Handles.DrawSolidArc(transform.position, transform.up, Quaternion.AngleAxis(angle / 2, transform.up) * transform.right, angle, fov);
        //Reverse FOV color used for debugging purposes.
        Handles.color = new Color(0, 0, 1, 0);
        //Draw Pred's FOV onto prey. This will be used to see if prey needs to flee
        predFOV = 10;
        predAngle = 45f;
        Handles.DrawSolidArc(transform.position, transform.up, Quaternion.AngleAxis(predAngle * 1.5f, transform.up) * -transform.right, predAngle, predFOV);
    }
    private void FindNewPosition()
    {
        //Find a new destination
        posX = Random.Range(-20f, 20f);
        posZ = Random.Range(-20f, 20f);
        newPosition = new Vector3(posX, 0.51f, posZ);
        targetTime = 3f;
    }
    public int vision()
    {
        bool wallInFOV = false;
        bool predInFOV = false;
        bool inPredFOV = false;
        bool preyInFOV = false;
        //Detect collisions within Prey's sight
        Collider[] tragetsInFOV = Physics.OverlapSphere(prey.transform.position, fov);
        //Classify collisions detected
        foreach (Collider collider in tragetsInFOV)
        {
            if (collider.CompareTag("Wall"))
            {
                wallInFOV = true;
            }
            if (collider.CompareTag("MovingWall"))
            {
                wallInFOV = true;
            }
            if (collider.CompareTag("Pred"))
            {
                predInFOV = true;
            }
            if (collider.CompareTag("Prey") && collider.transform.position != prey.transform.position)
            {
                preyInFOV = true;
                preyPos = collider.transform.position;

            }
        }
        //Detect collisions behind prey (ie is they prey in a pred's FOV)
        Collider[] tragetsInPredFOV = Physics.OverlapSphere(prey.transform.position, fov);
        foreach (Collider collider in tragetsInPredFOV)
        {
            //If prey is in predator FOV
            if (collider.CompareTag("Pred"))
            {
                //Get position of predator to be used later
                inPredFOV = true;
                predPos = collider.transform.position;
            }
        }
        //Return proper values
        if (inPredFOV)
        {
            return 3;
        }
        if (wallInFOV)
        {
            return 1;
        }
        else if (predInFOV)
        {
            return 2;
        }
        else if (preyInFOV)
        {
            return 4;
        }
        else
        {
            return 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        targetTime -= Time.deltaTime;
        //Check to see what the prey sees
        int alert = vision();
        //Check to see if prey reached desired destination
        if (((transform.position.x - newPosition.x) < 0.01f) && (transform.position.z - newPosition.z < .01f))
        {
            FindNewPosition();
        }
        //If prey sees another prey
        if (alert == 4 && targetTime < 0)
        {
            //Flock
            newPosition = preyPos;
            newPosition.x -= 3;
            newPosition.z -= 3;
        }
        //If the prey sees a wall or a predator
        if (alert ==1 || alert == 2)
        {
          //Turn 180 degress
          newPosition = new Vector3(-prey.transform.position.x, 0.51f, posZ);
            targetTime = 3f;

        }

        //If the prey is in the predator's vision
        if (alert == 3)
        {
            //Enter Flee mode
            newPosition = prey.transform.position - predPos;
            newPosition.y = .51f;
        }
        //Move prey
        prey.transform.position = Vector3.MoveTowards(prey.transform.position, newPosition, .035f);
        //Rotate prey
        direction = (prey.transform.position - newPosition).normalized;
        rotation = Quaternion.LookRotation(direction);
        prey.transform.rotation = Quaternion.Slerp(prey.transform.rotation, rotation, 1);

    }
}
