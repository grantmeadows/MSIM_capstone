using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Look_At : MonoBehaviour
{

    Camera CamToLookAt;

    // Start is called before the first frame update
    void Start()
    {
        CamToLookAt = Camera.main;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.LookAt(CamToLookAt.transform);
        transform.rotation = Quaternion.LookRotation(CamToLookAt.transform.forward);
    }
}
