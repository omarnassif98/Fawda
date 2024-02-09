using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    enum AnchorCode {
        BOTTOM_LEFT=-1,
        TOP_RIGHT=1,
        MIDDLE=0
    }
    
public class HauntGameRoomBehaviour : MonoBehaviour
{
    static bool extentReached;
    static float width_used = 0, height_used = 0;
    const float FLOOR_THICKNESS = 0.2f, WALL_THICKNESS = 0.4f, MAX_MAP_HEIGHT = 150, MAX_MAP_WIDTH = 70;
    float targetOpacity = 1;
    Renderer[] bottomWallMats = null; //WHO THE FUCK?????????
    short occupancy = 0;
    float offset;
    // Start is called before the first frame update

    enum AnchorCode{
        TOP=1,
        BOTTOM=2,
        LEFT=3,
        RIGHT=4
    }
    static Dictionary<AnchorCode,float> anchorExtents = new Dictionary<AnchorCode, float>();
    struct WallAnchor{
            public float horizontalAnchor, verticalAnchor;
            public AnchorCode direction;
            public HauntGameRoomBehaviour neighbor;
            public WallAnchor(float _horizontal, float _vertical, AnchorCode _direction){
                horizontalAnchor = _horizontal;
                verticalAnchor = _vertical;
                direction = _direction;
                neighbor = null;
            }
        }
    List<WallAnchor> doors = new List<WallAnchor>();


    void Awake(){
        if(anchorExtents.Count > 0) return;
        anchorExtents[AnchorCode.TOP] = 0.5f;
        anchorExtents[AnchorCode.RIGHT] = 0.5f;
        anchorExtents[AnchorCode.LEFT] = -0.5f;
        anchorExtents[AnchorCode.BOTTOM] = -0.5f;
    }
    void FixedUpdate(){
        if(bottomWallMats == null) return;
        targetOpacity = occupancy == 0? 1 : 0.2f;
        foreach(Renderer mat in bottomWallMats){
            mat.material.color = new Color(mat.material.color.r, mat.material.color.g, mat.material.color.b, Mathf.Lerp(mat.material.color.a, targetOpacity, Time.deltaTime / 2));
        }
    }

    public void Generate(){
        DetermineSize();
        SetupFloorBounds();
        GenerateDoors();
    }

    void DetermineSize(int _forceRand = -1){
        Vector3 roomSize = Vector3.zero;
         switch ((_forceRand == -1)?UnityEngine.Random.Range(0,2):_forceRand){
            case 0:
                roomSize = new Vector3(3,5,25);
                break;
            case 1:
                roomSize = new Vector3(12,5,10);
                break;
        }
        width_used += roomSize.x;
        height_used += roomSize.z;
        transform.localScale = roomSize;
        extentReached = width_used > MAX_MAP_WIDTH || height_used > MAX_MAP_HEIGHT;
        offset = (transform.localScale.y/2 + FLOOR_THICKNESS/2);
    }
    
    void SetupFloorBounds(){
        BoxCollider roomCol = gameObject.AddComponent<BoxCollider>();
        roomCol.center = transform.position;
        roomCol.size = Vector3.one;
        transform.position = transform.position + Vector3.up * offset;
        roomCol.isTrigger = true;
        gameObject.layer = 2;
        gameObject.name = "Room";
        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
        floor.name = "Floor";
        floor.transform.localScale = new Vector3(transform.localScale.x,FLOOR_THICKNESS,transform.localScale.z);
        floor.transform.position = transform.position + Vector3.down * offset;
        floor.transform.parent = transform;
        Material floorMat = (Material)Resources.Load("MinigameAssets/Haunt/FloorMaterial");
        floor.GetComponent<Renderer>().material = floorMat;
    }

    void GenerateDoors(){
        foreach(AnchorCode wallDirection in Enum.GetValues(typeof(AnchorCode))){
            print(string.Format("Now on: {0}",wallDirection.ToString()));
            //if (UnityEngine.Random.Range(0,3) < 3) continue;  
            float xAnchor = wallDirection == AnchorCode.BOTTOM || wallDirection == AnchorCode.TOP ? UnityEngine.Random.Range(-0.25f,0.25f):anchorExtents[wallDirection];
            float zAnchor = wallDirection == AnchorCode.LEFT || wallDirection == AnchorCode.RIGHT ? UnityEngine.Random.Range(-0.25f,0.25f):anchorExtents[wallDirection];
            WallAnchor anch = new WallAnchor(xAnchor,zAnchor,wallDirection);
            VisualizeAnchor(anch);
            doors.Add(anch);
        }
    }

    // Update is called once per frame
    void OnTriggerEnter(Collider _col){
        occupancy += 1;
    }

    void OnTriggerExit(Collider _col){
        occupancy -= 1;
    }

    void VisualizeAnchor(WallAnchor _anchor){
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.transform.position = transform.position + Vector3.down * offset + Vector3.right * _anchor.horizontalAnchor * transform.localScale.x + Vector3.forward * _anchor.verticalAnchor * transform.localScale.z;
        print(string.Format("WallDir: {0} - xAnch: {1} - zAnch = {2}", _anchor.direction.ToString(), _anchor.horizontalAnchor.ToString("F2"), _anchor.verticalAnchor.ToString("F2")));
    }
}
