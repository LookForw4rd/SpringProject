using UnityEngine;

public class PlantVent : PlantComponent
{
    public enum VentMode { Air, Water }
    private VentMode currentMode = VentMode.Air;

    // 在不同朝向的气孔预制体中对应调整
    [Header("物理风场配置")] 
    [Tooltip("风吹的方向")]
    [SerializeField] private Vector2 windDir = Vector2.up; 
    [Tooltip("风力强度")]
    [SerializeField] private float windForce = 10f;

    [Header("视觉/物理引用")]
    [SerializeField] private ParticleSystem airParticle;
    // [SerializeField] private ParticleSystem waterParticle;

    // 重写交互接口，响应玩家行动（例如玩家带着某种物品按 J）
    public override void OnLocalInteract(PlantInteractionType interactionType) {
        if (interactionType == PlantInteractionType.Water) {
            SetMode(VentMode.Water);
        }
        else if (interactionType == PlantInteractionType.Grit) {
            SetMode(VentMode.Air);
        }
    }

    public void SetMode(VentMode newMode) {
        currentMode = newMode;
        // 更新动画机的整数参数，让 AC 切换层或状态
        anim.SetInteger("Mode", (int)currentMode);
        UpdateVisuals(false);
    }

    private void OnTriggerStay2D(Collider2D other) {
        // 只有在非枯萎（isIrrigated）状态下才产生风力
        if (!isIrrigated) {
            return;
        }

        ElfController elf = other.GetComponent<ElfController>();
        if (elf != null) {
            // 将方向和强度结合，并传递给玩家
            elf.ApplyWindForce(windDir.normalized * windForce);
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        // 玩家离开时，无论植物状态如何，都应尝试清除玩家身上的该来源推力
        // 后续可能会有玩家运送物品的设计，所以这个逻辑可能会去掉
        ElfController elf = other.GetComponent<ElfController>();
        if (elf != null) {
            elf.ApplyWindForce(Vector2.zero);
        }
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
        if (airParticle == null) return;

        // 只有在湿润状态，且当前模式为 Air 时，才播放风粒子
        bool shouldPlayAir = isIrrigated && currentMode == VentMode.Air;

        if (shouldPlayAir && !airParticle.isPlaying) {
            airParticle.Play();
        } 
        else if (!shouldPlayAir && airParticle.isPlaying) {
            // Stop 可以让粒子不再产生新的，但已经存在的粒子会自然飘完生命周期，视觉过渡平滑
            airParticle.Stop(); 
        }

        // 针对 waterParticle 也可以写一套类似的逻辑
    }
}
