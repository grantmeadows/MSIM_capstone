using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;


public class RoomGeneration : MonoBehaviour
{
    public GameObject AnchorObj;
    public GameObject AnchorZero;
    public Vector3[] AnchorLoc;
    public Material RoomMaterial;

    void Start()
    {
        // Anchor Placement
        for (int i = 0; i < AnchorLoc.Length; i++)
        {
            GameObject a = Instantiate(AnchorObj);
            a.transform.position = AnchorLoc[i];
            a.transform.parent = AnchorZero.transform;
        }

        // Floor Placement
        ProBuilderMesh quad = ProBuilderMesh.Create(
            new Vector3[]
            {
                new Vector3(AnchorLoc[0].x, 0f, AnchorLoc[0].z),
                new Vector3(AnchorLoc[1].x, 0f, AnchorLoc[1].z),
                new Vector3(AnchorLoc[2].x, 0f, AnchorLoc[2].z),
                new Vector3(AnchorLoc[3].x, 0f, AnchorLoc[3].z)
            },
            new Face[] { new Face(new int[] { 0, 1, 2, 1, 3, 2 }) }
            );
        quad.GetComponent<MeshRenderer>().material = RoomMaterial;
    }

    // Update is called once per frame
    void Update()
    {

    }
}