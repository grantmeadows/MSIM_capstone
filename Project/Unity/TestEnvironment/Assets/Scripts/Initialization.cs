using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Initialization : MonoBehaviour
{
    private static Initialization _instance;
    public static Initialization Instance { get { return _instance; } }
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    public Anchor_Generation ag;
    public Tag_Application ta;
    public Room_Generation rg;

    // Start is called before the first frame update
    void Start()
    {
        ag.AnchorStart();
        ta.TagStart();
        rg.RoomStart();
        Debug.Break();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            rg.RoomSave();
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            rg.ResetRoom();
        }
    }
}
