using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpwardWind : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.CompareTag("Player"))
            col.gameObject.GetComponent<PlayerScript>().IsInUpwardWindZone = true;
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if(col.gameObject.CompareTag("Player"))
            col.gameObject.GetComponent<PlayerScript>().IsInUpwardWindZone = false;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position, transform.localScale);
    }
}
