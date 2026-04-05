using UnityEngine;

public class PlantVent : PlantComponent
{
    public enum VentMode { Air, Water }
    private VentMode currentMode = VentMode.Air;

    // [Header("视觉/物理引用")]
    // [SerializeField] private ParticleSystem airParticle;
    // [SerializeField] private ParticleSystem waterParticle;

    // 重写交互接口，响应玩家行动（例如玩家带着某种物品按 J）
    public override void OnLocalInteract(string itemType) {
        if (itemType == "drip") {
            SetMode(VentMode.Water);
        }
        else if (itemType == "grit") {
            SetMode(VentMode.Air);
        }
    }

    public void SetMode(VentMode newMode) {
        currentMode = newMode;
        // 更新动画机的整数参数，让 AC 切换层或状态
        anim.SetInteger("Mode", (int)currentMode);
        UpdateVisuals(false);
    }

    protected override void UpdateVisuals(bool immediate) {
        // 先同步 Animator 的参数，保证状态机连线逻辑正确
        if (anim != null) {
            anim.SetBool(IsIrrigatedHash, isIrrigated);
            anim.SetInteger("Mode", (int)currentMode);
        }

        // 处理粒子效果
        UpdateParticles();

        // 处理“瞬间跳转”逻辑 (重写基类的硬编码部分)
        if (immediate && anim != null) {
            if (!isIrrigated){
                // 干瘪状态：直接跳回 Withered
                anim.Play("Withered", 0, 1f); 
            }
            else {
                // 湿润状态：根据当前 Mode 跳到对应的状态名
                anim.Play((currentMode == VentMode.Air) ? "AirMode" : "WaterMode", 0, 1f);
            }
        }
    }

    private void UpdateParticles(){
        // 粒子逻辑...
    }
}