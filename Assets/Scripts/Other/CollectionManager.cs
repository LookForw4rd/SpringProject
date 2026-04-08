using UnityEngine;
using System.Collections.Generic;

public class CollectionManager : MonoBehaviour
{
    public static CollectionManager instance;
    public List<RectTransform> uiSlots; // 屏幕左上角的收集物槽位ui管理容器

    private void Awake() {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    // 使用index获取对应index的收集物槽位ui的transform，暴露给游戏环境
    public RectTransform GetSlotTransform(int slotIndex) {
        if (slotIndex >= 0 && slotIndex < uiSlots.Count)
            return uiSlots[slotIndex];
        return null;
    }
}
