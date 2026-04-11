using UnityEngine;

public class SpiralGrass : MonoBehaviour, CanActivateByCore
{
    public GameObject flyLeaf; // 飞叶子Prefab
    public GameObject spiralGrass; // 螺旋草Prefab
    
    public enum SpiralGrassState { Withered, HaveLeaf, NoLeaf } // 枯萎状态、激活且有叶子可以交互的状态、激活且没有叶子的状态
    public SpiralGrassState currentState; // 当前的螺旋草状态
    private Animator animator; 

    private void Awake() {
        currentState = SpiralGrassState.Withered;
        animator = GetComponent<Animator>();
        animator.SetBool("withered", true);
    }

    // 当前测试：使用6来让spiral grass进入灌溉状态
    private void Update() {
        if (Input.GetKeyDown(KeyCode.Alpha6))
            ActivatedByCore();
    }

    // 当前场景的core被激活时螺旋草进入激活状态
    public void ActivatedByCore() {
        currentState = SpiralGrassState.HaveLeaf;
        
        // animator进入have_leaf状态
        animator.SetBool("withered", false);
        animator.SetBool("have_leaf", true);
    }
    
    public GameObject ElfInteract(Transform holdPoint) {
        if (currentState == SpiralGrassState.HaveLeaf) {
            // 获取植株的飞叶子
            GameObject flyLeafObj = Instantiate(flyLeaf, holdPoint.position, Quaternion.identity);
        
            currentState = SpiralGrassState.NoLeaf;
        
            // animator进入no_leaf状态
            animator.SetBool("have_leaf", false);
            animator.SetBool("no_leaf", true);
            return flyLeafObj;
        }
        else {
            // 直接获取植株
            GameObject spiralGrassObj = Instantiate(spiralGrass, holdPoint.position, Quaternion.identity);
            Destroy(gameObject);
            return spiralGrassObj;
        }
    }

    public void IrrigatedByDrip() {
        if (currentState != SpiralGrassState.NoLeaf) return;
        currentState = SpiralGrassState.HaveLeaf;
        
        // animator进入have_leaf状态
        animator.SetBool("no_leaf", false);
        animator.SetBool("have_leaf", true);
    }

    // 只需要考虑灌溉状态即可，默认为凋零状态
    public void SetGrassStateWhenSpawn(SpiralGrassState state) {
        if (state == SpiralGrassState.NoLeaf) {
            currentState = SpiralGrassState.NoLeaf;
            animator.SetBool("withered", false);
            animator.SetBool("no_leaf", true);
        }
    }
}
