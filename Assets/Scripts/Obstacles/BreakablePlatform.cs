using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakablePlatform : MonoBehaviour
{
    public float shakeDuration = 1f;  // Duration of shaking before breaking
    public float shakeMagnitude = 0.1f;  // Magnitude of shaking

    private Vector3 initialPosition;
    private bool isShaking = false;
    private bool isBreaking = false;
    private float shakeTimeRemaining;

    SpriteRenderer sr;
    BoxCollider2D bc2D;
    Animator anim;

    void Awake()
    {
        GetComponentInChildren<BreakablePlatformTrigger>().breakablePlatform = this;

        initialPosition = transform.localPosition;
        sr = GetComponent<SpriteRenderer>();
        bc2D = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
    }

    public void BreakPlatform()
    {
        StartCoroutine(BreakPlatformCoroutine());
    }

    IEnumerator BreakPlatformCoroutine()
    {
        if(isBreaking)
            yield break;
        
        isBreaking = true;

        // Shake the platform for the specified duration
        float elapsed = 0f;
        while (elapsed < shakeDuration)
        {
            transform.localPosition = initialPosition + Random.insideUnitSphere * shakeMagnitude;
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure the platform returns to its initial position
        transform.localPosition = initialPosition;
        anim.SetTrigger("Break");
        bc2D.enabled = false;

        yield return new WaitForSeconds(2);
        ResetToInitialState();
    }

    void ResetToInitialState()
    {
        anim.SetTrigger("Idle");
        isBreaking = false;
        bc2D.enabled = true;
    }
}
