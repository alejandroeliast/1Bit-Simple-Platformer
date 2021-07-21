using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnOffManager : MonoBehaviour
{
    [SerializeField] protected OnOff.Type type;

    public List<OnOffSwitch> switches = new List<OnOffSwitch>();
    private List<OnOffBlock> blocks = new List<OnOffBlock>();

    private void Awake()
    {
        switches.Clear();
        foreach (var item in FindObjectsOfType<OnOffSwitch>())
        {
            switches.Add(item);
        }

        blocks.Clear();
        foreach (var item in FindObjectsOfType<OnOffBlock>())
        {
            blocks.Add(item);
        }
    }

    private void Start()
    {
        UpdateSwitches();
        UpdateBlocks();
    }

    private void UpdateSwitches()
    {
        foreach (var item in switches)
        {
            item.type = type;
            item.UpdateSprite();
        }
    }

    private void UpdateBlocks()
    {
        foreach (var item in blocks)
        {
            if (type == item.type)
            {
                item.isOn = true;
            }
            else
            {
                item.isOn = false;
            }

            item.UpdateCollider();
            item.UpdateSprite();
        }
    }

    public void ReverseState()
    {
        if (type == OnOff.Type.Off)
            type = OnOff.Type.On;
        else
            type = OnOff.Type.Off;

        UpdateSwitches();
        UpdateBlocks();
    }
}
