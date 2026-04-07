using UnityEngine;
using System.Collections.Generic;
// 植株交互核心组件的代码，继承自植株组件基类
public class PlantCore : PlantComponent
{
    [Header("联动组件列表")]
    // 将这棵植株所属的所有枝干、叶子、功能组件放入此列表
    public List<PlantComponent> connectedComponents = new List<PlantComponent>();

    // 对外接口：唤醒整棵植株
    public void InteractWithWater()
    {
        // 改变核心自己的状态（继承自基类）
        SetState(true);

        // 指挥所有连接的组件变湿润
        foreach (var comp in connectedComponents)
        {
            if (comp != null)
            {
                comp.SetState(true);
            }
        }
        
        Debug.Log($"[PlantCore] {gameObject.name} 已被激活。");
    }

    // 对外接口：枯萎整棵植株
    public void InteractWithGrit()
    {
        // 1. 改变核心自己的状态
        SetState(false);

        // 2. 指挥所有连接的组件变干瘪
        foreach (var comp in connectedComponents)
        {
            if (comp != null)
            {
                comp.SetState(false);
            }
        }

        Debug.Log($"[PlantCore] {gameObject.name} 已枯萎。");
    }
}