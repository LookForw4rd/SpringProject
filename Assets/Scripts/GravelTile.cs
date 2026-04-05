using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Custom Tiles/GravelTile")]
public class GravelTile : Tile
{
    public GameObject gravelItemPrefab;

    // 生成玩家获取的沙砾对象
    public GameObject GenerateGravelItem(Transform holdPoint) {
        return Instantiate(gravelItemPrefab, holdPoint.position, Quaternion.identity);
    }
}
