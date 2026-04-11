using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
// 植株交互核心组件的代码，继承自植株组件基类
public class PlantCore : PlantComponent
{
    [Header("环境范围")]
    [SerializeField] private Collider2D effectAreaCollider;

    [Header("联动组件列表")]
    // 将这棵植株所属的所有枝干、叶子、功能组件放入此列表
    public List<PlantComponent> connectedComponents = new List<PlantComponent>();
    
    // 新组件使用CanActivateByCore作为基类
    public List<CanActivateByCore> canActivatePlants = new List<CanActivateByCore>();

    public override void OnLocalInteract(PlantInteractionType interactionType)
    {
        if (interactionType == PlantInteractionType.Water) {
            ApplyPlantState(true);
            Debug.Log($"[PlantCore] {gameObject.name} 已被激活。");
        }
        else if (interactionType == PlantInteractionType.Grit) {
            ApplyPlantState(false);
            Debug.Log($"[PlantCore] {gameObject.name} 已枯萎。");
        }
    }

    private void ApplyPlantState(bool isWet)
    {
        // 改变核心自己的状态（继承自基类）
        SetState(isWet);

        // 指挥所有管理的组件变湿润/干瘪
        foreach (var comp in connectedComponents) {
            if (comp != null) {
                comp.SetState(isWet);
            }
        }
        if (isWet) {
            foreach (var comp in canActivatePlants) {
                if (comp != null) {
                    comp.ActivatedByCore();
                }
            }
        }

        // 所有制定范围内的枝干 Tile 也跟着变
        UpdateEnvironmentTiles(isWet);
    }

    public void UpdateEnvironmentTiles(bool toWet)
    {
        if (BranchTileManager.Instance == null) {
            Debug.LogWarning($"[PlantCore] {gameObject.name} 找不到 BranchTileManager，无法更新环境 Tile。");
            return;
        }

        Tilemap map = BranchTileManager.Instance.targetTilemap;
        if (map == null) {
            Debug.LogWarning($"[PlantCore] {gameObject.name} 的 BranchTileManager 未配置 targetTilemap。");
            return;
        }

        Collider2D rangeCollider = ResolveRangeCollider();
        if (rangeCollider == null) {
            Debug.LogWarning($"[PlantCore] {gameObject.name} 找不到范围碰撞体，无法更新环境 Tile。");
            return;
        }

        Bounds bounds = rangeCollider.bounds;

        // 2. 将世界坐标的范围转换为 Tilemap 的坐标范围
        Vector3Int min = map.WorldToCell(bounds.min);
        Vector3Int max = map.WorldToCell(bounds.max);

        // 3. 遍历这个矩形区域
        for (int x = min.x; x <= max.x; x++) {
            for (int y = min.y; y <= max.y; y++) {
                Vector3Int cellPos = new Vector3Int(x, y, 0);
                TileBase currentTile = map.GetTile(cellPos);

                if (currentTile != null) {
                    // 4. 询问手册：这个格子有对应的湿润/干枯版本吗？
                    TileBase nextTile = BranchTileManager.Instance.GetCounterpart(currentTile, toWet);
                    
                    if (nextTile != null) {
                        map.SetTile(cellPos, nextTile);
                    }
                }
            }
        }
    }

    private Collider2D ResolveRangeCollider() {
        if (effectAreaCollider != null) {
            return effectAreaCollider;
        }

        Transform effectArea = transform.Find("EffectArea");
        if (effectArea != null) {
            Collider2D childCollider = effectArea.GetComponent<Collider2D>();
            if (childCollider != null) {
                return childCollider;
            }
        }

        return GetComponent<Collider2D>();
    }
}
