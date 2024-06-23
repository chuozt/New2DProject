using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.CompareTag("Player"))
            col.gameObject.GetComponent<PlayerScript>().SetCheckpoint(transform);
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, transform.localScale);
    }
}
