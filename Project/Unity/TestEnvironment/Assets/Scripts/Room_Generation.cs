using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class Room_Generation : MonoBehaviour
{
    private static Room_Generation _instance;
    public static Room_Generation Instance { get { return _instance; } }
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    public GameObject layout;

    public void RoomStart()
    {
        GameObject r = Instantiate(layout);
        r.transform.parent = transform;
    }

    public void RoomSave()
    {
        PrefabUtility.SaveAsPrefabAsset(GameObject.Find("New_Layout"), "Assets/Prefabs/Room_Layout.prefab");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
