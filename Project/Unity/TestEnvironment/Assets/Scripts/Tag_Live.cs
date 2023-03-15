using Cinemachine;
using PozyxSubscriber;
using PozyxSubscriber.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tag_Live : MonoBehaviour
{
    public int tagRefreshRate;

    [Serializable]
    public class TagIDs
    {
        public string[] tagIDList;
    }
    public TagIDs[] objects;

    SimEnvironment sim = SimEnvironment.Instance;

    public GameObject TagPrefab;
    public GameObject TagMarker;
    GameObject temp;
    GameObject baseObj;

    // Start is called before the first frame update
    void Start()
    {
        var targetGroup = GameObject.Find("TargetGroup").GetComponent<CinemachineTargetGroup>();

        var host = "10.0.0.254";
        var port = 1883;
        var numTags = 0;

        foreach (var obj in objects)
        {
            foreach (var tagID in obj.tagIDList)
            {
                sim.NewTag(tagID, tagRefreshRate);

                baseObj = new GameObject(tagID);
                GameObject t = Instantiate(TagPrefab);
                t.transform.parent = baseObj.transform;
                t.name = tagID;
                t.GetComponent<Renderer>().material.color = UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
                t.SetActive(false);

                targetGroup.AddMember(t.transform, 1, 0.25f);

                temp = new GameObject(tagID + "_History");
                temp.transform.parent = baseObj.transform;

                numTags++;
            }
        }

        sim.Initialize(host, port, objects.Length, numTags, "Dat.txt", tagRefreshRate);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
