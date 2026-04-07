using UnityEngine;
using System.Collections.Generic;
// 植株交互核心组件的代码，继承自植株组件基类
public class PlantCore : PlantComponent
{
    [Header("联动组件列表")]
    // 将这棵植株所属的所有枝干、叶子、功能组件放入此列表
    public List<PlantComponent> connectedComponents = new List<PlantComponent>();

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

        // 指挥所有连接的组件变湿润/干瘪
        foreach (var comp in connectedComponents)
        {
            if (comp != null)
            {
                comp.SetState(isWet);
            }
        }
    }
}
