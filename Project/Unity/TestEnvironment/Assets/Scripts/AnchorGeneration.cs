using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class AnchorGeneration : MonoBehaviour
{
    public GameObject AnchorObj;
    public Vector3[] AnchorLoc;

    void Start()
    {
        for (int i = 0; i < AnchorLoc.Length; i++)
        {
            GameObject a = Instantiate(AnchorObj);
            a.transform.position = AnchorLoc[i];
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
