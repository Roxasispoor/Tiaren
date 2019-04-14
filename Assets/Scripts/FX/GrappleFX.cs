using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleFX : MonoBehaviour
{
    private LineRenderer rope;
    public Transform ropeConnect;

    public void ConnectRope(Material mat)
    {
        rope = gameObject.AddComponent<LineRenderer>();
        rope.material = mat;
        rope.widthMultiplier = 0.02f;
        rope.positionCount = 2;
        Vector3[] positions = new Vector3[] { ropeConnect.position, GameManager.instance.PlayingPlaceable.GetPosition() };
        rope.SetPositions(positions);
    }

    private void Update()
    {
        Vector3[] topPositions = new Vector3[] { ropeConnect.position, GameManager.instance.PlayingPlaceable.GetPosition() };
        rope.SetPositions(topPositions);
    }            
}
