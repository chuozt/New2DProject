using UnityEngine;

public class PatrollingPlatform : MonoBehaviour
{
    public Transform startPosition;
    public Transform endPosition;
    public float travelTime = 2.5f; // Fixed time to travel between points in seconds

    private Vector3 nextPosition;
    private bool movingToEnd = true;
    private float speed;

    void Start()
    {
        nextPosition = endPosition.position;
        CalculateSpeed();
    }

    void Update()
    {
        // Move the platform towards the next position
        transform.position = Vector3.MoveTowards(transform.position, nextPosition, speed * Time.deltaTime);

        // Check if the platform has reached the next position
        if (Vector3.Distance(transform.position, nextPosition) < 0.01f)
        {
            // Toggle the direction of movement and set the next position
            if (movingToEnd)
                nextPosition = startPosition.position;
            else
                nextPosition = endPosition.position;

            movingToEnd = !movingToEnd;

            // Recalculate speed for the next segment
            CalculateSpeed();
        }
    }

    void CalculateSpeed()
    {
        // Calculate the distance between the current position and the next position
        float distance = Vector3.Distance(transform.position, nextPosition);

        // Calculate the speed based on the distance and the desired travel time
        speed = distance / travelTime;
    }
}