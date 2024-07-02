using UnityEngine;
using System;

public class FixedCameraZone : MonoBehaviour
{
    [SerializeField] private float newSize = 6;
    [SerializeField] private Transform newCameraPosition;
    float zoomDuration = 0.75f;

    public static event Action<Transform, float, float> onTriggerCameraZone;

    void Start()
    {
        if(newCameraPosition == null)
            newCameraPosition = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.CompareTag("Player"))
            onTriggerCameraZone?.Invoke(newCameraPosition, newSize, zoomDuration);
    }

    void OnDrawGizmos() => Gizmos.DrawWireCube(transform.position, transform.localScale);
}
