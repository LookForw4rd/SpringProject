using UnityEngine;

public class SpiralGrassItem : MonoBehaviour, IHoldable
{
    public Sprite WitheredGrass;
    public Sprite IrrigatedGrass;
    public GameObject spiralGrassPrefab;
    private SpriteRenderer spriteRenderer;
    private SpiralGrass.SpiralGrassState grassState;
    private Transform elfTransform;

    private void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public bool IsItemConsumedAfterInteract() {
        return true;
    }

    public void OnPickedUp(Transform holdPoint) {
        transform.SetParent(holdPoint);
    }

    public void OnInteracted() {
        GameObject spiralGrassObj = Instantiate(spiralGrassPrefab, elfTransform.position, Quaternion.identity);
        SpiralGrass spiralGrass = spiralGrassObj.GetComponent<SpiralGrass>();
        spiralGrass.SetGrassStateWhenSpawn(grassState);
        Destroy(gameObject);
    }

    public float GetWindForceMultiplier() {
        return 1f;
    }

    public void SetSprite(Transform _elfTransform, SpiralGrass.SpiralGrassState _grassState) {
        elfTransform = _elfTransform;
        grassState = _grassState;
        if (grassState == SpiralGrass.SpiralGrassState.Withered)
            spriteRenderer.sprite = WitheredGrass;
        else if (grassState == SpiralGrass.SpiralGrassState.NoLeaf)
            spriteRenderer.sprite = IrrigatedGrass;
    }
}