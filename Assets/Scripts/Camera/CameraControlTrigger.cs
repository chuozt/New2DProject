using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEditor;

public class CameraControlTrigger : MonoBehaviour
{
    public CustomInspector customInspector;
    private Collider2D col2D;

    void Start()
    {
        col2D = GetComponent<Collider2D>();
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.CompareTag("Player"))
            if(customInspector.panCameraOnContact)
                CameraManager.Instance.PanCameraOnContact(customInspector.panDistance, customInspector.panTime, customInspector.panDirection, false);
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if(col.CompareTag("Player"))
        {
            Vector2 exitDir = (col.transform.position - col2D.bounds.center).normalized;

            //Swap cameras horizontally
            if(customInspector.swapCameraHorizontally && customInspector.cameraOnLeft != null && customInspector.cameraOnRight != null)
                CameraManager.Instance.SwapCameras(customInspector.cameraOnLeft, customInspector.cameraOnRight, exitDir);

            //Swap cameras vertically
            if(customInspector.swapCameraVertically && customInspector.cameraOnTop != null && customInspector.cameraOnBottom != null)
                CameraManager.Instance.SwapCamerasVertically(customInspector.cameraOnBottom, customInspector.cameraOnTop, exitDir);

            //Pan camera
            if(customInspector.panCameraOnContact)
                CameraManager.Instance.PanCameraOnContact(customInspector.panDistance, customInspector.panTime, customInspector.panDirection, true);

            //Switch camera's bound, horizontal only
            if(customInspector.switchHorizontalBounds && customInspector.boundOnLeft != null && customInspector.boundOnRight != null)
                CameraManager.Instance.SwitchHorizontalCameraBound(customInspector.boundOnLeft, customInspector.boundOnRight, exitDir);

            ////Switch camera's bound, vertical only
            if(customInspector.switchVerticalBounds && customInspector.boundOnTop != null && customInspector.boundOnBottom != null)
                CameraManager.Instance.SwitchVerticalCameraBound(customInspector.boundOnTop, customInspector.boundOnBottom, exitDir);
        }
    }

    void OnDrawGizmos()
    {
        if(customInspector.swapCameraHorizontally && !customInspector.panCameraOnContact)
            Gizmos.color = Color.yellow;
        else if(!customInspector.swapCameraHorizontally && customInspector.panCameraOnContact)
            Gizmos.color = Color.cyan;
        else if(customInspector.swapCameraHorizontally && customInspector.panCameraOnContact)
            Gizmos.color = Color.magenta;
        else if(!customInspector.swapCameraHorizontally && !customInspector.panCameraOnContact)
            Gizmos.color = Color.green;
        else
            Gizmos.color = Color.red;

        Gizmos.DrawWireCube(transform.position, new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z));
    }
}

public enum PanDirection
{
    Up,
    Down,
    Left,
    Right
}

[System.Serializable]
public class CustomInspector
{
    public bool switchHorizontalBounds = false;
    [HideInInspector] public Collider2D boundOnLeft, boundOnRight;

    public bool switchVerticalBounds = false;
    [HideInInspector] public Collider2D boundOnTop, boundOnBottom;

    public bool swapCameraHorizontally = false;
    [HideInInspector] public CinemachineVirtualCamera cameraOnLeft, cameraOnRight;

    public bool swapCameraVertically = false;
    [HideInInspector] public CinemachineVirtualCamera cameraOnTop, cameraOnBottom;

    public bool panCameraOnContact = false;
    [HideInInspector] public PanDirection panDirection;
    [HideInInspector] public float panDistance = 3f;
    [HideInInspector] public float panTime = 0.25f;
}

#if UNITY_EDITOR
[CustomEditor(typeof(CameraControlTrigger)), CanEditMultipleObjects]
public class ScriptEditor : Editor
{
    CameraControlTrigger cameraControlTrigger;
    
    void OnEnable()
    {
        cameraControlTrigger = (CameraControlTrigger)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if(cameraControlTrigger.customInspector.switchHorizontalBounds)
        {
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("Horizontal Bounds", EditorStyles.boldLabel);
            cameraControlTrigger.customInspector.boundOnLeft = EditorGUILayout.ObjectField("Bound On Left", cameraControlTrigger.customInspector.boundOnLeft, 
                                                                                            typeof(Collider2D), true) as Collider2D;

            cameraControlTrigger.customInspector.boundOnRight = EditorGUILayout.ObjectField("Bound On Right", cameraControlTrigger.customInspector.boundOnRight, 
                                                                                            typeof(Collider2D), true) as Collider2D;
        }

        if(cameraControlTrigger.customInspector.switchVerticalBounds)
        {
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("Vertical Bounds", EditorStyles.boldLabel);
            cameraControlTrigger.customInspector.boundOnTop = EditorGUILayout.ObjectField("Bound On Top", cameraControlTrigger.customInspector.boundOnTop, 
                                                                                            typeof(Collider2D), true) as Collider2D;

            cameraControlTrigger.customInspector.boundOnBottom = EditorGUILayout.ObjectField("Bound On Bottom", cameraControlTrigger.customInspector.boundOnBottom, 
                                                                                            typeof(Collider2D), true) as Collider2D;
        }

        if(cameraControlTrigger.customInspector.swapCameraHorizontally)
        {
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("Swap Cameras", EditorStyles.boldLabel);
            cameraControlTrigger.customInspector.cameraOnLeft = EditorGUILayout.ObjectField("Camera On Left", cameraControlTrigger.customInspector.cameraOnLeft,
                                                                                            typeof(CinemachineVirtualCamera), true) as CinemachineVirtualCamera;

            cameraControlTrigger.customInspector.cameraOnRight = EditorGUILayout.ObjectField("Camera On Right", cameraControlTrigger.customInspector.cameraOnRight,
                                                                                            typeof(CinemachineVirtualCamera), true) as CinemachineVirtualCamera;
        }

        if(cameraControlTrigger.customInspector.swapCameraVertically)
        {
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("Swap Cameras", EditorStyles.boldLabel);
            cameraControlTrigger.customInspector.cameraOnTop = EditorGUILayout.ObjectField("Camera On Top", cameraControlTrigger.customInspector.cameraOnTop,
                                                                                            typeof(CinemachineVirtualCamera), true) as CinemachineVirtualCamera;

            cameraControlTrigger.customInspector.cameraOnBottom = EditorGUILayout.ObjectField("Camera On Bottom", cameraControlTrigger.customInspector.cameraOnBottom,
                                                                                            typeof(CinemachineVirtualCamera), true) as CinemachineVirtualCamera;
        }

        if(cameraControlTrigger.customInspector.panCameraOnContact)
        {
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("Pan Camera", EditorStyles.boldLabel);
            cameraControlTrigger.customInspector.panDirection = (PanDirection)EditorGUILayout.EnumPopup("Camera Pan Direction", cameraControlTrigger.customInspector.panDirection);
            cameraControlTrigger.customInspector.panDistance = EditorGUILayout.FloatField("Pan Distance", cameraControlTrigger.customInspector.panDistance);
            cameraControlTrigger.customInspector.panTime = EditorGUILayout.FloatField("Pan Time", cameraControlTrigger.customInspector.panTime);
        }

        if(GUI.changed)
            EditorUtility.SetDirty(cameraControlTrigger);
    }
}
#endif
