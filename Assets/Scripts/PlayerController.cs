using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float baseSpeed;

    private Rigidbody rb;
    
    private Vector3 targetPosition;
    private Vector3 movementDirection;

    private int playerCubeCount = 0;

    private float currentSpeed;
    private float fixedYPosition;
    private float stopGlideThreshold = 0.01f;

    private bool canMove = true;
    private bool isMoving = false;
    private bool isOnIce = false;
    private bool hasReachedTarget = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.drag = 0f;
        // currentSpeed = baseSpeed;
        fixedYPosition = transform.position.y;
    }

    private void Update()
    {
        Debug.Log(currentSpeed);

        if (canMove && Input.GetMouseButtonDown(0))
        {
            HandleMouseClick();
        }
    }

    private void FixedUpdate()
    {
        if (canMove && isMoving)
        {
            if (isOnIce && hasReachedTarget)
            {
                HandleIceGlide();
            }
            else
            {
                MoveTowardsTarget();
            }
        }

        CheckTileUnderPlayer();
    }

    private void HandleMouseClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider != null && hit.collider.CompareTag("Tile"))
            {
                TileType tileType = hit.collider.GetComponent<TileType>();

                if (tileType != null && tileType.terrainType != TileType.TerrainType.Water)
                {
                    SetTargetPosition(hit.point);
                }
            }
        }
    }

    private void SetTargetPosition(Vector3 hitPoint)
    {
        targetPosition = hitPoint;
        targetPosition.y = fixedYPosition;

        movementDirection = (targetPosition - transform.position).normalized;

        isMoving = true;
        hasReachedTarget = false;
    }

    private void MoveTowardsTarget()
    {
        Ray ray = new Ray(transform.position, movementDirection);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Vector3.Distance(transform.position, targetPosition)))
        {
            TileType tileType = hit.collider.GetComponent<TileType>();

            if (tileType != null && tileType.terrainType == TileType.TerrainType.Water)
            {
                StopMoving();
                return;
            }
        }

        Vector3 newPosition = Vector3.MoveTowards(rb.position, targetPosition, currentSpeed * Time.fixedDeltaTime);
        newPosition.y = fixedYPosition;
        rb.MovePosition(newPosition);

        if (Vector3.Distance(rb.position, targetPosition) < 0.1f)
        {
            if (isOnIce)
            {
                rb.velocity = movementDirection * currentSpeed * 1f;
                hasReachedTarget = true;
            }
            else
            {
                StopMoving();
            }
        }
    }

    private void HandleIceGlide()
    {
        if (rb.velocity.magnitude > stopGlideThreshold)
        {
            rb.velocity = rb.velocity * 0.99f;
        }
        else
        {
            StopMoving();
        }
    }

    private void StopMoving()
    {
        rb.velocity = Vector3.zero;
        isMoving = false;
        hasReachedTarget = true;
    }

    private void CheckTileUnderPlayer()
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 5f))
        {
            if (hit.collider != null && hit.collider.CompareTag("Tile"))
            {
                TileType tileType = hit.collider.GetComponent<TileType>();

                if (tileType != null)
                {
                    ApplyTerrainSpeedModifier(tileType);
                }
            }
        }
    }

    private void ApplyTerrainSpeedModifier(TileType tile)
    {
        switch (tile.terrainType)
        {
            case TileType.TerrainType.Grass:
                rb.drag = 0f;
                currentSpeed = baseSpeed;
                isOnIce = false;
                break;

            case TileType.TerrainType.Forest:
                rb.drag = 0f;
                currentSpeed = baseSpeed * (1 - 0.25f);
                isOnIce = false;
                break;

            case TileType.TerrainType.Sand:
                rb.drag = 0f;
                currentSpeed = baseSpeed * (1 - 0.40f);
                isOnIce = false;
                break;

            case TileType.TerrainType.Mountain:
                rb.drag = 0f;
                HandleMountainTile(tile);
                isOnIce = false;
                break;

            case TileType.TerrainType.Water:
                rb.drag = 0f;
                StopMoving();
                isOnIce = false;
                break;

            case TileType.TerrainType.Ice:
                rb.drag = 0.1f;
                // currentSpeed = baseSpeed;
                isOnIce = true;
                break;

            default:
                break;
        }
    }

    private void HandleMountainTile(TileType tile)
    {
        if (tile.centerPoint != null)
        {
            float distanceToCenter = Vector3.Distance(transform.position, tile.centerPoint.position);
            float maxDistance = 3.5f;

            if (distanceToCenter < maxDistance / 2)
            {
                currentSpeed = baseSpeed * (1 - 0.30f);
            }
            else
            {
                currentSpeed = baseSpeed * (1 + 0.20f);
            }
        }
    }

    public void DisableMovement()
    {
        canMove = false;
        StopMoving();
    }

    public void EnableMovement()
    {
        canMove = true;
    }

    public int GetCubeCount()
    {
        return playerCubeCount;
    }

    public void AddCube()
    {
        playerCubeCount++;
    }

    public void RemoveCube()
    {
        if (playerCubeCount > 0)
        {
            playerCubeCount--;
        }
    }
}