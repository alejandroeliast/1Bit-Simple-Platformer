using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiamondPickUp : MonoBehaviour
{
    public int value;
    private DiamondManager manager;

    private void Awake()
    {
        manager = FindObjectOfType<DiamondManager>();
    }

    public void Collected()
    {
        manager.UpdateUI(value);
        Destroy(gameObject);
    }
}
