using UnityEngine;

public class BreakableByLeaf : MonoBehaviour
{
    public void BreakByLeaf() {
        // todo：能够被叶子打碎的对象的相关展示
        Destroy(gameObject);
    }
}
