
using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

[AddComponentMenu("Camera-Control/3dsMax Camera Style")]
public class CameraScript : NetworkBehaviour
{
    public Transform target;
    public Vector3 targetOffset;
    public float distance = 5.0f;
    public float maxDistance = 20;
    public float minDistance = .6f;
    public float xSpeed = 200.0f;
    public float ySpeed = 200.0f;
    public int yMinLimit = -80;
    public int yMaxLimit = 80;
    public int zoomRate = 70;
    public float panSpeed = 0.3f;
    public float zoomDampening = 5.0f;
    public Player player;

    private float xDeg = 0.0f;
    private float yDeg = 0.0f;
    private float currentDistance;
    private float desiredDistance;
    private Quaternion currentRotation;
    private Quaternion desiredRotation;
    private Quaternion rotation;
    private Vector3 position;
    private bool freecam = true;  //Is the camera free to move or not
    private bool skyview = false;   //If watching the field from above
    private Grid grid;


    public float XDeg { get { return xDeg; } set { xDeg = value; } }

    public float YDeg { get { return yDeg; } set { yDeg = value; } }

    public float DesiredDistance { get { return desiredDistance; } set { desiredDistance = value; } }

    void Start()
    {

        Init();

    }

    void OnEnable() { Init(); }

    public void Init()
    {
        //If there is no target, create a temporary target at 'distance' from the cameras current viewpoint
        if (!target)
        {
            GameObject go = new GameObject("Cam Target");
            go.transform.position = transform.position + (transform.forward * distance);
            target = go.transform;
        }

        distance = Vector3.Distance(transform.position, target.position);
        currentDistance = distance;
        DesiredDistance = distance;

        //be sure to grab the current rotations as starting points.
        position = transform.position;
        rotation = transform.rotation;
        currentRotation = transform.rotation;
        desiredRotation = transform.rotation;

        XDeg = Vector3.Angle(Vector3.right, transform.right);
        YDeg = Vector3.Angle(Vector3.up, transform.up);

        grid = Grid.instance;

    }

    /*
     * Camera logic on LateUpdate to only update after all character movement logic has been handled. 
     */
    void LateUpdate()
    {
        float y = position.y;

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            freecam = !freecam;
            skyview = false;
            Debug.Log("Camera mode changed");
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            skyview = !skyview;
            transform.LookAt(target.position);
            rotation = transform.rotation;

            if (skyview)
            {
                position = new Vector3(grid.sizeX / 2, 50f, grid.sizeZ / 2);
                freecam = true;
            }
            else freecam = false;
        }
        
        if (player.DicoCondition["BackToMovement"]())
        {
            if (GameManager.instance.playingPlaceable.Player == player) {
                GameManager.instance.playingPlaceable.ResetAreaOfTarget();
                GameManager.instance.state = States.Move;
                GameManager.instance.activeSkill = null;
                GameManager.instance.playingPlaceable.AreaOfMouvement = Grid.instance.CanGo(GameManager.instance.playingPlaceable.GetPosition(), GameManager.instance.playingPlaceable.CurrentPM,
                GameManager.instance.playingPlaceable.Jump, GameManager.instance.playingPlaceable.Player);
                GameManager.instance.playingPlaceable.ChangeMaterialAreaOfMovement(GameManager.instance.pathFindingMaterial);
            }      
        }

        //Debug.Log(transform.rotation);
        if (freecam)
        {
            if (!skyview)
            {
                position += rotation * Vector3.right * Input.GetAxis("Horizontal");
                position += rotation * Vector3.forward * Input.GetAxis("Vertical");
                position.y = y;
            }
        }

        else
        {
            if(target != null)
            { 
            position = target.position;
            }
        }


        //0 left 1 right 2 middle
        // If Middle button? ZOOM!

        // If middle mouse and left alt are selected? ORBIT

        if (player.DicoCondition["OrbitCamera"]())
        {
            XDeg += player.DicoAxis["AxisXCamera"]() * xSpeed * 0.02f;
            YDeg -= player.DicoAxis["AxisYCamera"]() * ySpeed * 0.02f;

            ////////OrbitAngle

            //Clamp the vertical axis for the orbit
            YDeg = ClampAngle(YDeg, yMinLimit, yMaxLimit);
            // set camera rotation 
            desiredRotation = Quaternion.Euler(YDeg, XDeg, 0);
            currentRotation = transform.rotation;

            rotation = Quaternion.Lerp(currentRotation, desiredRotation, Time.deltaTime * zoomDampening);

        }
        // otherwise if middle mouse is selected, we pan by way of transforming the target in screenspace
        else if (player.DicoCondition["PanCamera"]() && freecam)
        {
            //grab the rotation of the camera so we can move in a pseudo local XY space

            //target.rotation = transform.rotation;
            //transform.Translate(Vector3.right * -player.DicoAxis["AxisXCamera"]() * panSpeed);
            position += rotation * Vector3.right * -player.DicoAxis["AxisXCamera"]() * panSpeed;
            position += transform.up * -player.DicoAxis["AxisYCamera"]() * panSpeed;
            //transform.Translate(transform.up * -player.DicoAxis["AxisYCamera"]() * panSpeed/*, Space.World*/);
        }

        ////////Orbit Position

        // affect the desired Zoom distance if we roll the scrollwheel
        DesiredDistance -= player.DicoAxis["AxisZoomCamera"]() * Time.deltaTime * zoomRate * Mathf.Abs(DesiredDistance);
        //clamp the zoom min/max
        DesiredDistance = Mathf.Clamp(DesiredDistance, minDistance, maxDistance);
        // For smoothing of the zoom, lerp distance
        currentDistance = Mathf.Lerp(currentDistance, DesiredDistance, Time.deltaTime * zoomDampening);

        // calculate position based on the new currentDistance 
        transform.position = position - (rotation * Vector3.forward * currentDistance + targetOffset); ;
        transform.rotation = rotation;
    }

    private static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360)
            angle += 360;
        if (angle > 360)
            angle -= 360;
        return Mathf.Clamp(angle, min, max);
    }
}
