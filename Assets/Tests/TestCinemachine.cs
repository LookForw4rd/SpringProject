using UnityEngine;
using Unity.Cinemachine;

public class TestCinemachine : MonoBehaviour
{
    public CinemachineCamera vcamMicro; // 如果是旧版，用 CinemachineVirtualCamera
    public CinemachineCamera vcamMacro;

    public void Update() {
        if (Input.GetKeyDown(KeyCode.B))
            TransitionToMacroWorld();
    }

    public void TransitionToMacroWorld()
    {
        vcamMacro.Priority = 20; 
        vcamMicro.Priority = 10;
    }
}
