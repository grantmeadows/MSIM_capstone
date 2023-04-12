using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class Room_Generation : MonoBehaviour
{
    public GameObject layout;

    public void RoomStart()
    {
        GameObject r = Instantiate(layout);
        r.name = layout.name;
    }

    public void RoomSave()
    {
        PrefabUtility.SaveAsPrefabAsset(GameObject.Find("Room_Layout"), "Assets/Prefabs/Room_Layout.prefab");
    }

    public void ResetRoom()
    {
        foreach (Transform child in transform.GetChild(0).transform)
        {
            child.GetComponent<Renderer>().material.color = Color.white;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
