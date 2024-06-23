using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : Singleton<CameraManager>
{
    [SerializeField] private CinemachineVirtualCamera[] allVirtualCameras;
    [Header("Camera Shaking Settings"), Space(2)]
    [SerializeField] private float shakeIntensity = 1.75f;
    [SerializeField] private float shakeDuration = 0.3f;

    [Header("Camera Panning Settings"), Space(2)]
    [SerializeField] private float fallPanAmount = 0.25f;
    [SerializeField] private float fallYPanTime = 0.35f;
    public float fallSpeedYDampingChangeThreshold = -15;
    public bool IsLerpingYDamping { get; private set; }
    public bool LerpedFromPlayerFalling { get; set; }

    private Coroutine shakeCameraCoroutine;
    private Coroutine lerpYPanCoroutine;
    private Coroutine panCameraCoroutine;
    private CinemachineVirtualCamera currentCamera;
    private CinemachineFramingTransposer framingTransposer;

    private float normYPanAmount;
    private Vector2 startingTrackedObjectOffset;

    void Awake()
    {
        for(int i = 0; i < allVirtualCameras.Length; i++)
        {
            //Set shaking values for all cameras to 0
            allVirtualCameras[i].GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 0;

            if(allVirtualCameras[i].enabled)
            {
                //Set the current active camera
                currentCamera = allVirtualCameras[i];

                //Set the framing transposer
                framingTransposer = currentCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
            }
        }

        //Set the YDamping
        normYPanAmount = framingTransposer.m_YDamping;

        //Set the starting position of the tracked object offset
        startingTrackedObjectOffset = framingTransposer.m_TrackedObjectOffset;
    }

    #region ShakeCamera

    public void ShakeCamera()
    {
        shakeCameraCoroutine = StartCoroutine(ShakeCameraCoroutine());
    }

    private IEnumerator ShakeCameraCoroutine()
    {
        //Shake the current camera
        currentCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = shakeIntensity;

        float elapsedTime = 0;
        while(elapsedTime < shakeDuration)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        //Reset to 0
        currentCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 0;
    }

    #endregion

    #region LerpYDamping

    public void LerpYDamping(bool isPlayerFalling)
    {
        lerpYPanCoroutine = StartCoroutine(LerpYAction(isPlayerFalling));
    }

    private IEnumerator LerpYAction(bool isPlayerFalling)
    {
        IsLerpingYDamping = true;

        //Grab the starting damping amount
        float startDampAmount = framingTransposer.m_YDamping;
        float endDampAmount = 0;

        //Determine the end damping amount
        if(isPlayerFalling)
        {
            endDampAmount = fallPanAmount;
            LerpedFromPlayerFalling = true;
        }
        else
            endDampAmount = normYPanAmount;

        //Lerp the pan amount
        float elapsedTime = 0;

        while(elapsedTime < fallYPanTime)
        {
            elapsedTime += Time.deltaTime;

            float lerpedPanAmount = Mathf.Lerp(startDampAmount, endDampAmount, (elapsedTime/fallYPanTime));
            framingTransposer.m_YDamping = lerpedPanAmount;

            yield return null;
        }

        IsLerpingYDamping = false;
    }

    #endregion

    #region PanCamera

    public void PanCameraOnContact(float panDistance, float panTime, PanDirection panDirection, bool panToStartingPos)
    {
        panCameraCoroutine = StartCoroutine(PanCamera(panDistance, panTime, panDirection, panToStartingPos));
    }

    private IEnumerator PanCamera(float panDistance, float panTime, PanDirection panDirection, bool panToStartingPos)
    {
        Vector2 endPos = Vector2.zero;
        Vector2 startPos = Vector2.zero;

        //Handle the pan from trigger
        if(!panToStartingPos)
        {
            //Set the direction and the distance
            switch(panDirection)
            {
                case PanDirection.Up:
                    endPos = Vector2.up;
                    break;
                case PanDirection.Down:
                    endPos = Vector2.down;
                    break;
                case PanDirection.Left:
                    endPos = Vector2.right;
                    break;
                case PanDirection.Right:
                    endPos = Vector2.left;
                    break;
                default:
                    break;
            }

            endPos *= panDistance;
            startPos = startingTrackedObjectOffset;
            endPos += startPos;
        }
        //Handle the direction settings when moving back to the starting point
        else
        {
            startPos = framingTransposer.m_TrackedObjectOffset;
            endPos = startingTrackedObjectOffset;
        }

        //Handle the actual panning 
        float elapsed = 0;
        while(elapsed < panTime)
        {
            elapsed += Time.deltaTime;

            Vector3 panLerp = Vector3.Lerp(startPos, endPos, elapsed/panTime);
            framingTransposer.m_TrackedObjectOffset = panLerp;

            yield return null;
        }
    }

    #endregion

    #region SwapCameras

    public void SwapCameras(CinemachineVirtualCamera cameraFromLeft, CinemachineVirtualCamera cameraFromRight, Vector2 triggerExitDirection)
    {
        //If the current camera is the camera on the left, and trigger exit direction was on the right
        if(currentCamera == cameraFromLeft && triggerExitDirection.x > 0)
            ToggleCameras(cameraFromLeft, cameraFromRight, cameraFromRight);
        //Else if the current camera is the camera on the right, and trigger exit direction was on the left
        else if(currentCamera == cameraFromRight && triggerExitDirection.x < 0)
            ToggleCameras(cameraFromLeft, cameraFromRight, cameraFromLeft);
    }

    void ToggleCameras(CinemachineVirtualCamera cameraFromLeft, CinemachineVirtualCamera cameraFromRight, CinemachineVirtualCamera cameraToToggleOn)
    {
        //Deactivate all cameras
        cameraFromLeft.enabled = false;
        cameraFromRight.enabled = false;

        //Activate new camera
        cameraToToggleOn.enabled = true;

        //Set the current camera
        currentCamera = cameraToToggleOn;
        Debug.Log(cameraToToggleOn.name);

        //Update composer variables
        framingTransposer = currentCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
    }

    #endregion

    #region SwitchCameraBound

    public void SwitchHorizontalCameraBound(Collider2D boundOnLeft, Collider2D boundOnRight, Vector2 triggerExitDirection)
    {
        if(triggerExitDirection.x > 0)
            currentCamera.GetComponent<CinemachineConfiner2D>().m_BoundingShape2D = boundOnRight;
        else if(triggerExitDirection.x < 0)
            currentCamera.GetComponent<CinemachineConfiner2D>().m_BoundingShape2D = boundOnLeft;
    }

    public void SwitchVerticalCameraBound(Collider2D boundOnTop, Collider2D boundOnBottom, Vector2 triggerExitDirection)
    {
        if(triggerExitDirection.y > 0)
            currentCamera.GetComponent<CinemachineConfiner2D>().m_BoundingShape2D = boundOnTop;
        else if(triggerExitDirection.y < 0)
            currentCamera.GetComponent<CinemachineConfiner2D>().m_BoundingShape2D = boundOnBottom;
    }

    #endregion
}
