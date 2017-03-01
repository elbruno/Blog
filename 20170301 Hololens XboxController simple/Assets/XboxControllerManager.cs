using HoloToolkit.Unity.SpatialMapping;
using UnityEngine;

public class XboxControllerManager : MonoBehaviour
{

    void Start() { }

    void Update()
    {
        var buttonA = Input.GetButton("Fire1");
        SpatialMappingManager.Instance.DrawVisualMeshes = buttonA;
    }
}
