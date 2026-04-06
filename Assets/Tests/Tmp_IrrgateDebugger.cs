using UnityEngine;

public class PlantTester : MonoBehaviour
{
    [Header("测试目标")]
    public PlantCore targetCore;

    [Header("控制按键")]
    public KeyCode waterKey = KeyCode.Alpha1; // 按 1 灌溉
    public KeyCode sandKey = KeyCode.Alpha2;  // 按 2 干瘪

    void Update()
    {
        if (targetCore == null) return;

        if (Input.GetKeyDown(waterKey))
        {
            Debug.Log(">>> 模拟水滴交互");
            targetCore.InteractWithWater();
        }

        if (Input.GetKeyDown(sandKey))
        {
            Debug.Log(">>> 模拟沙砾交互");
            targetCore.InteractWithGrit();
        }
    }
}