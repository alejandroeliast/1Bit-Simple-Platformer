using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiamondManager : MonoBehaviour
{
    public List<Sprite> diamondSprites = new List<Sprite>();

    public List<Image> diamondPegs = new List<Image>();
    private List<DiamondPickUp> diamonds = new List<DiamondPickUp>();

    private void Awake()
    {
        diamonds.Clear();
        foreach (var item in FindObjectsOfType<DiamondPickUp>())
        {
            diamonds.Add(item);
        }
    }

    public void UpdateUI(int val)
    {

        diamondPegs[val - 1].sprite = diamondSprites[1];
    }
}
