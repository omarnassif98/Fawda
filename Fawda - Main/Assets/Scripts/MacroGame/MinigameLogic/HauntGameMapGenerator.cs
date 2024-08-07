using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class HauntGameMapGenerator
{
    const int ROWS = 3, COLS = 3;
    public const float FLOOR_THICKNESS = 0.4f, ROOM_SIZE = 12f, WALL_THICKNESS = 0.45f, DOOR_WIDTH = 3.0f;
    private Material wallmat;
    private UnityEngine.Object[] floorMats;
    private Transform mapTransform;
    public Transform[] hunterSpawnPoints = new Transform[4];
    public Transform ghostSpawnPoint;

    public HauntGameMapGenerator(Transform _mapTransform){
        DebugLogger.SourcedPrint("MapGenerator", "Awake");
        mapTransform = _mapTransform;
        floorMats = Resources.LoadAll("MinigameAssets/Haunt/FloorMaterials", typeof(Material));
        wallmat = Resources.Load("MinigameAssets/Haunt/Materials/WallMat") as Material;
    }

    public void GenerateFloormap(){
        //Wave-like collapse for generation
        DebugLogger.SourcedPrint("MapGenerator", "Generating");
        mapTransform.eulerAngles = Vector3.zero;
        DebugLogger.SourcedPrint("MapGenerator", "Go for it");
        int[,] rooms = new int[ROWS,COLS];
        bool[,] explored = new bool[ROWS,COLS];
        int roomNum = 0;
        Stack<Vector2Int> ungenerated = new Stack<Vector2Int>();
        ungenerated.Push(new Vector2Int(ROWS/2,COLS/2));
        while(ungenerated.Count > 0){
            Vector2Int coordToGenerate = ungenerated.Pop();
            explored[coordToGenerate.y,coordToGenerate.x] = true;
            int highestNeighbor = GetHighestNeighbor(ref explored, ref rooms, ref ungenerated, coordToGenerate);
            rooms[coordToGenerate.y, coordToGenerate.x] = (UnityEngine.Random.Range(0.0f,1.0f) < 0.45f)? highestNeighbor:roomNum;
            roomNum = (roomNum + 1)%floorMats.Length;
        }


        //Delete 1 room ON THE PERIFERY
        if (UnityEngine.Random.Range(0.0f, 1.0f) < 0.5f) rooms[UnityEngine.Random.Range(0,ROWS), new int[2]{0,COLS-1}[UnityEngine.Random.Range(0,2)]] = -1; //Either on the horizontals
        else rooms[new int[2]{0,ROWS-1}[UnityEngine.Random.Range(0,2)],UnityEngine.Random.Range(0,COLS)] = -1; //Or verticals

        //Setup of the floorplan and walls according to the rng
        //Also sets up the spawn points
        for(int i = 0; i < rooms.GetLength(0); i++){ //Down to Up
            for(int j = 0; j < rooms.GetLength(1); j++){//Left to Right

                List<GameObject> walls = SetupRoomWalls(rooms, new Vector2Int(j,i)); //Set up walls, each room handles their lower and right walls
                if(rooms[i,j] == -1) continue; //Empty rooms technically have walls, but no floor, so skip
                GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                go.transform.parent = mapTransform;
                go.transform.localScale = new Vector3(ROOM_SIZE , FLOOR_THICKNESS, ROOM_SIZE);
                go.transform.position = new Vector3(ROOM_SIZE * (j-1), 0, ROOM_SIZE * (i-1));
                go.GetComponent<Renderer>().material = (Material)floorMats[rooms[i,j]];
                go.name = "Room " + (i*3 + j).ToString();
                go.layer = LayerMask.NameToLayer("Ignore Raycast");
                BoxCollider roomCollider = go.GetComponent<BoxCollider>();
                roomCollider.isTrigger = true;
                roomCollider.center = Vector3.up * FLOOR_THICKNESS * 1.5f;
                HauntGameRoomBehaviour roomLogic = go.AddComponent<HauntGameRoomBehaviour>();
                roomLogic.FeedWalls(walls);
                if(i != 1 || j != 1) continue;
                SetupSpawnPoints(go.transform);
            }
        }
        mapTransform.eulerAngles = new Vector3(0,45,0);
    }

    void SetupSpawnPoints(Transform _centerRoom){
        ghostSpawnPoint = new GameObject().transform;
        ghostSpawnPoint.parent = _centerRoom;
        ghostSpawnPoint.localPosition = new Vector3(0,FLOOR_THICKNESS/2, 0);
        Vector3[] cardinalDirs = new Vector3[4]{Vector3.forward, Vector3.back, Vector3.left, Vector3.right};
        for(int i = 0; i < 4; i++){
            hunterSpawnPoints[i] = new GameObject().transform;
            hunterSpawnPoints[i].position = ghostSpawnPoint.position + cardinalDirs[i] * ROOM_SIZE/3;
            hunterSpawnPoints[i].parent = _centerRoom;
            hunterSpawnPoints[i].forward = -cardinalDirs[i];
        }
    }

    List<GameObject> SetupRoomWalls(int [,] _roomMap, Vector2Int _coordinate){
        List<GameObject> roomWalls = new List<GameObject>(); // We need to return the walls to the RIGHT, and DOWN

        Vector2Int[] potential = new Vector2Int[]{
            _coordinate + Vector2Int.right,
            _coordinate + Vector2Int.down
        }; // Potential inter-room walls, we need their states

        foreach(Vector2Int coord in potential){
            if(coord.x >= COLS || coord.y < 0) continue; //Ignore if it points out of the array
            if(_roomMap[_coordinate.y, _coordinate.x] != _roomMap[coord.y, coord.x]){
                foreach(GameObject wall in GenerateWall(_coordinate, coord - _coordinate, _roomMap[coord.y,coord.x] != -1 && _roomMap[_coordinate.y,_coordinate.x] != -1)){
                    roomWalls.Add(wall); //These ones are guaranteed to be relevant
                }
            }
        }

        if(_roomMap[_coordinate.y, _coordinate.x] == -1) return roomWalls; //Empty rooms take up no space, they have no periphery

        switch (_coordinate.x)
        {
            case 0:
                GenerateWall(_coordinate, Vector2Int.left, false);
                break;
            case ROWS - 1: //Add right wall if it's periphery
                foreach (GameObject wall in GenerateWall(_coordinate, Vector2Int.right, false)) roomWalls.Add(wall);
                break;
        }

        switch (_coordinate.y)
        {
            case 0: //Add bottom wall if it's a periphery
                foreach (GameObject wall in GenerateWall(_coordinate, Vector2Int.down, false)) roomWalls.Add(wall);
                break;
            case COLS - 1:
                GenerateWall(_coordinate, Vector2Int.up, false);
                break;
        }
        return roomWalls;
    }




    List<GameObject> GenerateWall(Vector2Int _coordinate, Vector2Int _dir, bool _doored = true){
        List<GameObject> wallParts = new List<GameObject>();//if doors have walls, it's 2 game objects

        switch (_doored){
            case true: //door exists, the rooms have different states
                float doorLoc = UnityEngine.Random.Range(0.25f,0.75f) * ROOM_SIZE;
                GameObject wallPartL = GameObject.CreatePrimitive(PrimitiveType.Cube);
                wallPartL.transform.localScale = new Vector3(
                    _dir.x == 0?(doorLoc - DOOR_WIDTH/2) + WALL_THICKNESS:WALL_THICKNESS,
                    3,
                    _dir.y ==0?(doorLoc - DOOR_WIDTH/2 + WALL_THICKNESS):WALL_THICKNESS);


                wallPartL.transform.position = new Vector3(
                    ROOM_SIZE * (_coordinate.x-1) + (_dir.x * ROOM_SIZE/2) - (_dir.y * ROOM_SIZE/2) + (_dir.y * wallPartL.transform.localScale.x/2) - (_dir.y * WALL_THICKNESS/2),
                    1.5f,
                    ROOM_SIZE * (_coordinate.y-1) + (_dir.y * ROOM_SIZE/2) - (_dir.x * ROOM_SIZE/2) + (_dir.x * wallPartL.transform.localScale.z/2) - (_dir.x * WALL_THICKNESS/2));
                wallParts.Add(wallPartL);
                GameObject wallPartR = GameObject.CreatePrimitive(PrimitiveType.Cube);
                wallPartR.transform.localScale = new Vector3(
                _dir.x == 0?ROOM_SIZE-doorLoc-DOOR_WIDTH/2:WALL_THICKNESS,
                3,
                _dir.y ==0?(ROOM_SIZE-doorLoc-DOOR_WIDTH/2):WALL_THICKNESS);

                wallPartR.transform.position = new Vector3(
                ROOM_SIZE * (_coordinate.x-1) + (_dir.x * ROOM_SIZE/2) - (_dir.y * ROOM_SIZE/2) + (_dir.y * doorLoc) + (_dir.y * DOOR_WIDTH/2) + (_dir.y * wallPartR.transform.localScale.x/2),
                1.5f,
                ROOM_SIZE * (_coordinate.y-1)  + (_dir.y * ROOM_SIZE/2)  - (_dir.x * ROOM_SIZE/2) + (_dir.x * doorLoc) + (_dir.x * DOOR_WIDTH/2) + (_dir.x * wallPartR.transform.localScale.z/2) );
                wallParts.Add(wallPartR);

                break;
            case false: //door does not exist
                GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
                wall.transform.localScale = new Vector3(_dir.x == 0?ROOM_SIZE:WALL_THICKNESS, 3, _dir.y ==0?ROOM_SIZE:WALL_THICKNESS);
                wall.transform.position = new Vector3(ROOM_SIZE * (_coordinate.x-1) + (_dir.x * ROOM_SIZE/2), 1.5f, ROOM_SIZE * (_coordinate.y-1) + (_dir.y * ROOM_SIZE/2));
                wallParts.Add(wall);
                break;
        }
        foreach (GameObject wall in wallParts)
        {
            wall.name = "Wall";
            wall.transform.parent = mapTransform;
            wall.GetComponent<Renderer>().material = wallmat;
        }
        return wallParts;
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
