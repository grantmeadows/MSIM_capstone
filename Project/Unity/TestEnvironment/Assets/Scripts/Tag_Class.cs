using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class My_Tag
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
}

public class Anchordata
{
    public string tagId { get; set; }
    public string anchorId { get; set; }
    public float rss { get; set; }
}

public class Tag_Class : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
