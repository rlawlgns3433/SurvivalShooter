using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public class PlayerShooter : MonoBehaviour
{
    public Gun gun;
    private PlayerInput playerInput;

    private void Start()
    {
        if(!TryGetComponent(out playerInput))
        {
            playerInput.enabled = false;
            return;
        }
    }

    private void Update()
    {
        if (playerInput.fire)
        {
            gun.Fire();
        }
    }
}
