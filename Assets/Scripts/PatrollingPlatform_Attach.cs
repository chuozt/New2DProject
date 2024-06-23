using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrollingPlatform_Attach : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.CompareTag("Player"))
            col.transform.SetParent(transform.parent.transform);
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if(col.CompareTag("Player"))
            col.transform.SetParent(null);
    }
}
