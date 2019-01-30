using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour {
    
    private Vector3 end;

    [SerializeField]
    private float speed = 0.1f;
	// Use this for initialization
	void Start () {
	}
	
    /// <summary>
    /// Intialise the fireball with its target
    /// </summary>
    /// <param name="aimPosition">
    /// The position the fireball is going
    /// </param>
    public void Init(Vector3 aimPosition)
    {
        end = aimPosition;
        transform.LookAt(end);
    }

	// Update is called once per frame
	void Update () {
        transform.position += transform.forward * speed;

        // if we are close to the end point, do something (ex Destroy)
        if ((transform.position - end).magnitude < 0.1f)
        {
            Destroy(gameObject);
        }

    }
}
