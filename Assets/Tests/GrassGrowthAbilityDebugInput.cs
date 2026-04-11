using UnityEngine;

// 测试脚本：用按键临时开关“踩地生长”能力，不影响正式关卡逻辑
public class GrassGrowthAbilityDebugInput : MonoBehaviour
{
    [Header("目标角色")]
    [SerializeField] private ElfController targetElf;

    [Header("调试按键")]
    [SerializeField] private KeyCode unlockKey = KeyCode.Alpha9;
    [SerializeField] private KeyCode lockKey = KeyCode.Alpha0;

    private void Awake() {
        if (targetElf == null) {
            targetElf = GetComponent<ElfController>();
        }
    }

    private void Update() {
        if (targetElf == null) {
            return;
        }

        if (Input.GetKeyDown(unlockKey)) {
            targetElf.SetGrassGrowthAbility(true);
            Debug.Log("[DebugAbility] 踩地生长能力已解锁。");
        }

        if (Input.GetKeyDown(lockKey)) {
            targetElf.SetGrassGrowthAbility(false);
            Debug.Log("[DebugAbility] 踩地生长能力已关闭。");
        }
    }
}
