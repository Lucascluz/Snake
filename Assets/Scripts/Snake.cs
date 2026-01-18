using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Snake : MonoBehaviour
{
    public Transform segmentPrefab;
    public Vector2Int direction = Vector2Int.right;
    public float speed = 20f;
    public float speedMultiplier = 1f;
    public int initialSize = 8;
    public bool moveThroughWalls = false;

    private readonly List<Transform> segments = new();
    private Vector2Int input;
    private float nextUpdate;
    
    // New: Wait for player input before starting
    private bool hasStarted = false;
    private bool isFirstInput = true;

    private void Start()
    {
        ResetState();
    }

    private void Update()
    {
        // Check for any directional input
        bool inputReceived = false;
        
        // Only allow turning up or down while moving in the x-axis
        if (direction.x != 0f)
        {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                input = Vector2Int.up;
                inputReceived = true;
            }
            else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                input = Vector2Int.down;
                inputReceived = true;
            }
        }
        // Only allow turning left or right while moving in the y-axis
        else if (direction.y != 0f)
        {
            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                input = Vector2Int.right;
                inputReceived = true;
            }
            else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                input = Vector2Int.left;
                inputReceived = true;
            }
        }
        
        // Also check for continuing in the same direction to start the game
        if (!hasStarted)
        {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow) ||
                Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow) ||
                Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow) ||
                Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                hasStarted = true;
                isFirstInput = false;
            }
        }
    }

    private void FixedUpdate()
    {
        // Don't move until the player provides input
        if (!hasStarted)
        {
            return;
        }
        
        // Wait until the next update before proceeding
        if (Time.time < nextUpdate)
        {
            return;
        }

        // Set the new direction based on the input
        if (input != Vector2Int.zero)
        {
            direction = input;
        }

        // Set each segment's position to be the same as the one it follows. We
        // must do this in reverse order so the position is set to the previous
        // position, otherwise they will all be stacked on top of each other.
        for (int i = segments.Count - 1; i > 0; i--)
        {
            segments[i].position = segments[i - 1].position;
        }

        // Move the snake in the direction it is facing
        // Round the values to ensure it aligns to the grid
        int x = Mathf.RoundToInt(transform.position.x) + direction.x;
        int y = Mathf.RoundToInt(transform.position.y) + direction.y;
        transform.position = new Vector2(x, y);

        // Set the next update time based on the speed
        nextUpdate = Time.time + (1f / (speed * speedMultiplier));
    }

    public void Grow()
    {
        Transform segment = Instantiate(segmentPrefab);
        segment.position = segments[segments.Count - 1].position;
        segments.Add(segment);
    }

    public void ResetState()
    {
        direction = Vector2Int.right;
        input = Vector2Int.zero;
        nextUpdate = Time.time;
        
        // Reset the started flag - wait for player input
        hasStarted = false;
        isFirstInput = true;

        transform.position = Vector3.zero;

        for (int i = 1; i < segments.Count; i++)
            Destroy(segments[i].gameObject);

        segments.Clear();
        segments.Add(transform);

        for (int i = 0; i < initialSize - 1; i++)
            Grow();

        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        if (collider != null)
        {
            collider.enabled = false;
            collider.enabled = true;
        }
    }
    
    /// <summary>
    /// Check if the snake has started moving (player has provided input)
    /// </summary>
    public bool HasStarted()
    {
        return hasStarted;
    }


    public bool Occupies(int x, int y)
    {
        foreach (Transform segment in segments)
        {
            if (Mathf.RoundToInt(segment.position.x) == x &&
                Mathf.RoundToInt(segment.position.y) == y)
            {
                return true;
            }
        }

        return false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Don't process collisions if game hasn't started yet
        if (!hasStarted)
        {
            return;
        }
        
        // Don't process collisions during game over or if time is paused
        if (Time.timeScale == 0f)
        {
            return;
        }

        if (other.gameObject.CompareTag("Food"))
        {
            Grow();
            if (Game.Instance != null)
            {
                Game.Instance.AddScore();
            }
        }
        else if (other.gameObject.CompareTag("Obstacle"))
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayObstacleHitSound();
            }
            if (Game.Instance != null)
            {
                Game.Instance.OnSnakeReset();
            }
        }
        else if (other.gameObject.CompareTag("Wall"))
        {
            if (moveThroughWalls)
            {
                Traverse(other.transform);
            }
            else
            {
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlayObstacleHitSound();
                }
                if (Game.Instance != null)
                {
                    Game.Instance.OnSnakeReset();
                }
            }
        }
    }

    private void Traverse(Transform wall)
    {
        Vector3 position = transform.position;

        if (direction.x != 0f)
        {
            position.x = Mathf.RoundToInt(-wall.position.x + direction.x);
        }
        else if (direction.y != 0f)
        {
            position.y = Mathf.RoundToInt(-wall.position.y + direction.y);
        }

        transform.position = position;
    }
}