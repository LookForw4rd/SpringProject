using UnityEngine;

public class AirVent_tmp : MonoBehaviour
{
    public Vector2 windDir; // 风力方向
    public float windForce; // 风力强度

    private void OnTriggerStay2D(Collider2D other) {
        ElfController elf = other.GetComponent<ElfController>();
        if (elf != null) 
            elf.ApplyWindForce(windDir * windForce);
    }

    private void OnTriggerExit2D(Collider2D other) {
        ElfController elf = other.GetComponent<ElfController>();
        if (elf != null) 
            elf.ApplyWindForce(Vector2.zero);
    }
}
