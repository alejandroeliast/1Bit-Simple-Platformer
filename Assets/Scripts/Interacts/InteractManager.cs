using System;
using UnityEngine;

public class InteractManager : MonoBehaviour
{
    private void Start()
    {
        PlayerController player = FindObjectOfType<PlayerController>();
        //player.OnGroundedChanged += Player_OnGroundedChanged;
    }

    private void Player_OnGroundedChanged(bool obj)
    {
        Debug.Log(obj);
    }
}
