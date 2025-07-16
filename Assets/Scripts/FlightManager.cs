using UnityEngine;

public class FlightManager : MonoBehaviour
{
    public static FlightManager Instance { get; private set; }

    [Header("Cameras")]
    [Tooltip("Your scene’s main overview camera")]
    public Camera mainCamera;

    [Tooltip("Third‑person camera for flying planes")]
    public Camera thirdPersonCam;

    [Tooltip("Offset of the camera relative to the plane")]
    public Vector3 cameraOffset = new Vector3(0, 5, -10);

    void Awake()
    {
        Instance = this;
        if (mainCamera != null) mainCamera.enabled = true;
        if (thirdPersonCam != null) thirdPersonCam.enabled = false;
    }

    public void FocusOnPlane(Transform planeTransform)
    {
        if (mainCamera != null) mainCamera.enabled = false;
        if (thirdPersonCam != null) thirdPersonCam.enabled = true;

        thirdPersonCam.transform.SetParent(planeTransform, false);
        thirdPersonCam.transform.localPosition = cameraOffset;
        thirdPersonCam.transform.localRotation = Quaternion.identity;
    }

    public void ClearFocus()
    {
        if (thirdPersonCam != null) thirdPersonCam.enabled = false;
        if (mainCamera != null) mainCamera.enabled = true;
        thirdPersonCam.transform.SetParent(null);
    }
}
