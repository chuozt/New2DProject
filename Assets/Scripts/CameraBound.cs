using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBound : MonoBehaviour
{
    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, transform.localScale);
    }
}
