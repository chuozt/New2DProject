using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchInputController : MonoBehaviour
{
    public GameObject moveLeftButtonBG;
    public GameObject moveRightButtonBG;
    public GameObject interactButtonBG;
    public GameObject jumpButtonBG;
    Rect rect1, rect2, rect3, rect4;

    private Vector2 touchPos;

    // void Awake()
    // {
    //     RectTransform r1 = moveLeftButtonBG.GetComponent<RectTransform>();
    //     RectTransform r2 = moveRightButtonBG.GetComponent<RectTransform>();
    //     rect1 = new Rect(r1.rect.position, r1.rect.size);
    //     rect2 = new Rect(r2.rect.position, r2.rect.size);
    // }

    // void Update()
    // {
    //     foreach(Touch touch in Input.touches)
    //     {
    //         if(touch.phase == TouchPhase.Began)
    //         {
    //             touchPos = touch.position;

    //             if(rect1.Contains(touchPos))
    //             {
    //                 Debug.Log("1");
    //             }
    //             if(rect2.Contains(touchPos))
    //             {
    //                 Debug.Log("2");
    //             }
    //             if(rect3.Contains(touchPos))
    //             {
    //                 Debug.Log("3");
    //             }
    //             if(rect4.Contains(touchPos))
    //             {
    //                 Debug.Log("4");
    //             }
    //         }
    //     }
    // }

    // void OnDrawGizmos()
    // {
    //     Gizmos.DrawWireCube(rect1.position, rect1.size);
    // }
}
