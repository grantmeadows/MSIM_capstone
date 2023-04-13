using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Distance : MonoBehaviour
{
    public GameObject Object1;
    public GameObject Object2;
    private float distance;

    // Start is called before the first frame update
    void Start()
    {
        distance = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        distance = Vector3.Distance(Object1.transform.position, Object2.transform.position);
        transform.GetChild(0).GetComponent<TMPro.TextMeshPro>().text = Object1.name + " " + Object2.name + "<br>" + distance.ToString("F3");
    }
}
