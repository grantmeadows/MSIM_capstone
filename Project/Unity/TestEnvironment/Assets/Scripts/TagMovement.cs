using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using System.Linq;
using Cinemachine;

public class Tag
{
    public string version { get; set; }
    public string tagId { get; set; }
    public long timestamp { get; set; }
    public Data data { get; set; }
    public bool success { get; set; }
}

public class Data
{
    public Metrics metrics { get; set; }
    public Tagdata tagData { get; set; }
    public Anchordata[] anchorData { get; set; }
    public int type { get; set; }
    public Coordinates coordinates { get; set; }
    public object[] zones { get; set; }
}

public class Metrics
{
    public int latency { get; set; }
    public Rates rates { get; set; }
}

public class Rates
{
    public float success { get; set; }
    public float update { get; set; }
    public float packetLoss { get; set; }
}

public class Tagdata
{
    public int blinkIndex { get; set; }
    public string status { get; set; }
    public object[] events { get; set; }
    public int[][] accelerometer { get; set; }
}

public class Coordinates
{
    public float x { get; set; }
    public float y { get; set; }
    public float z { get; set; }

    /*
    public Vector3 coord;

    public Coordinates()
    {
        coord = new Vector3(x / 100, z / 100, y / 100);
    }
    */
}

public class Anchordata
{
    public string tagId { get; set; }
    public string anchorId { get; set; }
    public float rss { get; set; }
}

public class TagMovement : MonoBehaviour
{
    double simStart;
    int count = 0;
    public float SimulationSpeed;

    public GameObject TagPrefab;
    public GameObject TagMarker;
    GameObject temp;

    public string FileName;
    string FilePath;

    List<Tag> tagData = new List<Tag>();
    List<GameObject> tagList = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        var tHistory = new GameObject("TagHistory");
        var targetGroup = GameObject.Find("TargetGroup").GetComponent<CinemachineTargetGroup>();

        FilePath = "Assets/Data/" + FileName + ".txt";
        using (var sr = new StreamReader(FilePath))
        {
            while (sr.Peek() >= 0)
            {
                var tObj = JsonConvert.DeserializeObject<List<Tag>>(sr.ReadLine());
                System.DateTimeOffset dateTimeOff = System.DateTimeOffset.FromUnixTimeSeconds(tObj[0].timestamp);
                tagData.Add(tObj[0]);
            }
            foreach (string id in tagData.Select(x => x.tagId).Distinct())
            {
                GameObject t = Instantiate(TagPrefab);
                t.transform.parent = this.transform;
                t.name = id;
                t.GetComponent<Renderer>().material.color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
                t.SetActive(false);
                tagList.Add(t);

                targetGroup.AddMember(t.transform, 1, 0.25f);

                temp = new GameObject(id + "_History");
                temp.transform.parent = tHistory.transform;
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
                        th.transform.parent = GameObject.Find("/TagHistory/" + temp.name + "_History").transform;
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