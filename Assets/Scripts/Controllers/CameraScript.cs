
using UnityEngine;
using UnityEngine.Networking;

[AddComponentMenu("Camera-Control/3dsMax Camera Style")]
public class CameraScript : NetworkBehaviour
{
    private Transform target;
    public Vector3 targetOffset;
    public float distance = 6.0f;
    public float maxDistance = 25;
    public float minDistance = .6f;
    public float xSpeed = 6.0f;
    public float ySpeed = 6.0f;
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
    private int freecam = 0;  //Is the camera free to move or locked on player, or in skyview
    private Grid grid;

    private Vector3 spawncenter = new Vector3(0, 0, 0);

    public float XDeg { get { return xDeg; } set { xDeg = value; } }

    public float YDeg { get { return yDeg; } set { yDeg = value; } }

    public float DesiredDistance { get { return desiredDistance; } set { desiredDistance = value; } }

    public Vector3 SpawnCenter { get { return spawncenter; } set { spawncenter = value; } }

    public int Freecam { get { return freecam; } set { freecam = value; } }

    public void Init()
    {
        //If there is no target, create a temporary target at 'distance' from the cameras current viewpoint
        if (!target)
        {
            GameObject go = new GameObject("Cam Target");
            go.transform.position = spawncenter + (transform.forward * distance);
            target = go.transform;
        }

        currentDistance = distance;
        DesiredDistance = distance;

        //be sure to grab the current rotations as starting points.
        position = transform.position;
        rotation = transform.rotation;

        grid = GameObject.Find("GameManager").GetComponent<Grid>();

        Debug.Log(spawncenter);
        Vector3 spawntoobjective = new Vector3(grid.sizeX / 2, spawncenter.y, grid.sizeZ / 2) - spawncenter;
        XDeg = spawntoobjective.x >= 0 ? Vector3.Angle(Vector3.forward, spawntoobjective) : -Vector3.Angle(Vector3.forward, spawntoobjective);
        YDeg = Vector3.Angle(Vector3.up, spawntoobjective) - 45;
        //Debug.Log(XDeg + "  " + YDeg);
        rotation = Quaternion.Euler(YDeg, XDeg, 0);
    }

    public void SetTarget(Transform newtarget)
    {
        target = newtarget;
        Vector3 playertoobjective = new Vector3(grid.sizeX / 2, newtarget.position.y, grid.sizeZ / 2) - newtarget.position;
        XDeg = playertoobjective.x >= 0 ? Vector3.Angle(Vector3.forward, playertoobjective) : -Vector3.Angle(Vector3.forward, playertoobjective);
        YDeg = Vector3.Angle(Vector3.up, playertoobjective) - 45;
        DesiredDistance = distance;
        //Debug.Log(XDeg + "  " + YDeg);
        rotation = Quaternion.Euler(YDeg, XDeg, 0);
        position = target.position;
    }

    /*
     * Camera logic on LateUpdate to only update after all character movement logic has been handled. 
     */
    private void LateUpdate()
    {
        //Storing y position in order to not modifie the high when travelling with keyboard
        float y = position.y;

        if (!GameManager.instance.IsGameStarted)
        {
            //If there is no target, create a temporary target at 'distance' from the cameras current viewpoint
            if (!target)
            {
                GameObject go = new GameObject("Cam Target");
                go.transform.position = spawncenter + (transform.forward * distance);
                target = go.transform;
            }
            target.transform.position = spawncenter;
        }
        

        //If camera mode change
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            //updating camera mode
            freecam = freecam == 1 ? 0 : freecam + 1;

            //if skyview mode
            if (freecam == 2)
            {
                transform.LookAt(target.transform);
                rotation = transform.rotation;
                position = new Vector3(grid.sizeX / 2, grid.sizeY * 2, grid.sizeZ / 2);
            }
            else
            {
                currentDistance = distance;
            }
            //Debug.Log("Camera mode changed");
        }

        //if free cam mode
        if (freecam == 1)
        {
            //move according to input keys
            position += rotation * Vector3.right * Input.GetAxis("Horizontal") * 0.5f;
            position += rotation * Vector3.forward * Input.GetAxis("Vertical") * 0.5f;
            //reset the good high
            position.y = y;
        }

        //if in stuck camera mode
        else if (freecam == 0)
        {
            position = target.position;
        }


