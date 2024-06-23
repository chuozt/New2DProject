using UnityEngine;

public class RotatingSpikes : MonoBehaviour
{
    public float rotationDuration = 2.5f; // Duration to complete a full rotation in seconds
    public Vector3 rotationAxis = Vector3.forward; // Axis to rotate around

    void Update()
    {
        // Calculate the rotation amount for this frame
        float rotationThisFrame = 360f / rotationDuration * Time.deltaTime;

        // Rotate the object
        transform.Rotate(rotationAxis, rotationThisFrame);
    }
}