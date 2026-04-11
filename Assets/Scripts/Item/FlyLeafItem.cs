using UnityEngine;

public class FlyLeafItem : MonoBehaviour, IHoldable
{
    private Animator _animator;   
    private Rigidbody2D _rigidbody;
    private Collider2D _collider;
    
    private bool isInteracted = false;
    private bool isFlying = false;
    public float flySpeed = 6f;

    private void Awake() {
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
    }
    
    public bool IsItemConsumedAfterInteract() {
        return true;
    }

    public void OnPickedUp(Transform holdPoint) {
        transform.SetParent(holdPoint);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        _animator.SetBool("hold", true);
        _rigidbody.simulated = false;
    }

    public void OnInteracted() {
        if (isInteracted) return;
        isInteracted = true;
        isFlying = true;
        _animator.SetBool("hold", false);
        _animator.SetBool("flying", true);
        
        transform.SetParent(null);
        _rigidbody.simulated = true;
        _rigidbody.bodyType = RigidbodyType2D.Dynamic; 
        _rigidbody.gravityScale = 0f;
        _rigidbody.linearVelocity = transform.right * flySpeed; 
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (!isFlying) return;
        if (collision.GetComponent<ElfController>() != null) return; // 过滤玩家本身碰撞体的碰撞
        if (collision.isTrigger) return;

        if (collision.GetComponent<BreakableByLeaf>() != null) 
            collision.GetComponent<BreakableByLeaf>().BreakByLeaf();
        
        Shatter();
    }

    private void Shatter() {
        isFlying = false;
        _rigidbody.linearVelocity = Vector2.zero;
        _rigidbody.simulated = false;
        
        _animator.SetBool("flying", false);
        _animator.SetBool("break", true);
    }

    public void DestroyItem() {
        Destroy(gameObject);
    }

    public float GetWindForceMultiplier() {
        return 1f;
    }
}