using UnityEngine;

public class InflatePlantItem : MonoBehaviour, IHoldable
{
    public GameObject inflatePlantPrefab;
    private Transform elfTransform;

    public bool IsItemConsumedAfterInteract() {
        return true;
    }

    public void OnPickedUp(Transform holdPoint) {
        transform.SetParent(holdPoint);
    }

    public void SetElfTransform(Transform _elfTransform) {
        elfTransform = _elfTransform;
    }

    public void OnInteracted() {
        GameObject inflatePlantObj = Instantiate(inflatePlantPrefab, elfTransform.position, Quaternion.identity);
        InflatePlant inflatePlant = inflatePlantObj.GetComponent<InflatePlant>();
        inflatePlant.SetPlantStateWhenSpawn();
        Destroy(gameObject);
    }

    public float GetWindForceMultiplier() {
        return 1f;
    }
}