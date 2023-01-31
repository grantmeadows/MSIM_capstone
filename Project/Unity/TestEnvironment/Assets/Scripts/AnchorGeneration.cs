using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class AnchorGeneration : MonoBehaviour
{
    public GameObject AnchorObj;
    public GameObject AnchorZero;
    public Vector3[] AnchorLoc;

    void Start()
    {
        for (int i = 0; i < AnchorLoc.Length; i++)
        {
            GameObject a = Instantiate(AnchorObj);
            a.transform.position = AnchorLoc[i];
            a.transform.parent = AnchorZero.transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
