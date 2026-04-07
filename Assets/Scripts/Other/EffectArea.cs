using UnityEngine;
public class EffectArea : MonoBehaviour 
{
    public enum EffectType { Water, Grit }
    [Header("效果类型设置")]
    public EffectType thisType; 

    private void OnTriggerEnter2D(Collider2D other) 
    {
        // 尝试获取范围内的 PlantCore
        Debug.Log($"[EffectArea] {gameObject.name} 触发了 {other.gameObject.name} 的碰撞。");
        PlantCore core = other.GetComponent<PlantCore>();
        
        if (core != null) {
            // Debug.Log($"[EffectArea] {gameObject.name} 触发了 {core.gameObject.name} 的 {thisType} 效果。");
            // 根据物品自身的类型，调用 Core 对应的接口
            if (thisType == EffectType.Water) {
                core.InteractWithWater();
            }
            else if (thisType == EffectType.Grit){
                core.InteractWithGrit();
            }
        }
    }
}