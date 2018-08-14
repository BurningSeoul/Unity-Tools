using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTSCamera : MonoBehaviour {
    public enum CameraPreset { TOPDOWN, ISOMETRIC, CUSTOM };
    public enum ControlsPreset { WASD, ARROWS, CUSTOM};
    [SerializeField] private CameraPreset currentCamera = CameraPreset.TOPDOWN;
    [SerializeField] private ControlsPreset currentControls = ControlsPreset.WASD;

    [Tooltip("X = Min, Y = Max. Cannot Be negative")]
    [SerializeField] private Vector2 minMaxZoom = Vector2.zero;
    [SerializeField] private bool edgeScroll = false;
    [SerializeField] private float moveSpeed = 5f;
    [Tooltip("The layer that the camera will interact with: e.g hover above, rotate around etc.")]
    [SerializeField] private LayerMask layerMask;

    //Camera Rotation
    [SerializeField] private bool rotateCamera = false;
    [SerializeField] [ConditionalField("rotateCamera")] private float rotSpeed = 50f;
    [SerializeField] [ConditionalField("rotateCamera")] private bool orbitalRotate = false;

    //Internal Input Manager, Replace when adding to project//
    public KeyCode camLeft { get; set; }
    public KeyCode camRight { get; set; }
    public KeyCode camUp { get; set; }
    public KeyCode camDown { get; set; }
    public KeyCode camRotLeft { get; set; }
    public KeyCode camRotRight { get; set; }
    
    private Camera cam;
    private Vector3 forward, right;
    private Transform target;
    
	// Use this for initialization
	void Start () {
        cam = gameObject.GetComponent<Camera>();
        target = transform.GetChild(0);
        
		switch (currentCamera) {
            case (CameraPreset.TOPDOWN):
                transform.rotation = Quaternion.Euler(90, 0, 0);
                transform.position = Vector3.up * 100;
                target.position = Vector3.zero;
                break;
            case (CameraPreset.ISOMETRIC):
                transform.rotation = Quaternion.Euler(30, 45, 0);
                transform.position = Vector3.up * 100;
                target.position = Vector3.zero;
                break;

            case (CameraPreset.CUSTOM):
                break;
        }
        if (transform.forward == Vector3.down) {
            forward = transform.up;
        } else {
            forward = transform.forward;
        }
        forward.y = 0;
        forward = Vector3.Normalize(forward);
        right = Quaternion.Euler(new Vector3(0, 90, 0)) * forward;
        

       

        #region CONTROLS
        switch (currentControls) {
            case (ControlsPreset.WASD):
                camLeft = KeyCode.A;
                camRight = KeyCode.D; 
                camUp = KeyCode.W;
                camDown = KeyCode.S;
                camRotLeft = KeyCode.Q;
                camRotRight = KeyCode.E;
                break;
            case (ControlsPreset.ARROWS):
                camLeft = KeyCode.LeftArrow;
                camRight = KeyCode.RightArrow;
                camUp = KeyCode.UpArrow;
                camDown = KeyCode.DownArrow;
                camRotLeft = KeyCode.Delete;
                camRotRight = KeyCode.PageDown;
                break;

            case (ControlsPreset.CUSTOM):
                //Fill In with custom controls as per project needs
                break;
        }
        #endregion
    }

    // Update is called once per frame
    void Update () {
        RaycastHit hit;
        float castMaxDistance = minMaxZoom.y * 3;
        if (Input.anyKey) {
            Move();
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, castMaxDistance, layerMask)) {
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.red);
                target.position = hit.point;
            } else {
                Debug.DrawRay(transform.position, transform.forward * castMaxDistance, Color.white);
            }
        }
	}

    //Input Handling
    void Move() {
        float horizontalAxis = 0;
        float verticalAxis = 0;

        #region CAMERA_ROTATION
        if (rotateCamera) {
            if (orbitalRotate) {
                //Orbital Rotation

            } else {
                //Normal Rotation
                if (Input.GetKey(camRotRight)) { transform.Rotate(0, Time.deltaTime * rotSpeed, 0, Space.World); }
                if (Input.GetKey(camRotLeft)) { transform.Rotate(0, Time.deltaTime * -rotSpeed, 0, Space.World); }

                //Redefine forward/right vectors after rotation
                if (transform.forward == Vector3.down) {
                    forward = transform.up;
                } else {
                    forward = transform.forward;
                }
                forward.y = 0;
                forward = Vector3.Normalize(forward);
                right = Quaternion.Euler(new Vector3(0, 90, 0)) * forward;
            }

        }
        #endregion
        //Digital Axis'
        if (Input.GetKey(camLeft)) { horizontalAxis = -1;}
        if (Input.GetKey(camRight)) { horizontalAxis = 1;}
        if (Input.GetKey(camUp)) { verticalAxis = 1;}
        if (Input.GetKey(camDown)){ verticalAxis = -1;}

        Vector3 direction = new Vector3(horizontalAxis, 0, verticalAxis);
        Vector3 rightMovement = right * moveSpeed * Time.deltaTime * horizontalAxis;
        Vector3 upMovement = forward * moveSpeed * Time.deltaTime * verticalAxis;

        Vector3 heading = Vector3.Normalize(rightMovement + upMovement);
        transform.position += heading;
    }
}
