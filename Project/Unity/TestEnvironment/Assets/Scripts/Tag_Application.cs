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
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Tag_Application : MonoBehaviour
{
    [Header("CHANGE PRIOR TO PLAY MODE")]
    public bool LiveTracking;
    public bool DisplayTags;
    [Space(10)]
    public string host;
    public int port;
    public string fileName;
    public int tagRefreshRate;

    [System.Serializable]

    public class TagIDs
    {
        public string[] tagIDList;
    }
    [Space(10)]
    [Header("INSERT TAG IDs")]
    public TagIDs[] objects;

    [Space(10)]
    public GameObject TagPrefab;
    public GameObject TagMarker;

    List<GameObject> tagList = new List<GameObject>();
    private List<SimObject> simObjs = new List<SimObject>();

    GameObject temp;
    GameObject baseObj;
    SimEnvironment env;

    private int[] count;
    private int num;
    private int fileCount = 0;
    private float time;
    private bool simRunning;

    // Start is called before the first frame update
    public void TagStart()
    {
        fileName = "Assets/Data/" + fileName;
        time = 0;
        simRunning = false;

        VisualizeTags();
    }

    // Loads gameObjects for each tag and creates a list of gameObjects.
    void VisualizeTags()
    {
        var targetGroup = GameObject.Find("TargetGroup").GetComponent<CinemachineTargetGroup>();

        if (DisplayTags == true)
        {
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
        }
        else
        {
            num = 0;
            foreach (var obj in objects)
            {
                var color = UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);

                // Add tag to Unity
                baseObj = new GameObject("Object_" + num.ToString());
                GameObject t = Instantiate(TagPrefab);
                t.transform.parent = baseObj.transform;
                t.name = "Object_" + num.ToString();
                t.GetComponent<Renderer>().material.color = color;
                t.SetActive(false);
                tagList.Add(t);

                // Cinemachine camera tracking
                targetGroup.AddMember(t.transform, 1, 0.25f);

                // Empty game object to organize history
                temp = new GameObject("Object_" + num.ToString() + "_History");
                temp.transform.parent = baseObj.transform;
            }
        }

        count = new int[tagList.Count];
    }

    // Update is called once per frame
    void Update()
    {
        if (simRunning == true)
        {
            num = 0;
            if (DisplayTags == true)
            {
                foreach (var id in env.TagIDs)
                {
                    var position = env.GetTag(id).Position;
                    if (position.x != 0 && position.y != 0 && position.z != 0)
                    {
                        temp = tagList.Find(x => x.name == id);

                        if (Vector3.Distance(temp.transform.position, new Vector3(position.x / 1000f, position.z / 1000f, position.y / 1000f)) > 0.001)
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
                            th.name = temp.name + "_" + count[num].ToString();
                            th.transform.parent = temp.transform.parent.GetChild(1);
                            count[num]++;
                        }
                    }
                    num++;
                }
            }
            else
            {
                foreach (var obj in objects)
                {
                    var position = simObjs[num].Position;
                    var orientation = simObjs[num].Orientation;

                    if (position.x != 0 && position.y != 0 && position.z != 0)
                    {
                        temp = tagList.Find(x => x.name == "Object_" + num.ToString());

                        if (Vector3.Distance(temp.transform.position, new Vector3(position.x / 1000f, position.z / 1000f, position.y / 1000f)) > 0.001)
                        {
                            //Activate tag
                            if (temp.activeSelf == false)
                            {
                                temp.SetActive(true);
                            }

                            // Move tag
                            temp.transform.position = new Vector3(position.x / 1000f, position.z / 1000f, position.y / 1000f);

                            // Rotate tag
                            // ??????????
                            temp.transform.localEulerAngles = new Vector3(orientation.x * 180f / 3.14159265f, orientation.z * 180f / 3.14159265f, orientation.y * 180f / 3.14159265f);
                            // ??????????

                            // Place a marker
                            GameObject th = Instantiate(TagMarker);
                            th.transform.position = temp.transform.position;
                            th.GetComponent<Renderer>().material.color = temp.GetComponent<Renderer>().material.GetColor("_Color");
                            th.name = temp.name + "_" + count[num].ToString();
                            th.transform.parent = GameObject.Find(temp.name + "_History").transform;
                            count[num]++;
                        }
                    }
                    num++;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetTags();
        }
        else if (Input.GetKeyDown(KeyCode.P))
        {
            PrintPositions();
        }  
        else if (Input.GetKeyDown(KeyCode.Space) && simRunning == false) 
        { 
            StartFramework();
        }
    }

    private void PrintPositions() 
    {
        foreach (var id in tagList)
        {
            File.AppendAllText("Assets/Data/" + id.name.ToString() + "_" + fileCount.ToString() + ".txt", "Time, X, Y, Z" + "\n");
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

    private void StartFramework()
    {
        env = SimEnvironment.Instance;

        if (LiveTracking == true)
        {
            env.Initialize(host, port, fileName, tagRefreshRate);
        }
        else
        {
            env.Initialize(fileName, tagRefreshRate);
        }

        num = 0;

        foreach (var obj in objects)
        {
            simObjs.Add(new SimObject());
            foreach (var tagID in obj.tagIDList)
            {
                simObjs[num].AddTag(env.newTag(tagID, tagRefreshRate));
            }
            num++;
        }

        env.StartEnvironment();

        while (!env.ConnectedStatus);

        simRunning = true;

        foreach (var obj in simObjs)
        {
            obj.Calibrate();
        }
    }
}
