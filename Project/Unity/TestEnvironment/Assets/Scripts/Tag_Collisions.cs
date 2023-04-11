using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tag_Collisions : MonoBehaviour
{
    float timeCollided;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Obstacle"))
        {
            timeCollided = Time.time;
            other.gameObject.GetComponent<Renderer>().material.color = GetComponent<Renderer>().material.color;
            Debug.Log(name + " collided with obstacle " + other.gameObject.name + " at time " + Time.time.ToString());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Obstacle"))
        {
            Debug.Log(name + " left after colliding for  " + (Time.time - timeCollided).ToString());
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
