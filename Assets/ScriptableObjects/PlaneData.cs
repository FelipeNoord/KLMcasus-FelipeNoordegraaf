using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlaneData", menuName = "ScriptableObjects/PlaneData")]
public class PlaneData : ScriptableObject
{
    public float planeOffsetY = 0.22f;
    public float groundSpeed = 1f;
    public float acceleration = 10f;
    public float flightSpeed = 10f;
    public float rotationSpeed = 1f;
    public Vector3 flightAreaCenter = Vector3.zero;
    public Vector3 flightAreaSize = new Vector3(100f, 50f, 100f);
}
