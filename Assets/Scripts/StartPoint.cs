using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartPoint : MonoBehaviour
{
    public GameObject _prefab;

    private void Awake()
    {
        GameObject player = Instantiate(_prefab, transform.position, Quaternion.identity) as GameObject;
        player.name = "Player";
    }
}
