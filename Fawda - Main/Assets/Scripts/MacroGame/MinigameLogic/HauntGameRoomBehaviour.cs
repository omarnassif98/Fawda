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
    static int num = 0;
    static bool extentReached;
    static float width_used = 0, height_used = 0;
    const float FLOOR_THICKNESS = 0.2f, WALL_THICKNESS = 0.4f, MAX_MAP_HEIGHT = 150, MAX_MAP_WIDTH = 70;
    float targetOpacity = 1;
    Renderer[] bottomWallMats = null; //WHO THE FUCK?????????
    short occupancy = 0;
    float offset;
    // Start is called before the first frame update

    public enum AnchorCode{
        TOP=1,
        BOTTOM=-1,
        LEFT=-2,
        RIGHT=2
    }
    static Dictionary<AnchorCode,float> anchorExtents = new Dictionary<AnchorCode, float>();
    public struct WallAnchor{
            public Vector3 anchorPoint;
            public AnchorCode direction;
            public HauntGameRoomBehaviour neighbor;
            public WallAnchor(Vector3 _anchorPoint, AnchorCode _direction){
                anchorPoint = _anchorPoint;
                direction = _direction;
                neighbor = null;
            }

            public WallAnchor(Vector3 _anchorPoint, AnchorCode _direction, HauntGameRoomBehaviour _neighbor){
                anchorPoint = _anchorPoint;
                direction = _direction;
                neighbor = _neighbor;
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
        DetermineLocation();
        SetupFloorBounds();
        GenerateDoors();
    }

    public void FeedWallAnchor(WallAnchor _anchorRelativeToNeighbor, HauntGameRoomBehaviour _neighbor){
        AnchorCode codeRelativeToSelf = FlipAnchorCode(_anchorRelativeToNeighbor.direction);
        WallAnchor fedWallAnchor = new WallAnchor(_anchorRelativeToNeighbor.anchorPoint,codeRelativeToSelf,_neighbor);
        doors.Add(fedWallAnchor);
    }

    private AnchorCode FlipAnchorCode(AnchorCode _anch){
        return (AnchorCode)(-1 * (int)_anch);
    }

    void DetermineSize(int _forceRand = -1){
        Vector3 roomSize = Vector3.zero;
         switch ((_forceRand == -1)?UnityEngine.Random.Range(0,4):_forceRand){
            case 0:
                roomSize = new Vector3(3,5,25);
                break;
            case 1:
                roomSize = new Vector3(12,5,10);
                break;
            case 2:
                roomSize = new Vector3(10,5,6);
                break;
            case 3:
                roomSize = new Vector3(4,5,12);
                break;
        }
        width_used += roomSize.x;
        height_used += roomSize.z;
        transform.localScale = roomSize;
        extentReached = width_used > MAX_MAP_WIDTH || height_used > MAX_MAP_HEIGHT;
        offset = (transform.localScale.y/2 + FLOOR_THICKNESS/2);
    }

    void DetermineLocation(){
        if(doors.Count == 0) return;
        Vector3 dir = Vector3.zero;
        float factor = 0;
        switch (doors[0].direction)
        {
            case AnchorCode.TOP:
            case AnchorCode.BOTTOM:
                dir = Vector3.forward;
                factor = transform.localScale.z;
                break;
            case AnchorCode.LEFT:
            case AnchorCode.RIGHT:
                dir = Vector3.right;
                factor = transform.localScale.x;
                break;
        }
        Vector3 prospect = new Vector3(doors[0].anchorPoint.x, transform.position.y, doors[0].anchorPoint.z) + dir * factor/2 * Mathf.Sign((int)FlipAnchorCode(doors[0].direction));
        foreach(Collider col in Physics.OverlapBox(prospect,transform.localScale/2,transform.rotation)){
            if(col.GetComponent<HauntGameRoomBehaviour>() && col.gameObject != doors[0].neighbor.gameObject && col.gameObject != gameObject){ // UNRELIABLE
                print(string.Format("Graceful Deletion - {0}", col.gameObject.name));
                Destroy(gameObject);}
        }
        transform.position = prospect;
    }
    
    void SetupFloorBounds(){
        BoxCollider roomCol = gameObject.AddComponent<BoxCollider>();
        roomCol.center = transform.position;
        roomCol.size = Vector3.one;
        transform.position = transform.position + Vector3.up * offset;
        roomCol.isTrigger = true;
        gameObject.layer = 2;
        gameObject.name = "Room " + num;
        num += 1;
        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
        floor.name = "Floor";
        floor.transform.localScale = new Vector3(transform.localScale.x,FLOOR_THICKNESS,transform.localScale.z);
        floor.transform.position = transform.position + Vector3.down * offset;
        floor.transform.parent = transform;
        Material floorMat = (Material)Resources.Load("MinigameAssets/Haunt/FloorMaterial");
        floor.GetComponent<Renderer>().material = floorMat;
    }

    void GenerateDoors(){
        if(extentReached) return;
        foreach(AnchorCode wallDirection in Enum.GetValues(typeof(AnchorCode))){
            if (UnityEngine.Random.Range(0,5) < 3) continue;  
            float xAnchor = wallDirection == AnchorCode.BOTTOM || wallDirection == AnchorCode.TOP ? UnityEngine.Random.Range(-0.25f,0.25f):anchorExtents[wallDirection];
            float zAnchor = wallDirection == AnchorCode.LEFT || wallDirection == AnchorCode.RIGHT ? UnityEngine.Random.Range(-0.25f,0.25f):anchorExtents[wallDirection];
            Vector3 doorPos = transform.position + Vector3.down * offset + Vector3.right * xAnchor * transform.localScale.x + Vector3.forward * zAnchor * transform.localScale.z;
            WallAnchor anch = new WallAnchor(doorPos,wallDirection);
            anch.neighbor = new GameObject().AddComponent<HauntGameRoomBehaviour>();
            anch.neighbor.transform.parent = transform.parent;
            anch.neighbor.FeedWallAnchor(anch,this);
            anch.neighbor.Generate();
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
        go.transform.position = _anchor.anchorPoint;
        go.transform.parent = transform;
    }
}
