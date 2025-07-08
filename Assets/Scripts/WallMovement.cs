using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class WallMovement : MonoBehaviour
{
    [SerializeField] private GameObject wall;
    private Vector3 direction = Vector3.back;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Movement for wall
        wall.transform.Translate(direction * .05f);

        //If at right side, siwtch movement to the left
        if (transform.position.z <= -20)
        {
            direction = Vector3.forward;
        }
        //If at left side, switch movement to the right
        else if (transform.position.z >= 20)
        {
            direction = Vector3.back;
        }
    }
}
