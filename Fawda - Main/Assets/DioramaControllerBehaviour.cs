using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class DioramaControllerBehaviour : MonoBehaviour
{
    // Start is called before the first frame update

    [Serializable]
    struct RigidCamPos{
        public Vector3 pos;
        public Vector3 eulerAngles;
        public float zoom;
        public RigidCamPos(Vector3 _pos, Vector3 _eulerAngles, float _zoom){
            this.pos = _pos;
            this.eulerAngles = _eulerAngles;
            this.zoom = _zoom;
        }
    }
    private bool dioramaMode = true;
    
    RigidCamPos restingCamPos = new RigidCamPos(new Vector3(0, 20, -40), new Vector3(25, 0, 0), 34),
        dioramaCamPos = new RigidCamPos(new Vector3(0, 15, -15), new Vector3(35, 0, 0), 25),
        idealCamPos;

    private List<Transform> trackedTransforms = new List<Transform>();
    Transform checkerboardCanvas;
    public static DioramaControllerBehaviour singleton;
    private Camera cam;
    const float MIN_ZOOM = 12.5f, ZOOM_BUFFER = 2.5f;

    public void Awake()
    {
        if (singleton != null) Destroy(gameObject);
        singleton = this;
        cam = GetComponent<Camera>();
        checkerboardCanvas = GameObject.Find("Checkerboard").transform;
    }

    public void Start()
    {
        idealCamPos = restingCamPos;
        ConnectionManager.singleton.RegisterServerEventListener("wakeup",() =>SetCameraMode(false));
    }

    public void TrackTransform(Transform _toTrack) => trackedTransforms.Add(_toTrack);

    public void StopTrackTransform(Transform _untrack) => trackedTransforms.Remove(_untrack);

    public void ClearTrackTransform() => trackedTransforms.Clear();



    public void SetCameraMode(bool _diorama)
    {
        DebugLogger.SourcedPrint("Camera", "Changing camera mode: " + _diorama.ToString(), "007700");
        dioramaMode = _diorama;
        DebugLogger.SourcedPrint("Camera", "Diorama: " + dioramaMode.ToString(), "007700");

    }
    void UpdateIdealPos(RigidCamPos _pos) => idealCamPos = _pos;

    void UpdateCameraPosition(){
        transform.position = Vector3.Lerp(transform.position, idealCamPos.pos, 0.1f);
        transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, idealCamPos.eulerAngles, 0.1f);
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, idealCamPos.zoom, 0.1f);

    }

    void CalculateTrackPos()
    {
        Bounds bounds = new Bounds(trackedTransforms[0].position, Vector3.zero);
        foreach (Transform t in trackedTransforms) bounds.Encapsulate(t.position);
        float bufferedZoom = MathF.Max(bounds.size.x, bounds.size.z) + ZOOM_BUFFER;
        UpdateIdealPos(new RigidCamPos(bounds.center + new Vector3(0,5,-5), new Vector3(40, 0, 0), Mathf.Max(bufferedZoom, MIN_ZOOM)));
    }

    void LateUpdate(){
        UpdateCameraPosition();
        if (dioramaMode || trackedTransforms.Count == 0) idealCamPos = dioramaCamPos;
        else CalculateTrackPos();
    }

   
}