        //0 left 1 right 2 middle
        // If Middle button? ZOOM!

        // If middle mouse and left alt are selected? ORBIT

        if (player.DicoCondition["OrbitCamera"]())
        {
            XDeg += player.DicoAxis["AxisXCamera"]() * xSpeed;
            YDeg -= player.DicoAxis["AxisYCamera"]() * ySpeed;

            //Clamp the vertical axis for the orbit
            YDeg = ClampAngle(YDeg, yMinLimit, yMaxLimit);
            // set camera rotation 
            desiredRotation = Quaternion.Euler(YDeg, XDeg, 0);
            currentRotation = transform.rotation;

            rotation = Quaternion.Lerp(currentRotation, desiredRotation, Time.deltaTime * zoomDampening);

        }
        // otherwise if middle mouse is selected, we pan by way of transforming the target in screenspace
        else if (player.DicoCondition["PanCamera"]() && freecam == 1)
        {
            //grab the rotation of the camera so we can move in a pseudo local XY space

            //target.rotation = transform.rotation;
            //transform.Translate(Vector3.right * -player.DicoAxis["AxisXCamera"]() * panSpeed);
            position += rotation * Vector3.right * -player.DicoAxis["AxisXCamera"]() * panSpeed;
            position += transform.up * -player.DicoAxis["AxisYCamera"]() * panSpeed;
            //transform.Translate(transform.up * -player.DicoAxis["AxisYCamera"]() * panSpeed/*, Space.World*/);
        }

        //resume the movement part
        if (player.DicoCondition["BackToMovement"]())
        {
            BackToMovement();
        }


        // affect the desired Zoom distance if we roll the scrollwheel
        DesiredDistance -= player.DicoAxis["AxisZoomCamera"]() * Time.deltaTime * zoomRate * Mathf.Abs(DesiredDistance);
        //clamp the zoom min/max
        DesiredDistance = Mathf.Clamp(DesiredDistance, minDistance, maxDistance);
        // For smoothing of the zoom, lerp distance
        float newdist = Mathf.Lerp(currentDistance, DesiredDistance, Time.deltaTime * zoomDampening);

        // calculate position based on the new currentDistance 
        Vector3 newpos = position - (rotation * Vector3.forward * newdist + targetOffset);
        //check if the rotation make the camera fall under the floor

        if (newpos.y >= 1)
        {
            transform.position = newpos;
            transform.rotation = rotation;
            currentDistance = newdist;
        }
        else
        {
            Quaternion newrot = Quaternion.Euler(currentRotation.eulerAngles.x, rotation.eulerAngles.y, 0);
            transform.position = position - (newrot * Vector3.forward * currentDistance + targetOffset);
            transform.rotation = newrot;
            desiredRotation = newrot;
            DesiredDistance = newdist;
            YDeg = currentRotation.eulerAngles.x > 180 ? currentRotation.eulerAngles.x - 360 : currentRotation.eulerAngles.x;
            XDeg = currentRotation.eulerAngles.y;
        }
        //transform.position = position - (rotation * Vector3.forward * currentDistance + targetOffset);
        transform.position = new Vector3(transform.position.x, Mathf.Max(transform.position.y, 1), transform.position.z);
    }

    public void BackToMovement()
    {
        if (GameManager.instance.playingPlaceable.Player == player)
        {
            GameManager.instance.playingPlaceable.ResetTargets();
            RaycastSelector rayselect = GameManager.instance.playingPlaceable.player.GetComponentInChildren<RaycastSelector>();
            rayselect.EffectArea = 0;
            rayselect.Pattern = SkillArea.NONE;
            GameManager.instance.State = States.Move;
            GameManager.instance.activeSkill = null;
            GameManager.instance.playingPlaceable.AreaOfMouvement = Grid.instance.CanGo(GameManager.instance.playingPlaceable.GetPosition(), GameManager.instance.playingPlaceable.CurrentPM,
            GameManager.instance.playingPlaceable.Jump, GameManager.instance.playingPlaceable.Player);
            GameManager.instance.playingPlaceable.ChangeMaterialAreaOfMovement(GameManager.instance.pathFindingMaterial);
        }
    }

    private static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360)
        {
            angle += 360;
        }

        if (angle > 360)
        {
            angle -= 360;
        }

        return Mathf.Clamp(angle, min, max);
    }
}