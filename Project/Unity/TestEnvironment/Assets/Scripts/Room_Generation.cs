using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

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

    public GameObject RoomPrefab;

    void Start()
    {
        if (RoomPrefab != null) 
        { 
            Instantiate(RoomPrefab);
        }
    }

    // Start is called before the first frame update
    public void RoomStart()
    {
        PrefabUtility.SaveAsPrefabAsset(gameObject, "Assets/Prefabs/"+ gameObject.name + ".prefab");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
