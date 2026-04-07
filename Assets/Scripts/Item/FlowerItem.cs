using UnityEngine;

public class FlowerItem : MonoBehaviour, IHoldable
{
    private Animator _animator;
    private bool _isInteracted = false;
    private bool _isHeld = false;
    private ElfController _owner;
    [SerializeField] private float verticalBoostSpeed = 12f;

    private void Awake() {
        _animator = GetComponent<Animator>();
    }

    public bool IsItemConsumedAfterInteract() {
        // 花朵当前交互为一次性演示，触发后销毁
        return true;
    }

    public void OnPickedUp(Transform holdPoint) {
        _isHeld = true;
        _owner = holdPoint != null ? holdPoint.GetComponentInParent<ElfController>() : null;
        transform.SetParent(holdPoint);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        if (_animator != null) {
            _animator.SetBool("hold", true);
            _animator.SetBool("hold_in_air", false);
        }
    }

    public void OnInteracted() {
        if (_isInteracted) {
            return;
        }

        _isInteracted = true;
        _isHeld = false;
        ApplyLiftEffect();
        if (_animator != null) {
            _animator.SetBool("hold", false);
            _animator.SetBool("hold_in_air", false);
            _animator.SetTrigger("interact");
        }
        else {
            DestroyItemAfterInteract();
        }
    }

    private void Update() {
        if (_animator == null || _isInteracted || !_isHeld) {
            return;
        }

        if (_owner == null) {
            _owner = GetComponentInParent<ElfController>();
            if (_owner == null) {
                return;
            }
        }

        bool isInAir = !_owner.isGrounded();
        _animator.SetBool("hold", !isInAir);
        _animator.SetBool("hold_in_air", isInAir);
    }

    private void ApplyLiftEffect() {
        ElfController player = GetComponentInParent<ElfController>();
        if (player == null) {
            return;
        }

        player.ApplyVerticalBoost(verticalBoostSpeed);
    }

    // 由动画事件在 Flower_Interact 的最后一帧调用
    public void DestroyItemAfterInteract() {
        ElfController player = GetComponentInParent<ElfController>();
        if (player != null && player.currentHeldItem == (IHoldable)this) {
            player.currentHeldItem = null;
        }
        Destroy(gameObject);
    }

    public float GetWindForceMultiplier() {
        return 5f;
    }
}
