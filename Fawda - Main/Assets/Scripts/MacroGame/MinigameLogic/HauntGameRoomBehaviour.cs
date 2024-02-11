using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public enum AnchorCode{
        TOP=1,
        BOTTOM=-1,
        LEFT=-2,
        RIGHT=2
    }

        public struct WallAnchor{
            public static readonly WallAnchor Empty;
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
    
public class HauntGameRoomBehaviour : MonoBehaviour
{   
    static int num = 0;
    static bool extentReached;
    static float width_used = 0, height_used = 0, view_angle_factor = 1;
    const float FLOOR_THICKNESS = 0.2f, MAX_MAP_HEIGHT = 150, MAX_MAP_WIDTH = 70, OFFSET = FLOOR_THICKNESS + 5.0f/2;
    float targetOpacity = 1;
    readonly Renderer[] bottomWallMats = null; //WHO THE FUCK?????????
    short occupancy = 0;
    // Start is called before the first frame update
    static readonly Dictionary<AnchorCode,float> anchorExtents = new Dictionary<AnchorCode, float>();

    List<WallAnchor> doors = new List<WallAnchor>();


    void Awake(){
        if(anchorExtents.Count > 0) return;
        anchorExtents[AnchorCode.TOP] = 0.5f;
        anchorExtents[AnchorCode.RIGHT] = 0.5f;
        anchorExtents[AnchorCode.LEFT] = -0.5f;
        anchorExtents[AnchorCode.BOTTOM] = -0.5f;
        float viewAngle = Camera.main.transform.eulerAngles.x;
        view_angle_factor = 1/Mathf.Sin(Mathf.Deg2Rad * viewAngle);
        print("VIEW ANGLE FACTOR = " + view_angle_factor + " - from: " + viewAngle);
    }
    void FixedUpdate(){
        if(bottomWallMats == null) return;
        targetOpacity = occupancy == 0? 1 : 0.2f;
        foreach(Renderer mat in bottomWallMats){
            mat.material.color = new Color(mat.material.color.r, mat.material.color.g, mat.material.color.b, Mathf.Lerp(mat.material.color.a, targetOpacity, Time.deltaTime / 2));
        }
    }

    public void Generate(){
        DetermineLocation();
        SetupFloorBounds();
        GenerateDoors();
    }

    private AnchorCode FlipAnchorCode(AnchorCode _anch){
        return (AnchorCode)(-1 * (int)_anch);
    }

    public void DetermineSize(float _xWidth, float _zHeight){
        Vector3 roomSize = new Vector3(_xWidth, 5, _zHeight);
        if(doors.Count > 0 && (doors[0].direction == AnchorCode.LEFT || doors[0].direction == AnchorCode.RIGHT)) roomSize = new Vector3(_zHeight,5,_xWidth);
        
        roomSize.z *= view_angle_factor;
        width_used += roomSize.x;
        height_used += roomSize.z;
        transform.localScale = roomSize;
        extentReached = width_used > MAX_MAP_WIDTH || height_used > MAX_MAP_HEIGHT;
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
        foreach(Collider col in Physics.OverlapBox(prospect,transform.localScale,transform.rotation)){
            if(col.GetComponent<HauntGameRoomBehaviour>() && col.gameObject != doors[0].neighbor.gameObject && col.gameObject != gameObject){ // UNRELIABLE
                print(string.Format("Graceful Deletion - {0}", col.gameObject.name));
                extentReached = true;
                Destroy(gameObject);
                }
        }
        transform.position = prospect;
    }
    
    void SetupFloorBounds(){
        BoxCollider roomCol = gameObject.AddComponent<BoxCollider>();
        roomCol.center = transform.position;
        roomCol.size = Vector3.one;
        transform.position = transform.position + Vector3.up * OFFSET;
        roomCol.isTrigger = true;
        gameObject.layer = 2;
        gameObject.name = "Room " + num;
        num += 1;
        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
        floor.name = "Floor";
        floor.transform.localScale = new Vector3(transform.localScale.x,FLOOR_THICKNESS,transform.localScale.z);
        floor.transform.position = transform.position + Vector3.down * OFFSET;
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
            Vector3 doorPos = transform.position + Vector3.down * OFFSET + Vector3.right * xAnchor * transform.localScale.x + Vector3.forward * zAnchor * transform.localScale.z;
            WallAnchor anch = new WallAnchor(doorPos,wallDirection);
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

    public WallAnchor[] GetAnchors(){
        return doors.ToArray();
    }

    public void FeedWallAnchor(WallAnchor _anchorRelativeToNeighbor, HauntGameRoomBehaviour _neighbor){
        AnchorCode codeRelativeToSelf = FlipAnchorCode(_anchorRelativeToNeighbor.direction);
        WallAnchor fedWallAnchor = new WallAnchor(_anchorRelativeToNeighbor.anchorPoint,codeRelativeToSelf,_neighbor);
        doors.Add(fedWallAnchor);
    }

    
}
