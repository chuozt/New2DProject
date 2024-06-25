using UnityEngine;
using System;

public class PowerUpUnlocker : MonoBehaviour, IInteractable
{
    [SerializeField] private PowerUp newPowerUp = 0;
    [SerializeField, TextArea(1,4)] string guideText;
    bool isInteracted = false;

    public static event Action<PowerUp> onUnlockPowerUp;

    public void Interact()
    {
        if(isInteracted)
            return;
            
        isInteracted = true;
        onUnlockPowerUp?.Invoke(newPowerUp);
        PowerUpText.Instance.ShowPowerUpText(guideText);
        gameObject.SetActive(false);
    }
}

public enum PowerUp
{
    DoubleJump = 0,
    Floating,
    WallSliding,
    Dash
}
