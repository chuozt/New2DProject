using UnityEngine;
using Cinemachine;

public class CinemachineCamera : MonoBehaviour
{
    CinemachineVirtualCamera cvc;
    Coroutine sizeChangeCoroutine;

    void OnEnable()
    {
        //FixedCameraZone.onTriggerCameraZone += StartChangeCameraSize;
    }

    void OnDisable()
    {
        //FixedCameraZone.onTriggerCameraZone -= StartChangeCameraSize;
    }

    void Start()
    {
        cvc = GetComponent<CinemachineVirtualCamera>();
    }

    // void StartChangeCameraSize(Transform newCameraPosition, float newSize, float zoomDuration = 0.75f)
    // {
    //     if(sizeChangeCoroutine != null)
    //         StopCoroutine(sizeChangeCoroutine);
        
    //     sizeChangeCoroutine = StartCoroutine(LerpCamera(newCameraPosition, newSize, zoomDuration));
    // }

    // IEnumerator LerpCamera(Transform newCameraPosition, float newSize, float zoomDuration)
    // {
    //     cvc.m_Follow = newCameraPosition;
    //     float initialSize = cvc.m_Lens.OrthographicSize;
    //     float elapsedTime = 0;

    //     while(elapsedTime < zoomDuration)
    //     {
    //         // Interpolate between initial and new size based on elapsed time
    //         float time = elapsedTime / zoomDuration;
    //         cvc.m_Lens.OrthographicSize = Mathf.Lerp(initialSize, newSize, time);

    //         elapsedTime += Time.deltaTime; // Update elapsed time
    //         yield return null; // Wait for the next frame
    //     }

    //     cvc.m_Lens.OrthographicSize = newSize;
    // }

    public void ChangeTrackedObjectOffset(Vector3 offset)
    {
        cvc.GetCinemachineComponent<CinemachineFramingTransposer>().m_TrackedObjectOffset = offset;
    }
}
