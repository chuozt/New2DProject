using UnityEngine;
using Cinemachine;

public class CinemachineCamera : MonoBehaviour
{
    CinemachineVirtualCamera cvc;
    Coroutine sizeChangeCoroutine;

    void Start() => cvc = GetComponent<CinemachineVirtualCamera>();

    public void ChangeTrackedObjectOffset(Vector3 offset) => cvc.GetCinemachineComponent<CinemachineFramingTransposer>().m_TrackedObjectOffset = offset;
}
