using UnityEngine;

public class PlantLeaf : PlantComponent
{
    [Header("会变化的叶子碰撞体")]
    [Tooltip("干瘪下垂时的垂直碰撞体（墙）")]
    [SerializeField] private Collider2D verticalLeafCollider;
    
    [Tooltip("湿润展开时的水平碰撞体（平台）")]
    [SerializeField] private Collider2D horizontalLeafCollider;

    // 我们可以通过代码控制，也可以在动画帧里控制，或者双管齐下
    protected override void UpdateVisuals(bool immediate) {
        // 执行基类逻辑
        base.UpdateVisuals(immediate);

        if (verticalLeafCollider == null || horizontalLeafCollider == null) {
            return;
        }

        // 叶子独有的碰撞体处理逻辑
        if (immediate){
            // 瞬间切换
            ToggleColliders(isIrrigated);
        }
        else{
            // 如果是正常生长，我们希望在 Growing 动画开始时就切换，或者在动画帧里精准控制
            // 这里先做简单的状态同步
            ToggleColliders(isIrrigated);
        }
    }

    private void ToggleColliders(bool irrigated) {
        // 湿润时：水平关闭，竖直开启
        // 干瘪时：竖直关闭，水平开启
        horizontalLeafCollider.enabled = irrigated;
        verticalLeafCollider.enabled = !irrigated;
    }
}