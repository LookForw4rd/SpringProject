using UnityEngine;

public interface IHoldable
{
    bool IsItemConsumedAfterInteract(); // 获取当前物品使用后是否消失的信息
    void OnPickedUp(Transform holdPoint); // 获取可握持物的交互接口
    void OnInteracted(); // 与当前握持物进行交互，消耗当前握持物的接口      
    float GetWindForceMultiplier(); // 获取当前物品对玩家所受风力的倍率影响
}