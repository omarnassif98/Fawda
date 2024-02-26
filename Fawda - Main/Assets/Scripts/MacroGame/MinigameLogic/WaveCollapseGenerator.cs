using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class WaveCollapseGenerator : MonoBehaviour
{
    const int SIZE = 3;
    public struct RoomNode{

    }


    void Awake(){
        
    }


    void Start(){
        GenerateFloormap();
    }

    public void GenerateFloormap(){
        int[,] rooms = new int[SIZE,SIZE];
        bool[,] explored = new bool[SIZE,SIZE];
        int roomNum = 0;
        Stack<Vector2Int> ungenerated = new Stack<Vector2Int>();
        ungenerated.Push(new Vector2Int(SIZE/2,SIZE/2));
        while(ungenerated.Count > 0){
            Vector2Int coordToGenerate = ungenerated.Pop();
            print("Popped " + roomNum);
            explored[coordToGenerate.y,coordToGenerate.x] = true;
            int highestNeighbor = GetHighestNeighbor(ref explored, ref rooms, ref ungenerated, coordToGenerate);
            rooms[coordToGenerate.y, coordToGenerate.x] = (UnityEngine.Random.Range(0.0f,1.0f) < 0.45f)? highestNeighbor:roomNum;
            roomNum += 1;
        }
        string mat = "";
        for(int i = 0; i < rooms.GetLength(0); i++){
            for(int j = 0; j < rooms.GetLength(1); j++){
                mat += rooms[i,j].ToString() + ' ';
            }
            mat += '\n';
        }
        print(mat);
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
            if (vec.x <0 || vec.x >= SIZE || vec.y < 0 || vec.y >= SIZE || roomMap[vec.y,vec.x] == -1) continue;
            if(_explorationMap[vec.y,vec.x]) validNeighborStates.Add(roomMap[vec.y,vec.x]);
            else{ _stack.Push(vec); roomMap[vec.y,vec.x] = -1;}
        }
        return (validNeighborStates.Count > 0)? validNeighborStates.Max():0;
    }
    



}
