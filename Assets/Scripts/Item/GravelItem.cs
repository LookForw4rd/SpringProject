using UnityEngine;

public class GravelItem : MonoBehaviour, IHoldable
{
    private Animator _animator;
    
    private void Awake() {
        _animator = GetComponent<Animator>();
    }

    public bool IsItemConsumedAfterInteract() {
        return true; // 沙砾在使用之后会消失
    }

    public void OnPickedUp(Transform holdPoint) {
        transform.SetParent(holdPoint);
        transform.localPosition = Vector3.zero;
    }

    public void OnInteracted() {
        // todo:当和沙砾交互时造成的影响逻辑
        _animator.SetBool("hold", false);
        _animator.SetBool("interact", true); // 进入交互动画
    }

    // 交互动画后自动调取此方式进行gravel item的删除
    public void DestroyItemAfterInteract() {
        Destroy(gameObject);
    }
}