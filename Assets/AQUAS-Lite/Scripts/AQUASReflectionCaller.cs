using UnityEngine;
using UnityEngine.Rendering;
using AQUAS_Lite; // Make sure this matches the namespace in AQUAS_Lite_Reflection.cs

[RequireComponent(typeof(Camera))]
public class AQUASReflectionCaller : MonoBehaviour
{
    void OnPreCull()
    {
        // Use new recommended method
        var waterObjects = Object.FindObjectsByType<AQUAS_Lite_Reflection>(FindObjectsSortMode.None);
        foreach (var water in waterObjects)
        {
            if (water.enabled && water.gameObject.activeInHierarchy)
            {
                water.RenderReflectionForCamera(GetComponent<Camera>());
            }
        }
    }
}
