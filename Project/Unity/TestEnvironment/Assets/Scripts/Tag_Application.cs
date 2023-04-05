using Cinemachine;
using PozyxPositioner;
using PozyxPositioner.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Xml.Serialization;
using Unity.VisualScripting;
using UnityEngine;

public class Tag_Application : MonoBehaviour
{
    public string fileName;
    public int tagRefreshRate;

    SimEnvironment env;

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

    private int[] count;
    private int fileCount = 0;
    private float time;

    // Start is called before the first frame update
    void Start()
    {
        var host = "10.0.0.254";
        var port = 1883;
        fileName = "Assets/Data/" + fileName;
        time = 0;

        VisualizeTags();


        env = SimEnvironment.Instance;
        //env.Initialize(host, port, fileName, tagRefreshRate);
        env.Initialize(fileName, tagRefreshRate);

        foreach (var obj in objects)
        {
            foreach (var tagID in obj.tagIDList)
            {
                env.newTag(tagID, tagRefreshRate);
            }
        }

        env.StartEnvironment();

        while (!env.ConnectedStatus);
    }

    // Loads gameObjects for each tag and creates a list of gameObjects.
    void VisualizeTags()
    {
        var targetGroup = GameObject.Find("TargetGroup").GetComponent<CinemachineTargetGroup>();
        
        foreach (var obj in objects)
        {
            var color = UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
            foreach (var tagID in obj.tagIDList)
            {
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

        count = new int[tagList.Count];
    }

    // Update is called once per frame
    void Update()
    {
        int tagNum = 0;
        foreach (var id in env.TagIDs)
        {
            var position = env.GetTag(id).Position;
            if (position.x != 0 && position.y != 0 && position.z != 0)
            {
                temp = tagList.Find(x => x.name == id);

                if (Vector3.Distance(temp.transform.position, new Vector3(position.x / 1000f, position.z / 1000f, position.y / 1000f)) > 0.01)
                {
                    //Activate tag
                    if (temp.activeSelf == false)
                    {
                        temp.SetActive(true);
                    }

                    // Move tag
                    temp.transform.position = new Vector3(position.x / 1000f, position.z / 1000f, position.y / 1000f);

                    // Place a marker
                    GameObject th = Instantiate(TagMarker);
                    th.transform.position = temp.transform.position;
                    th.GetComponent<Renderer>().material.color = temp.GetComponent<Renderer>().material.GetColor("_Color");
                    th.name = temp.name + "_" + count[tagNum].ToString();
                    th.transform.parent = GameObject.Find(temp.name + "_History").transform;
                    count[tagNum]++;
                }
            }
            tagNum++;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ResetTags();
        }
        else if (Input.GetKeyDown(KeyCode.P))
        {
            PrintPositions();
        }  
    }

    private void PrintPositions() 
    {
        foreach (var id in tagList)
        {
            File.AppendAllText("Assets/Data/" + id.name.ToString() + "_" + fileCount.ToString() + ".txt", "Time, X, Y, Z");
            foreach (Transform child in GameObject.Find(id.name + "_History").transform)
            {
                string outputText = (child.gameObject.GetComponent<Time_Storage>().getTime() - time).ToString() + ", " + 
                    child.position.x.ToString() + ", " + 
                    child.position.y.ToString() + ", " + 
                    child.position.z.ToString() + "\n";
                File.AppendAllText("Assets/Data/" + id.name.ToString() + "_" + fileCount.ToString() + ".txt", outputText);
            }
        }
        fileCount++;
    }

    private void ResetTags()
    {
        foreach (var id in tagList)
        {
            foreach(Transform child in GameObject.Find(id.name + "_History").transform)
            {
                Destroy(child.gameObject);
            }
            id.GetComponent<TrailRenderer>().Clear();
        }
        time = Time.time;
    }
}
