using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using System.Linq;
using Cinemachine;

public class Tag_Log: MonoBehaviour
{
    double simStart;
    int count = 0;
    public float SimulationSpeed;

    public GameObject TagPrefab;
    public GameObject TagMarker;
    GameObject temp;
    GameObject baseObj;

    public string FileName;
    string FilePath;

    List<My_Tag> tagData = new List<My_Tag>();
    List<GameObject> tagList = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        var targetGroup = GameObject.Find("TargetGroup").GetComponent<CinemachineTargetGroup>();

        FilePath = "Assets/Data/" + FileName + ".txt";
        using (var sr = new StreamReader(FilePath))
        {
            while (sr.Peek() >= 0)
            {
                var tObj = JsonConvert.DeserializeObject<List<My_Tag>>(sr.ReadLine());
                System.DateTimeOffset dateTimeOff = System.DateTimeOffset.FromUnixTimeSeconds(tObj[0].timestamp);
                tagData.Add(tObj[0]);
            }
            foreach (string id in tagData.Select(x => x.tagId).Distinct())
            {
                baseObj = new GameObject(id);
                GameObject t = Instantiate(TagPrefab);
                t.transform.parent = baseObj.transform;
                t.name = id;
                t.GetComponent<Renderer>().material.color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
                t.SetActive(false);
                tagList.Add(t);

                targetGroup.AddMember(t.transform, 1, 0.25f);

                temp = new GameObject(id + "_History");
                temp.transform.parent = baseObj.transform;
            }
        }

        Time.timeScale = SimulationSpeed;
        simStart = tagData[0].timestamp;
    }

    // FixedUpdate can be called multiple times per frame
    void FixedUpdate()
    {
        if (count < tagData.Count)
        {
            if (Time.fixedTimeAsDouble + simStart >= tagData[count].timestamp)
            {
                temp = tagList.Find(x => x.name == tagData[count].tagId);

                if (tagData[count].success == true)
                {
                    if (temp.activeSelf == false)
                    {
                        temp.SetActive(true);
                    }
                    else 
                    {
                        GameObject th = Instantiate(TagMarker);
                        th.transform.position = temp.transform.position;
                        th.GetComponent<Renderer>().material.color = temp.GetComponent<Renderer>().material.GetColor("_Color");
                        th.name = temp.name + "_" + count.ToString();
                        th.transform.parent = GameObject.Find(temp.name + "_History").transform;
                    }

                    temp.transform.position = new Vector3(
                        tagData[count].data.coordinates.x / 1000,
                        tagData[count].data.coordinates.z / 1000,
                        tagData[count].data.coordinates.y / 1000
                        );

                    Debug.Log(
                        tagData[count].tagId.ToString() + " " +
                        (tagData[count].data.coordinates.x / 1000).ToString() + " " +
                        (tagData[count].data.coordinates.z / 1000).ToString() + " " +
                        (tagData[count].data.coordinates.y / 1000).ToString() + " ");
                }
                else
                {
                    Debug.Log(tagData[count].tagId.ToString() + " Count: " + count.ToString() + " Failure.");
                }

                count++;
            }
        }
        else
        {
            Debug.Break();
        }
    }
}