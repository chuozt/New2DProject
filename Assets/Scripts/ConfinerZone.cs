using UnityEngine;
using System;

public class ConfinerZone : MonoBehaviour
{
    [SerializeField] private PolygonCollider2D pc2D;
    public static event Action<PolygonCollider2D> onBoundingChanged;

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.CompareTag("Player"))
            onBoundingChanged?.Invoke(pc2D);
    }
}
