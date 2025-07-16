using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class PlaneController : MonoBehaviour
{
    [Header("Shared Data")]
    public PlaneData planeData;

    [Header("Flight State")]
    public bool inTheAir = false;
    public bool isTakingOff = false;
    public bool isLanding = false;
    public bool isReturning = false;

    [HideInInspector] public int waypointTakeoffIndex = 0;
    [HideInInspector] public int waypointLandingIndex = 0;
    [HideInInspector] public float currentSpeed = 0f;

    [Header("References")]
    public Transform parkingSpot;
    public GameObject headlight;

    private Vector3 currentTarget;

    [HideInInspector] public UnityAction<PlaneController> onTakeOffComplete;
    [HideInInspector] public UnityAction<PlaneController> onLandingComplete;

    void Start()
    {
        StartingPosition();
        if (inTheAir)
        {
            GetNewRandomTarget();
        }
    }

    void Update()
    {
        if (inTheAir)
        {
            HandleAIFlying();
        }

    }

    void HandleAIFlying()
    {
        Vector3 toTarget = (currentTarget - transform.position);
        Vector3 desiredDirection = toTarget.normalized;
        Vector3 steering = desiredDirection - transform.forward;
        Vector3 newDirection = (transform.forward + steering * Time.deltaTime * planeData.rotationSpeed).normalized;

        transform.position += newDirection * planeData.flightSpeed * Time.deltaTime;

        Quaternion targetRotation = Quaternion.LookRotation(newDirection);
        float bankAmount = Vector3.Dot(transform.right, desiredDirection) * 15f;
        targetRotation *= Quaternion.Euler(0f, 0f, -bankAmount);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * planeData.rotationSpeed);

        transform.position += transform.up * Mathf.Sin(Time.time * 0.3f) * 0.02f;

        if (Vector3.Distance(transform.position, currentTarget) < 20f)
            GetNewRandomTarget();

    }

    public void StartingPosition()
    {
        transform.position = parkingSpot.position + Vector3.up * planeData.planeOffsetY;
    }

    void GetNewRandomTarget()
    {
        float minH = 10f;
        float maxCenterY = Mathf.Max(planeData.flightAreaCenter.y, minH + 10f);
        float maxH = maxCenterY + planeData.flightAreaSize.y / 2;

        float x = Random.Range(-planeData.flightAreaSize.x / 2, planeData.flightAreaSize.x / 2);
        float y = Random.Range(minH, maxH);
        float z = Random.Range(-planeData.flightAreaSize.z / 2, planeData.flightAreaSize.z / 2);

        float noise = Mathf.PerlinNoise(Time.time * 0.5f, 0f) * 5f;
        y = Mathf.Clamp(y + noise, minH, maxH);

        currentTarget = planeData.flightAreaCenter + new Vector3(x, y, z);
    }

    public void StartRandomFlying()
    {
        inTheAir = true;
        GetNewRandomTarget();
    }

    public void ReturnToHangar()
    {
        isReturning = true;
        inTheAir = isTakingOff = isLanding = false;
        StartCoroutine(SmoothReturnToParkingSpot());
    }

    IEnumerator SmoothReturnToParkingSpot()
    {
        float speed = 5f;
        float rotSpeed = 2f;
        float threshold = 0.1f;

        Vector3 tgtPos = parkingSpot.position + Vector3.up * planeData.planeOffsetY;
        Quaternion tgtRot = parkingSpot.rotation;

        while (Vector3.Distance(transform.position, tgtPos) > threshold)
        {
            transform.position = Vector3.MoveTowards(transform.position, tgtPos, speed * Time.deltaTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, tgtRot, rotSpeed * Time.deltaTime);

            yield return null;
        }

        transform.position = tgtPos;
        transform.rotation = tgtRot;
        isReturning = false;
        onLandingComplete?.Invoke(this);
    }

    public void TurnLightsOn() => headlight.SetActive(true);
    public void TurnLightsOff() => headlight.SetActive(false);

}
