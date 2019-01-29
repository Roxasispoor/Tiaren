using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour {

    public GameObject target;
    [SerializeField]
    private float speed;
	// Use this for initialization
	void Start () {
        transform.LookAt(target.transform);
	}
	
	// Update is called once per frame
	void Update () {
        transform.position += transform.forward * speed;

    }
}
