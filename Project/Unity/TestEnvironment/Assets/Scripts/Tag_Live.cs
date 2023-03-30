using Cinemachine;
using PozyxPositioner;
using PozyxPositioner.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Tag_Live : MonoBehaviour
{
    public int numTags;
    public int tagRefreshRate;

    SimEnvironment sim = SimEnvironment.Instance;

    [System.Serializable]
    public class TagIDs
    {
        public string[] tagIDList;
    }
    public TagIDs[] objects;

    public GameObject TagPrefab;
    public GameObject TagMarker;
    List<GameObject> tagList = new List<GameObject>();

    GameObject temp;
    GameObject baseObj;

    int count = 0;

    // Start is called before the first frame update
    void Start()
    {
        var targetGroup = GameObject.Find("TargetGroup").GetComponent<CinemachineTargetGroup>();

        var host = "10.0.0.254";
        var port = 1883;

        //sim.Initialize(host, port, "Dat.txt", tagRefreshRate);
        sim.Initialize("Assets/Data/FailureRun.txt", 5);

        foreach (var obj in objects)
        {
            var color = UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
            foreach (var tagID in obj.tagIDList)
            {
                sim.newTag(tagID, tagRefreshRate);

                // Add tag to Unity
                baseObj = new GameObject(tagID);
                GameObject t = Instantiate(TagPrefab);
                t.transform.parent = baseObj.transform;
                t.name = tagID;
                t.GetComponent<Renderer>().material.color = color;
                t.SetActive(false);
                tagList.Add(t);

                // Cinemachine camera tracking
                targetGroup.AddMember(t.transform, 1, 0.25f);

                // Empty game object to organize history
                temp = new GameObject(tagID + "_History");
                temp.transform.parent = baseObj.transform;
            }
        }

        sim.StartEnvironment();

        while (!sim.ConnectedStatus);
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var id in sim.TagIDs)
        {
            var position = sim.GetTag(id).Position;

            Debug.Log(position.x);
            Debug.Log(position.y);
            Debug.Log(position.z);

            temp = tagList.Find(x => x.name == id);

            //Activate tag
            if (temp.activeSelf == false)
            {
                temp.SetActive(true);
            }

            // Place a marker
            GameObject th = Instantiate(TagMarker);
            th.transform.position = temp.transform.position;
            th.GetComponent<Renderer>().material.color = temp.GetComponent<Renderer>().material.GetColor("_Color");
            th.name = temp.name + "_" + count.ToString();
            th.transform.parent = GameObject.Find(temp.name + "_History").transform;

            // Move tag
            temp.transform.position = new Vector3(position.x /1000f, position.z /1000f, position.y / 1000f);
        }
                
    }
}
