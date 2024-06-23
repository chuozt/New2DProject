using UnityEngine;
using System;

public class PowerUpUnlocker : MonoBehaviour, IInteractable
{
    [SerializeField] private PowerUp newPowerUp = 0;
    bool isInteracted = false;

    public static event Action<PowerUp> onUnlockPowerUp;

    public void Interact()
    {
        if(isInteracted)
            return;
            
        isInteracted = true;
        onUnlockPowerUp?.Invoke(newPowerUp);
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
