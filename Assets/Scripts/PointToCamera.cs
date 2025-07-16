using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointToCamera : MonoBehaviour
{

    public Transform[] planeNumbers;
    Transform cam;

    void Start()
    {
        cam = Camera.main.transform;
        if (planeNumbers == null || planeNumbers.Length == 0)
            Debug.LogWarning("PointToCamera: no planeNumbers assigned!");
    }

    void LateUpdate()
    {
        if (cam == null) return;

        foreach (var tn in planeNumbers)
        {
            if (tn == null) continue;

            // dir = camera → object
            Vector3 dir = tn.position - cam.position;

            // Align +Z of tn to that direction
            tn.rotation = Quaternion.LookRotation(dir, Vector3.up);
        }
    }
}
