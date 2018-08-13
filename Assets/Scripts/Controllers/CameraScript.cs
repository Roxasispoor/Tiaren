
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
    public Joueur joueur;

    private float xDeg = 0.0f;
    private float yDeg = 0.0f;
    private float currentDistance;
    private float desiredDistance;
    private Quaternion currentRotation;
    private Quaternion desiredRotation;
    private Quaternion rotation;
    private Vector3 position;

    
    public float XDeg
    {
        get
        {
            return xDeg;
        }

        set
        {
            xDeg = value;
        }
    }

    public float YDeg
    {
        get
        {
            return yDeg;
        }

        set
        {
            yDeg = value;
        }
    }

    public float DesiredDistance
    {
        get
        {
            return desiredDistance;
        }

        set
        {
            desiredDistance = value;
        }
    }

    void Start() {
        
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
        

    }

    /*
     * Camera logic on LateUpdate to only update after all character movement logic has been handled. 
     */
    void LateUpdate()
    {
     

        //0 gauche 1droite 2 milieu
        // If Middle button? ZOOM!

        // If middle mouse and left alt are selected? ORBIT

        if (joueur.DicoCondition["OrbitCamera"]())
        {
            XDeg += joueur.DicoAxis["AxisXCamera"]() * xSpeed * 0.02f;
            YDeg -= joueur.DicoAxis["AxisYCamera"]() * ySpeed * 0.02f;

            ////////OrbitAngle

            //Clamp the vertical axis for the orbit
            YDeg = ClampAngle(YDeg, yMinLimit, yMaxLimit);
            // set camera rotation 
            desiredRotation = Quaternion.Euler(YDeg, XDeg, 0);
            currentRotation = transform.rotation;

            rotation = Quaternion.Lerp(currentRotation, desiredRotation, Time.deltaTime * zoomDampening);
            transform.rotation = rotation;
        }
        // otherwise if middle mouse is selected, we pan by way of transforming the target in screenspace
        else if (joueur.DicoCondition["PanCamera"]())
        {
            //grab the rotation of the camera so we can move in a pseudo local XY space
            target.rotation = transform.rotation;
            target.Translate(Vector3.right * -joueur.DicoAxis["AxisXCamera"]() * panSpeed);
            target.Translate(transform.up * -joueur.DicoAxis["AxisYCamera"]() * panSpeed, Space.World);
        }

        ////////Orbit Position

        // affect the desired Zoom distance if we roll the scrollwheel
        DesiredDistance -= joueur.DicoAxis["AxisZoomCamera"]()  * Time.deltaTime * zoomRate * Mathf.Abs(DesiredDistance);
        //clamp the zoom min/max
        DesiredDistance = Mathf.Clamp(DesiredDistance, minDistance, maxDistance);
        // For smoothing of the zoom, lerp distance
        currentDistance = Mathf.Lerp(currentDistance, DesiredDistance, Time.deltaTime * zoomDampening);

        // calculate position based on the new currentDistance 
        position = target.position - (rotation * Vector3.forward * currentDistance + targetOffset);
        transform.position = position;
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