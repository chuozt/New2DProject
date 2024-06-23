using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakablePlatformTrigger : MonoBehaviour
{
    [HideInInspector] public BreakablePlatform breakablePlatform;

    void OnTriggerStay2D(Collider2D col)
    {
        if(col.CompareTag("Player"))
            breakablePlatform.BreakPlatform();
    }
}
