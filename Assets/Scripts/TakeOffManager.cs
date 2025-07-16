using System.Collections.Generic;
using UnityEngine;

public class TakeOffManager : MonoBehaviour
{
    public GameObject waypointsTakeoff;
    public GameObject waypointsLanding;
    public PlaneData planeData;
    private int planesLanded = 0;

    private List<PlaneController> planeScripts = new List<PlaneController>();

    void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            PlaneController pc = transform.GetChild(i).GetComponent<PlaneController>();
            if (pc != null)
            {
                pc.currentSpeed = planeData.groundSpeed;
                planeScripts.Add(pc);

                // Subscribe to individual plane callbacks
                pc.onTakeOffComplete += HandlePlaneTakenOff;
                pc.onLandingComplete += HandlePlaneLanded;
            }
            else
            {
                Debug.LogWarning("Missing PlaneController on child: " + transform.GetChild(i).name);
            }
        }
    }

    void Update()
    {
        foreach (PlaneController plane in planeScripts)
        {
            if (plane.isTakingOff)
                MovePlaneTakeoff(plane);
            else if (plane.isLanding)
                MovePlaneLanding(plane);
        }
    }

    public void StartTakeoff()
    {
        foreach (PlaneController plane in planeScripts)
        {
            plane.isTakingOff = true;
            plane.isLanding = false;
            plane.waypointTakeoffIndex = 0;
        }
    }

    public void StartLanding()
    {
        foreach (PlaneController plane in planeScripts)
        {
            plane.isLanding = true;
            plane.isTakingOff = false;
            plane.waypointLandingIndex = 0;
            plane.inTheAir = false;
        }
    }

    void MovePlaneTakeoff(PlaneController plane)
    {
        int count = waypointsTakeoff.transform.childCount;
        if (plane.waypointTakeoffIndex >= count)
        {
            plane.isTakingOff = false;
            return;
        }

        Transform target = waypointsTakeoff.transform.GetChild(plane.waypointTakeoffIndex);
        Transform planeTransform = plane.transform;

        int secondLastIndex = count - 3;
        if (plane.waypointTakeoffIndex < secondLastIndex)
            plane.currentSpeed = planeData.groundSpeed;
        else
        {
            float maxSpeed = planeData.flightSpeed * 2f;
            plane.currentSpeed = Mathf.Min(maxSpeed, plane.currentSpeed + planeData.acceleration * Time.deltaTime);
        }

        planeTransform.position = Vector3.MoveTowards(
            planeTransform.position,
            target.position,
            plane.currentSpeed * Time.deltaTime
        );

        Vector3 direction = (target.position - planeTransform.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            planeTransform.rotation = Quaternion.Slerp(
                planeTransform.rotation,
                lookRotation,
                planeData.rotationSpeed * Time.deltaTime
            );
        }

        if (Vector3.Distance(planeTransform.position, target.position) < 0.1f)
        {
            plane.waypointTakeoffIndex++;

            if (plane.waypointTakeoffIndex >= count)
            {
                plane.isTakingOff = false;
                plane.waypointTakeoffIndex = 0;
                plane.inTheAir = true;
                plane.onTakeOffComplete?.Invoke(plane);
                Debug.Log($"{plane.name} has taken off");
            }
        }
    }

    void MovePlaneLanding(PlaneController plane)
    {
        int count = waypointsLanding.transform.childCount;
        if (plane.waypointLandingIndex >= count) return;

        Transform target = waypointsLanding.transform.GetChild(plane.waypointLandingIndex);
        Transform planeTransform = plane.transform;

        // Deceleration logic
        if (plane.waypointLandingIndex <= 1)
        {
            float minSpeed = planeData.groundSpeed;
            plane.currentSpeed = Mathf.Max(
                minSpeed,
                plane.currentSpeed - (planeData.acceleration * Time.deltaTime)
            );
        }

        planeTransform.position = Vector3.MoveTowards(
            planeTransform.position,
            target.position,
            plane.currentSpeed * Time.deltaTime
        );

        Vector3 direction = (target.position - planeTransform.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            planeTransform.rotation = Quaternion.Slerp(
                planeTransform.rotation,
                targetRotation,
                planeData.rotationSpeed * Time.deltaTime
            );
        }

        if (Vector3.Distance(planeTransform.position, target.position) < 0.1f)
        {
            plane.waypointLandingIndex++;

            if (plane.waypointLandingIndex >= count)
            {
                plane.isLanding = false;
                plane.waypointLandingIndex = 0;
                plane.onLandingComplete?.Invoke(plane);

                plane.ReturnToHangar();

                Debug.Log($"{plane.name} has landed and is returning to hangar");
            }
        }
    }

    void HandlePlaneTakenOff(PlaneController plane)
    {
        Debug.Log($"[Event] {plane.name} took off!");
    }

    void HandlePlaneLanded(PlaneController plane)
    {
        Debug.Log($"[Event] {plane.name} landed!");
        planesLanded++;
    }
}
