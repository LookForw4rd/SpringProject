using UnityEngine;

public class PlantFlower : PlantComponent
{
    [Header("花朵特有状态")]
    [SerializeField] private bool isPicked = false; // 是否已被采摘
    [SerializeField] private GameObject flowerItemPrefab;

    // 动画参数 Hash
    private static readonly int IsPickedHash = Animator.StringToHash("IsPicked");

    protected override void Awake() {
        base.Awake();
    }

    // 局部交互不直接执行采摘：玩家侧会通过 TryHarvest 获取并握持生成的花朵道具
    public override void OnLocalInteract(PlantInteractionType interactionType) {

    }

    public bool TryHarvest(Transform holdPoint, out IHoldable harvestedItem) {
        harvestedItem = null;
        if (!CanHarvest()) {
            return false;
        }

        Vector3 spawnPosition = holdPoint != null ? holdPoint.position : transform.position;
        GameObject flowerItemObj = Instantiate(flowerItemPrefab, spawnPosition, Quaternion.identity);
        IHoldable holdable = flowerItemObj.GetComponent<IHoldable>();

        // 兜底：prefab 上如果尚未挂 IHoldable，则动态补上最小 FlowerItem 组件
        // if (holdable == null) {
        //     FlowerItem runtimeFlowerItem = flowerItemObj.AddComponent<FlowerItem>();
        //     holdable = runtimeFlowerItem;
        // }

        // if (holdable == null) {
        //     Debug.LogError($"[PlantFlower] {gameObject.name} 生成的花朵道具缺少 IHoldable，采摘失败。");
        //     Destroy(flowerItemObj);
        //     return false;
        // }

        ExecutePickUp();
        harvestedItem = holdable;
        return true;
    }

    private bool CanHarvest() {
        if (!isIrrigated) {
            Debug.Log($"[PlantFlower] {gameObject.name} 已枯萎，无法采摘。");
            return false;
        }

        if (isPicked) {
            Debug.Log($"[PlantFlower] {gameObject.name} 已经被采摘过了。");
            return false;
        }

        if (flowerItemPrefab == null) {
            Debug.LogError($"[PlantFlower] {gameObject.name} 未配置 flowerItemPrefab，无法生成花朵道具。");
            return false;
        }

        return true;
    }

    private void ExecutePickUp()
    {
        isPicked = true;
        // 更新视觉表现
        UpdateVisuals(false);
        // 生成花朵 Item
        Debug.Log($"<color=cyan>[PlantFlower]</color> 玩家采摘了 {gameObject.name}。");
    }

    // 当核心导致枯萎时，花朵也需要响应
    public override void SetState(bool state, bool immediate = false)
    {
        base.SetState(state, immediate);
        // 特殊逻辑：如果植物枯萎了，且花还没被采摘
        if (!state && !isPicked) {
            // 这里可以触发花朵凋谢的特定表现，或者禁用交互
            Debug.Log($"[PlantFlower] {gameObject.name} 随核心一同枯萎。");
        }
    }

    protected override void UpdateVisuals(bool immediate)
    {
        base.UpdateVisuals(immediate);
        if (anim != null) {
            anim.SetBool(IsPickedHash, isPicked);
        }
    }
}
