using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public PlaneController[] planes; // size = 5

    public void OnSelectPlane(int index)
    {
        int i = index - 1;
        if (i < 0 || i >= planes.Length || planes[i] == null)
        {
            return;
        }
        FlightManager.Instance.FocusOnPlane(planes[i].transform);
    }

    public void OnClearSelection()
    {
        FlightManager.Instance.ClearFocus();
    }
}