using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PozyxPositioner;
using PozyxPositioner.Framework;
using UnityEngine.UI;
using System.IO;

public class SimObjMov : MonoBehaviour
{

    private Transform t;
    public int tagrefresh;
    public string tag1ID;
    public string tag2ID;
    private Vector3 startingPos;
    private float startingO;
    private SimEnvironment env;
    private SimObject S;
    private Text textObj;
    private int count;
    public string filename;
    public bool Uselog;


    // Start is called before the first frame update
    void Start()
    {
        count = 0;
        //textObj = GameObject.Find("TextExits").GetComponent<Text>();
        t = transform.GetComponent<Transform>();
        startingPos = transform.position;
        startingO = t.eulerAngles.z;
        t.localPosition = new Vector2(1, 1);
        env = SimEnvironment.Instance;
        //textObj.text = "Exits: " + count;

        var host = "10.0.0.254";
        var port = 1883;

        if (Uselog)
            env.Initialize(filename, tagrefresh);
        else
            env.Initialize(host, port, filename, tagrefresh);

        Tag T1 = env.newTag(tag1ID, tagrefresh);
        //Tag T2 = env.newTag("7012", tagrefresh);
        Tag T2 = env.newTag(tag2ID, tagrefresh);

        S = new SimObject();

        S.AddTag(T1);
        S.AddTag(T2);

        env.StartEnvironment();

        while (!env.ConnectedStatus) ;
        S.Calibrate(startingPos.x, startingPos.y, startingPos.z, 0.0393701f);
        //S.Scale = 0.0393701f;
        //S.Calibrate();

    }

    // Update is called once per frame
    void Update()
    {
        if (S.Calibrated)
        {
            var V = new Vector3(S.Position.x, S.Position.y, -1.0f);
            float or = (S.Orientation.z * -(180 / Mathf.PI)) + startingO;
            var O = new Vector3(0.0f, 0.0f, or);
            t.localPosition = V;
            t.transform.eulerAngles = O;
        }
        else
        {
            t.localPosition = startingPos;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Marker")
        {
            count++;
            textObj.text = "Exits: " + count;
        }
    }

}
