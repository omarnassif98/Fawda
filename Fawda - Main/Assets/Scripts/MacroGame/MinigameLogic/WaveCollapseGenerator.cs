using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveCollapseGenerator : MonoBehaviour
{
    Dictionary<string, RoomNode> graph = new Dictionary<string, RoomNode>();
    struct RoomNodeEdge{
        public RoomNode neighbor;
        public int weight;
        public RoomNodeEdge(RoomNode _neighbor, int _weight){
            neighbor = _neighbor;
            weight = _weight;
        }
    }

    struct RoomNode{
        float xWidth;
        float zHeigth;
        List<RoomNodeEdge> connections;
        public RoomNode(float _xWidth, float _zHeight){
            xWidth = _xWidth;
            zHeigth = _zHeight;
            connections = new List<RoomNodeEdge>();
        }
    }


    void Awake(){
        LoadConfig();
    }

    void LoadConfig(){
        TextAsset rawNodeData = Resources.Load<TextAsset>("MinigameAssets/Haunt/WaveFunctionRoomNodes");
        string[] rows = rawNodeData.text.Split('\n');
        for(int i = 1; i < rows.Length; i++){
            string row = rows[i];
            try
            {
                string[] vals = row.Split(',');
                graph[vals[0]] = new RoomNode(float.Parse(vals[1].Trim()), float.Parse(vals[2].Trim()));
                print("Node Created: " + vals[0]);
            }
            catch (System.Exception)
            {
                Debug.LogError("Error Loading Node: " + row);
            }

        }
    }

    void Start(){
        //GenerateRoom();
    }
    void GenerateRoom(){
        HauntGameRoomBehaviour room = new GameObject().AddComponent<HauntGameRoomBehaviour>();
        room.transform.parent = transform;
        room.transform.position = Vector3.zero;
        room.Generate();
    }



}
