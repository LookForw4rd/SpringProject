using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Custom Tiles/SteppableGrass")]
public class SteppableGrassTile : Tile
{
    public Sprite[] tileSprites; // 草坪的四种形态对应sprite
    public TileBase[] decorationTiles; // 在地面上长的装饰性草tile
    [Range(0f, 1f)]public float spawnChance = 0.4f; // groundTile上方草的生成可能
    public static Dictionary<Vector3Int, int> steppableGrassRecords = new Dictionary<Vector3Int, int>(); // 记录地图中所有能够生长的草坪的信息

    // 重新运行游戏时防止steppableGrasRecords信息未被完全清空
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void Init() {
        steppableGrassRecords.Clear();
    }
    
    // 渲染当前场景tilemap中的某个tile的方法
    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData) {
        base.GetTileData(position, tilemap, ref tileData);
        steppableGrassRecords.TryGetValue(position, out int stepCount);
        stepCount = Mathf.Min(stepCount, tileSprites.Length - 1);
        tileData.sprite = tileSprites[stepCount];
    }

    // 当玩家踩到任意SteppableGrassTile时对对应tile进行sprite的更新
    public void OnStepped(Vector3Int position, Tilemap groundTilemap, Tilemap decorationTilemap) {
        if (tileSprites == null || tileSprites.Length == 0) return;
        
        steppableGrassRecords.TryGetValue(position, out int stepCount);
        if (stepCount < tileSprites.Length - 1) {
            int currentStep = stepCount + 1;
            steppableGrassRecords[position] = currentStep;
            groundTilemap.RefreshTile(position);
            if (currentStep == 2 || currentStep == 3) 
                TrySpawnDecoration(position, decorationTilemap, currentStep);
        }
    }

    // 尝试在绿色地面上生长装饰性草
    private void TrySpawnDecoration(Vector3Int groundPos, Tilemap decorationTilemap, int stepCount) {
        if (decorationTiles == null || decorationTiles.Length == 0) return;
        if (Random.value > spawnChance) return;
        if (decorationTilemap == null) return;

        Vector3Int abovePos = groundPos + Vector3Int.up;
        if (!decorationTilemap.HasTile(abovePos)) {
            TileBase decorationTile = decorationTiles[stepCount - 2];
            decorationTilemap.SetTile(abovePos, decorationTile);
        }
    }
}
