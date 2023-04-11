using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tag_Collisions : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Obstacle"))
        {
            Debug.Log(name + " collided with obstacle" + other.gameObject.name);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
