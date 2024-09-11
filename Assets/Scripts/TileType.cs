using UnityEngine;

public class TileType : MonoBehaviour
{
    public enum TerrainType
    {
        Grass,
        Forest,
        Sand,
        Mountain,
        Water,
        Ice
    }

    public TerrainType terrainType;

    public Transform centerPoint;
}