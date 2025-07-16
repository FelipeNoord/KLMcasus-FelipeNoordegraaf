using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorAssigner : MonoBehaviour
{
    // Start is called before the first frame update
    public ColorData colorData;
    void Start()
    {
        Renderer rend = GetComponent<Renderer>();

        // Check if the Renderer and material exist
        if (rend != null && rend.material != null)
        {
            // Change the material color to red (for example)
            rend.material.color = colorData.objectColor;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
