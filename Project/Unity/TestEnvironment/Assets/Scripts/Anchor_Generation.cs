using Cinemachine;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.ProBuilder;


public class Anchor_Generation : MonoBehaviour
{
    public GameObject AnchorPrefab;
    public List<Vector3> AnchorLoc;

    public Material RoomMaterial;
    private List<int> FloorFaces = new List<int>();

    public void AnchorStart()
    {
        var targetGroup = GameObject.Find("TargetGroup").GetComponent<CinemachineTargetGroup>();
        float max_X = int.MinValue;
        float min_X = int.MaxValue;
        float max_Z = int.MinValue;
        float min_Z = int.MaxValue;

        // Anchor Placement
        for (int i = 0; i < AnchorLoc.Count; i++)
        {
            GameObject a = Instantiate(AnchorPrefab);
            a.transform.position = AnchorLoc[i];
            a.transform.parent = this.transform;
            a.name = "Anchor " + i;

            if (AnchorLoc[i].x > max_X)
            {
                max_X = AnchorLoc[i].x;
            }
            if (AnchorLoc[i].z > max_Z)
            {
                max_Z = AnchorLoc[i].z;
            }
            if (AnchorLoc[i].x < min_X)
            {
                min_X = AnchorLoc[i].x;
            }
            if (AnchorLoc[i].z < min_Z)
            {
                min_Z = AnchorLoc[i].z;
            }

            AnchorLoc[i] = new Vector3(AnchorLoc[i].x, 0f, AnchorLoc[i].z);

            FloorFaces.Add(AnchorLoc.Count);
            FloorFaces.Add(i);
            if(i < AnchorLoc.Count - 1)
            {
                FloorFaces.Add(i + 1);
            }
            else
            {
                FloorFaces.Add(0);
            }

            targetGroup.AddMember(a.transform, 1, 0.25f);
        }

        // Clockwise Anchor Floor Placement
        AnchorLoc.Add(new Vector3((max_X + min_X)/2f, 0f, (max_Z + min_Z)/2f));

        ProBuilderMesh quad = ProBuilderMesh.Create(
            AnchorLoc.ToArray(),
            new Face[] { new Face(FloorFaces.ToArray()) }
            );
        quad.GetComponent<MeshRenderer>().material = RoomMaterial;
        quad.GetComponent<MeshRenderer>().receiveShadows = false;
        quad.transform.parent = this.transform;
        quad.name = "Floor";
    }

    // Update is called once per frame
    void Update()
    {

    }
}