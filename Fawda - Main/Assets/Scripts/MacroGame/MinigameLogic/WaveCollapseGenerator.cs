using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveCollapseGenerator : MonoBehaviour
{
    Dictionary<RoomCode, RoomNode> proceduralTemplates = new Dictionary<RoomCode, RoomNode>();

    public enum DirCode{
        UP=1,
        DOWN=-1,
        LEFT=-2,
        RIGHT=2
    }

    public enum RoomCode{
        UNSET=0,
        NORTHWEST=1,
        NORTH=2,
        NORTHEAST=3,
        WEST=4,
        MID=5,
        EAST=6,
        SOUTHWEST=7,
        SOUTH=8,
        SOUTHEAST=9
    }

    public struct RoomNode{
        public static readonly RoomNode Empty;
        public string name;
        public bool topWall, southWall, leftWall, rightWall;
        public Dictionary<DirCode,List<RoomCode>> connections;
        public RoomNode(string _name, bool _topWall, bool _southWall, bool _leftWall, bool _rightWall){
            name = _name;
            topWall = _topWall;
            southWall = _southWall;
            leftWall = _leftWall;
            rightWall = _rightWall;
            connections = new Dictionary<DirCode, List<RoomCode>>();
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
                proceduralTemplates[(RoomCode)Enum.Parse(typeof(RoomCode),vals[0].Trim())] = new RoomNode(vals[0].Trim(), bool.Parse(vals[1].Trim()), bool.Parse(vals[2].Trim()), bool.Parse(vals[3].Trim()), bool.Parse(vals[4].Trim()));
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
                RoomCode from = (RoomCode)Enum.Parse(typeof(RoomCode),vals[0].Trim());
                RoomCode to = (RoomCode)Enum.Parse(typeof(RoomCode),vals[1].Trim());
                DirCode dir = (DirCode)Enum.Parse(typeof(DirCode),vals[2].Trim());
                if (!proceduralTemplates[from].connections.ContainsKey(dir)) proceduralTemplates[from].connections[dir] = new List<RoomCode>();
                proceduralTemplates[from].connections[dir].Add(to);
            }
            catch (System.Exception)
            {
                Debug.LogError("Error Loading Edge: " + row);
            }
        }
    }

    void Start(){
        GenerateFloormap();
    }

    void GenerateFloormap(){
        RoomCode[,] map = new RoomCode[30,30];
        Stack<Vector2Int> coordsToCollapse = new Stack<Vector2Int>();
        map[15,15] = RoomCode.MID;
        GetEmptyCoords(ref map,new Vector2Int(15,15));
    
    }

    List<Vector2> GetEmptyCoords(ref RoomCode[,] map, Vector2Int _coord){
        List<Vector2> l = new List<Vector2>();
        print(Enum.GetName(typeof(RoomCode), map[_coord.y, _coord.x]));
        if(_coord.y-1 >= 0) print(string.Format("LEFT IS NULL {0}", map[0,0] == RoomCode.UNSET));
        return l;
    }
    RoomNode PickRandomNeighborTemplate(){
        return RoomNode.Empty;        
    }



}
