using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class WaveCollapseGenerator : MonoBehaviour
{
    const int ROWS = 3, COLS = 3;
    const float FLOOR_THICKNESS = 0.4f, ROOM_SIZE = 17.5f, WALL_THICKNESS = 0.45f;
    float depthRatio = 0;
    [SerializeField] Material wallmat;
    [SerializeField] Material[] floorMats;
    [SerializeField] GameObject playerPrefab;

    void Awake(){
        depthRatio = 1/Mathf.Sin((Camera.main.transform.eulerAngles.x + 25) * Mathf.Deg2Rad);
    }


    void Start(){
        GenerateFloormap();
    }

    public void GenerateFloormap(){
        foreach(Transform t in transform){
            Destroy(t.gameObject);
        }
        int[,] rooms = new int[ROWS,COLS];
        bool[,] explored = new bool[ROWS,COLS];
        int roomNum = 0;
        Stack<Vector2Int> ungenerated = new Stack<Vector2Int>();
        ungenerated.Push(new Vector2Int(ROWS/2,COLS/2));
        while(ungenerated.Count > 0){
            Vector2Int coordToGenerate = ungenerated.Pop();
            print("Popped " + roomNum);
            explored[coordToGenerate.y,coordToGenerate.x] = true;
            int highestNeighbor = GetHighestNeighbor(ref explored, ref rooms, ref ungenerated, coordToGenerate);
            rooms[coordToGenerate.y, coordToGenerate.x] = (UnityEngine.Random.Range(0.0f,1.0f) < 0.45f)? highestNeighbor:roomNum;
            roomNum = (roomNum + 1)%floorMats.Length;
        }

        rooms[UnityEngine.Random.Range(0,ROWS), UnityEngine.Random.Range(0,COLS)] = -1;
        string mat = "";
        for(int i = 0; i < rooms.GetLength(0); i++){
            for(int j = 0; j < rooms.GetLength(1); j++){
                mat += rooms[i,j].ToString() + ' ';
                SetupRoomWalls(rooms, new Vector2Int(j,i));
                if(rooms[i,j] == -1) continue;
                GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                go.transform.parent = transform;
                go.transform.localScale = new Vector3(ROOM_SIZE , FLOOR_THICKNESS, ROOM_SIZE * depthRatio);
                go.transform.position = new Vector3(ROOM_SIZE * (j-1), 0, ROOM_SIZE * depthRatio * (i-1));
                go.GetComponent<Renderer>().material = floorMats[rooms[i,j]];
            }
            mat += '\n';
        }
        print(mat);

        GameObject pl = GameObject.Instantiate(playerPrefab, new Vector3(0, .5f, 0), Quaternion.identity, transform);
        
    }

    void SetupRoomWalls(int [,] _roomMap, Vector2Int _coordinate){
        Vector2Int[] potential = new Vector2Int[]{
            _coordinate + Vector2Int.right,
            _coordinate + Vector2Int.up
        };
        foreach(Vector2Int coord in potential){
            if(coord.x >= COLS || coord.y >= ROWS) continue;
            print(string.Format("{0} | {2} looking at {1} | {3}", _coordinate, coord, _roomMap[_coordinate.y, _coordinate.x], _roomMap[coord.y, coord.x]));
            
            if(_roomMap[_coordinate.y, _coordinate.x] != _roomMap[coord.y, coord.x]){
                GenerateWall(_coordinate, coord - _coordinate);
                
                
            }
        }

        if(_roomMap[_coordinate.y, _coordinate.x] == -1) return;
            switch (_coordinate.x)
            {
                case 0:
                    GenerateWall(_coordinate, Vector2Int.left);
                    break;
                case ROWS - 1:
                    GenerateWall(_coordinate, Vector2Int.right);
                    break;
            }
            
            switch (_coordinate.y)
            {
                case 0:
                    GenerateWall(_coordinate, Vector2Int.down);
                    break;
                case COLS - 1:
                    GenerateWall(_coordinate, Vector2Int.up);
                    break;
            }
    }

    void GenerateWall(Vector2Int _coordinate, Vector2Int _dir){
        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.transform.localScale = new Vector3(_dir.x == 0?ROOM_SIZE:WALL_THICKNESS, 3, _dir.y ==0?ROOM_SIZE*depthRatio:WALL_THICKNESS*depthRatio);
        wall.transform.position = new Vector3(ROOM_SIZE * (_coordinate.x-1) + (_dir.x * ROOM_SIZE/2), 1.5f, ROOM_SIZE * depthRatio * (_coordinate.y-1) + (_dir.y * ROOM_SIZE/2 * depthRatio));
        wall.transform.parent = transform;
        wall.GetComponent<Renderer>().material = wallmat;
    }
    int GetHighestNeighbor(ref bool[,] _explorationMap, ref int[,] roomMap, ref Stack<Vector2Int> _stack, Vector2Int _coordinate){
        Vector2Int[] potential = new Vector2Int[]{
            _coordinate + Vector2Int.left,
            _coordinate + Vector2Int.right,
            _coordinate + Vector2Int.up,
            _coordinate + Vector2Int.down
        };
        List<int> validNeighborStates = new List<int>();
        foreach(Vector2Int vec in potential) {
            if (vec.x <0 || vec.x >= COLS || vec.y < 0 || vec.y >= ROWS || roomMap[vec.y,vec.x] == -1) continue;
            if(_explorationMap[vec.y,vec.x]) validNeighborStates.Add(roomMap[vec.y,vec.x]);
            else{ _stack.Push(vec); roomMap[vec.y,vec.x] = -1;}
        }
        return (validNeighborStates.Count > 0)? validNeighborStates.Max():0;
    }
    



}
