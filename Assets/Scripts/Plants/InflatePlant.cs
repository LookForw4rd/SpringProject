using UnityEngine;

public class InflatePlant : MonoBehaviour, CanActivateByCore
{
    public float smallBounceForce = 8f;
    public float bigBounceForce = 15f;
    public enum InflatePlantState { Withered, BeforeInflate, AfterInflate } // 枯萎状态，未膨胀状态，膨胀状态
    public InflatePlantState currentState;
    
    private Animator animator;
    public Collider2D smallCollider;
    public Collider2D largeCollider;
    
    public GameObject inflatePlantItem;

    private void Awake() {
        currentState = InflatePlantState.Withered;
        animator = GetComponent<Animator>();
        animator.SetBool("withered", true);
        
        smallCollider.enabled = true;
        largeCollider.enabled = false;
    }
    
    // 当前测试：使用6来让spiral grass进入灌溉状态
    private void Update() {
        if (Input.GetKeyDown(KeyCode.Alpha6))
            ActivatedByCore();
    }

    public void ActivatedByCore() {
        currentState = InflatePlantState.BeforeInflate;
        
        animator.SetBool("withered", false);
        animator.SetBool("before_inflate", true);
    }

    public GameObject ElfInteract(Transform holdPoint) {
        if (currentState == InflatePlantState.BeforeInflate) {
            GameObject inflatePlantObj = Instantiate(inflatePlantItem, holdPoint.position, Quaternion.identity);
            Destroy(gameObject);
            return inflatePlantObj;
        }
        
        return null;
    }

    public void IrrigatedByDrip() {
        if (currentState != InflatePlantState.BeforeInflate) return;
        currentState = InflatePlantState.AfterInflate;
        
        animator.SetBool("before_inflate", false);
        animator.SetBool("after_inflate", true);
        
        smallCollider.enabled = false;
        largeCollider.enabled = true;
    }

    public void DryByGravel() {
        currentState = InflatePlantState.BeforeInflate;
        animator.SetBool("after_inflate", false);
        animator.SetBool("before_inflate", true);
        
        smallCollider.enabled = true;
        largeCollider.enabled = false;
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (currentState == InflatePlantState.Withered) return;
        ElfController elf = collision.gameObject.GetComponent<ElfController>();
        if (elf == null) return;
        
        Vector2 bounceDir = (elf.transform.position - transform.position).normalized;
        float bounceForce = (currentState == InflatePlantState.BeforeInflate) ? smallBounceForce : bigBounceForce;
        elf.ApplyBounce(bounceDir * bounceForce, 0.2f);
    }

    public void SetPlantStateWhenSpawn() {
        currentState = InflatePlantState.BeforeInflate;
        animator.SetBool("withered", false);
        animator.SetBool("before_inflate", true);
    }
}