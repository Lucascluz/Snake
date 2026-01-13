using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Portal : MonoBehaviour
{
    public Collider2D gridArea;
    private Portal connectedPortal;

    private bool isTeleporting = false;

    public void RandomizePosition()
    {
        Bounds bounds = gridArea.bounds;

        // Pick a random position inside the bounds
        // Round the values to ensure it aligns with the grid
        int x = Mathf.RoundToInt(Random.Range(bounds.min.x, bounds.max.x));
        int y = Mathf.RoundToInt(Random.Range(bounds.min.y, bounds.max.y));

        // Prevent the Portal from spawning on the snake, food, obstacles, or other portals
        while (Game.Instance != null && Game.Instance.PositionOccupied(x, y))
        {
            x++;

            if (x > bounds.max.x)
            {
                x = Mathf.RoundToInt(bounds.min.x);
                y++;

                if (y > bounds.max.y)
                {
                    y = Mathf.RoundToInt(bounds.min.y);
                }
            }
        }

        transform.position = new Vector2(x, y);
    }

    public void connectPortal(Portal other)
    {
        connectedPortal = other;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if it's the snake
        if (other.gameObject.CompareTag("Player") && !isTeleporting && connectedPortal != null)
        {
            Snake snake = other.GetComponent<Snake>();
            if (snake != null)
            {
                TeleportSnake(snake);
            }
        }
    }

    private void TeleportSnake(Snake snake)
    {
        // Mark both portals as teleporting to prevent infinite loops
        isTeleporting = true;
        connectedPortal.isTeleporting = true;

        // Calculate new position based on snake's direction
        Vector3 newPosition = connectedPortal.transform.position;

        // Move the snake one step away from the exit portal
        if (snake.direction.x != 0f)
        {
            newPosition.x += snake.direction.x;
        }
        else if (snake.direction.y != 0f)
        {
            newPosition.y += snake.direction.y;
        }

        snake.transform.position = newPosition;

        // Reset teleporting flags after a delay
        Invoke(nameof(ResetTeleport), 0.2f);
    }

    private void ResetTeleport()
    {
        isTeleporting = false;
        if (connectedPortal != null)
        {
            connectedPortal.isTeleporting = false;
        }
    }
}
