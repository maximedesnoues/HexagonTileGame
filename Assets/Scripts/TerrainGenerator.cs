using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    [Header("Tile Settings")]
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private int gridWidth;
    [SerializeField] private int gridHeight;
    [SerializeField] private float tileSpacingX;
    [SerializeField] private float tileSpacingZ;

    [Header("Terrain Types")]
    [SerializeField] private Material grassMaterial;
    [SerializeField] private Material forestMaterial;
    [SerializeField] private Material sandMaterial;
    [SerializeField] private Material mountainMaterial;
    [SerializeField] private Material waterMaterial;
    [SerializeField] private Material iceMaterial;

    private void Start()
    {
        GenerateTerrain();
    }

    private void GenerateTerrain()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Vector3 position = CalculateHexPosition(x, y);

                GameObject hexTile = Instantiate(tilePrefab, position, Quaternion.identity, this.transform);

                if (hexTile.GetComponent<Collider>() == null)
                {
                    hexTile.AddComponent<MeshCollider>();
                }

                hexTile.tag = "Tile";

                AssignRandomTerrainAndTileType(hexTile);
            }
        }
    }

    private Vector3 CalculateHexPosition(int x, int y)
    {
        float xPos = x * tileSpacingX;
        float zPos = y * tileSpacingZ;

        if (y % 2 == 1)
        {
            xPos += tileSpacingX / 2f;
        }

        return new Vector3(xPos, 0, zPos);
    }

    private void AssignRandomTerrainAndTileType(GameObject hexTile)
    {
        Renderer renderer = hexTile.GetComponent<Renderer>();

        int randomTerrain = Random.Range(0, 6);
        Material selectedMaterial = grassMaterial;
        TileType.TerrainType terrainType = TileType.TerrainType.Grass;

        switch (randomTerrain)
        {
            case 0:
                selectedMaterial = grassMaterial;
                terrainType = TileType.TerrainType.Grass;
                break;
            
            case 1:
                selectedMaterial = forestMaterial;
                terrainType = TileType.TerrainType.Forest;
                break;
            
            case 2:
                selectedMaterial = sandMaterial;
                terrainType = TileType.TerrainType.Sand;
                break;
            
            case 3:
                selectedMaterial = mountainMaterial;
                terrainType = TileType.TerrainType.Mountain;
                break;
            
            case 4:
                selectedMaterial = waterMaterial;
                terrainType = TileType.TerrainType.Water;
                break;
            
            case 5:
                selectedMaterial = iceMaterial;
                terrainType = TileType.TerrainType.Ice;
                break;

            default:
                break;
        }

        if (renderer != null)
        {
            renderer.material = selectedMaterial;
        }

        TileType tileTypeComponent = hexTile.AddComponent<TileType>();
        tileTypeComponent.terrainType = terrainType;

        if (terrainType == TileType.TerrainType.Mountain)
        {
            GameObject centerPoint = new GameObject("CenterPoint");
            centerPoint.transform.position = hexTile.transform.position;
            centerPoint.transform.SetParent(hexTile.transform);
            tileTypeComponent.centerPoint = centerPoint.transform;
        }
    }
}