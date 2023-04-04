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
        textObj = GameObject.Find("TextExits").GetComponent<Text>();
        t = transform.GetComponent<Transform>();
        t.localPosition = new Vector2(1, 1);
        env = SimEnvironment.Instance;
        textObj.text = "Exits: " + count;

        var host = "10.0.0.254";
        var port = 1883;
        var tagrefresh = 24;

        if (Uselog)
            env.Initialize(filename, tagrefresh);
        else
            env.Initialize(host, port, filename, 24);

        Tag T1 = env.newTag("5772", tagrefresh);
        //Tag T2 = env.newTag("7012", tagrefresh);
        Tag T2 = env.newTag("6985", tagrefresh);

        S = new SimObject();

        S.AddTag(T1);
        S.AddTag(T2);

        env.StartEnvironment();

        while (!env.ConnectedStatus) ;
        S.Calibrate(0.0f, -37.2f, 0.0f, (0.0393701f));
        //S.Scale = 0.0393701f;
        //S.Calibrate();

    }

    // Update is called once per frame
    void Update()
    {
        var V = new Vector3(S.Position.x, S.Position.y, -1.0f);
        var O = new Vector3(0.0f, 0.0f, S.Orientation.z * -(180/ Mathf.PI));
        t.localPosition = V;
        t.transform.eulerAngles = O;
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
