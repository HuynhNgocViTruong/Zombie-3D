using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ak47PickUp : MonoBehaviour
{
    [Header("Gun Thing")]
    public GameObject PlayerGun;
    public GameObject PickupGun;
    public PlayerPunch playerPunch;
    public bool isPick;

    [Header("Gun Assign Thing")]
    public PlayerController player;
    private float PickupRangeRadius = 2.5f;

    void Awake()
    {
        PlayerGun.SetActive(false);
    }

    void Update()
    {
        if (player != null)
        {
            if (Vector3.Distance(transform.position, player.transform.position) < PickupRangeRadius)
            {
                if (Input.GetKeyDown(KeyCode.F))
                {
                    PlayerGun.SetActive(true);
                    PlayerGun.GetComponent<Ak47>().reset();
                    playerPunch.SetCanPunch(false);
                }
            }
        }
    }
}
