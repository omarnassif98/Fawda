using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveCollapseGenerator : MonoBehaviour
{
    Dictionary<string, RoomNode> graph = new Dictionary<string, RoomNode>();
    public struct RoomNodeEdge{
        public RoomNode neighbor;
        public int weight;
        public bool walled;
        public RoomNodeEdge(RoomNode _neighbor, int _weight, bool _walled){
            neighbor = _neighbor;
            weight = _weight;
            walled = _walled;
        }
    }

    public struct RoomNode{
        public static readonly RoomNode Empty;
        public float xWidth;
        public float zHeigth;
        public string name;
       public List<RoomNodeEdge> connections;
        public RoomNode(string _name, float _xWidth, float _zHeight){
            name = _name;
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
        string[] nodeRows = rawNodeData.text.Split('\n');
        for(int i = 1; i < nodeRows.Length; i++){
            string row = nodeRows[i];
            try
            {
                string[] vals = row.Split(',');
                graph[vals[0].Trim()] = new RoomNode(vals[0].Trim(), float.Parse(vals[1].Trim()), float.Parse(vals[2].Trim()));
                print("Node Created: " + vals[0]);
            }
            catch (System.Exception)
            {
                Debug.LogError("Error Loading Node: " + row);
            }
        }

        TextAsset rawEdgeData = Resources.Load<TextAsset>("MinigameAssets/Haunt/WaveFunctionNodeEdges");
        string[] edgeRows = rawEdgeData.text.Split('\n');
        for(int i = 1; i<edgeRows.Length; i++){
            string row = edgeRows[i];
            try
            {
                string[] vals = row.Split(',');
                graph[vals[0].Trim()].connections.Add(new RoomNodeEdge(graph[vals[1].Trim()], int.Parse(vals[2].Trim()), bool.Parse(vals[3].Trim())));
                print("Edge Created: " + vals[0] + " ->> " + vals[1]);
            }
            catch (System.Exception)
            {
                Debug.LogError("Error Loading Edge: " + row);
            }
        }
    }

    void Start(){
        GenerateRoom(graph["square"],WallAnchor.Empty);
    }
    void GenerateRoom(RoomNode _preset, WallAnchor _wallAnchor, HauntGameRoomBehaviour _prevRoom = null){
        HauntGameRoomBehaviour room = new GameObject().AddComponent<HauntGameRoomBehaviour>();
        room.transform.parent = transform;
        if (_prevRoom) room.FeedWallAnchor(_wallAnchor,_prevRoom);
        room.DetermineSize(_preset.xWidth, _preset.zHeigth);
        room.Generate();
        foreach(WallAnchor wa in room.GetAnchors()){
            if(wa.neighbor == null) GenerateRoom(PickRandomNeighborTemplate(_preset),wa,room);
        }
    }

    RoomNode PickRandomNeighborTemplate(RoomNode _previousNode){
        int[] weightPool = new int[_previousNode.connections.Count];
        for(int i = 0; i < weightPool.Length; i++){
            weightPool[i] = (i==0?0:weightPool[i-1]) + _previousNode.connections[i].weight;
        }

        int randChoice = UnityEngine.Random.Range(0,weightPool[weightPool.Length-1]);

        for(int i = 0; i < weightPool.Length; i++){
            if(randChoice < weightPool[i]) return _previousNode.connections[i].neighbor;
        }

        return RoomNode.Empty;        
    }



}
