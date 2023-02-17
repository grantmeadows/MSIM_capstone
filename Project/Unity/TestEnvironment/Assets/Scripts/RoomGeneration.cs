using UnityEngine;
using UnityEngine.ProBuilder;


public class RoomGeneration : MonoBehaviour
{
    public GameObject AnchorPrefab;
    public Vector3[] AnchorLoc;
    public Material RoomMaterial;

    void Start()
    {
        // Anchor Placement
        for (int i = 0; i < AnchorLoc.Length; i++)
        {
            GameObject a = Instantiate(AnchorPrefab);
            a.transform.position = AnchorLoc[i];
            a.transform.parent = this.transform;
            a.name = "Anchor " + i;
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
            new Face[] { new Face(new int[] { 0, 1, 2, 0, 2, 3 }) }
            );
        quad.GetComponent<MeshRenderer>().material = RoomMaterial;
        quad.transform.parent = this.transform;
        quad.name = "Floor";
    }

    // Update is called once per frame
    void Update()
    {

    }
}