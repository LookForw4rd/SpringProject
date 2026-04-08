using UnityEngine;

// 滴露处：被激活后通过动画事件持续生成水珠
public class PlantDew : PlantComponent
{
    [Header("滴露生成配置")]
    [SerializeField] private GameObject dripItemPrefab;
    [SerializeField] private Transform dripSpawnPoint;
    [SerializeField] private float minSpawnInterval = 0.1f;

    private float _lastSpawnTime = -999f;
    private bool _hasLoggedMissingPrefab = false;

    // 由动画最后一帧的 Animation Event 调用
    public void SpawnDripFromAnimationEvent() {
        Debug.Log($"[PlantDew] SpawnDripFromAnimationEvent called at time {Time.time} for {gameObject.name}");
        if (!isIrrigated) {
            return;
        }

        if (dripItemPrefab == null) {
            if (!_hasLoggedMissingPrefab) {
                Debug.LogWarning($"[PlantDew] {gameObject.name} 未配置 dripItemPrefab，无法生成水珠。");
                _hasLoggedMissingPrefab = true;
            }
            return;
        }

        if (Time.time - _lastSpawnTime < minSpawnInterval) {
            return;
        }

        Transform spawnPoint = dripSpawnPoint != null ? dripSpawnPoint : transform;
        Instantiate(dripItemPrefab, spawnPoint.position, Quaternion.identity);
        _lastSpawnTime = Time.time;
    }

    private void OnDrawGizmosSelected() {
        Transform spawnPoint = dripSpawnPoint != null ? dripSpawnPoint : transform;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(spawnPoint.position, 0.08f);
    }
}
