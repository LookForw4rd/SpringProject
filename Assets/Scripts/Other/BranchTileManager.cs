using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class BranchTileManager : MonoBehaviour
{
    public static BranchTileManager Instance; // 单例，方便 Core 调用
    public Tilemap targetTilemap;

    [System.Serializable]
    public struct TilePair {
        public TileBase dryTile;
        public TileBase wetTile;
    }

    public List<TilePair> mappingList;

    // 两个字典，实现双向快速查询
    private Dictionary<TileBase, TileBase> _toWetDict = new Dictionary<TileBase, TileBase>();
    private Dictionary<TileBase, TileBase> _toDryDict = new Dictionary<TileBase, TileBase>();

    private void Awake() {
        Instance = this;
        foreach (var pair in mappingList) {
            if (pair.dryTile != null && pair.wetTile != null) {
                _toWetDict[pair.dryTile] = pair.wetTile;
                _toDryDict[pair.wetTile] = pair.dryTile;
            }
        }
    }

    // 获取对应的 Tile
    public TileBase GetCounterpart(TileBase current, bool wantWet) {
        if (wantWet) {
            return _toWetDict.ContainsKey(current) ? _toWetDict[current] : null;
        } else {
            return _toDryDict.ContainsKey(current) ? _toDryDict[current] : null;
        }
    }
}