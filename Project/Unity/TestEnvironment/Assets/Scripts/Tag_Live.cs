using Cinemachine;
using PozyxSubscriber;
using PozyxSubscriber.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Tag_Live : MonoBehaviour
{
    public int numTags;
    public int tagRefreshRate;

    [System.Serializable]
    public class TagIDs
    {
        public string[] tagIDList;
    }
    public TagIDs[] objects;

    SimEnvironment sim = SimEnvironment.Instance;

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

        sim.Initialize(host, port, objects.Length, numTags, "Dat.txt", tagRefreshRate);

        foreach (var obj in objects)
        {
            var color = UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
            foreach (var tagID in obj.tagIDList)
            {
                // Add tag to framework
                sim.NewTag(tagID, tagRefreshRate);

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
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var id in sim.GetTagIDs())
        {
            count++;
            var position = sim.getLatestposition(id);

            if (position.good == true)
            {
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
                temp.transform.position = new Vector3(position.pos.x /1000, position.pos.z /1000, position.pos.y / 1000);
            }
        }
                
    }
}
