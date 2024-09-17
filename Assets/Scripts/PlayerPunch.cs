using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPunch : MonoBehaviour
{
    [Header("Punch varlue")]
    public float punchDame = 10;
    public float punchRange = 5;
    private float nextPunch;
    public float punchCharge = 15;
    public Animator animator;
    public bool canPunch = true;
    public PlayerController player;

    [Header("Aim")]
    public GameObject TPS;
    public GameObject Aim;
    public GameObject GoredEffect;

    void Update()
    {
        if(player != null)
        {
            if (!player.playerDead)
            {
                if (canPunch)
                {
                    TPS.SetActive(false);
                    Aim.SetActive(false);
                }
                else if (!canPunch)
                {
                    TPS.SetActive(true);
                    Aim.SetActive(true);
                }

                if (Input.GetButton("Fire1") && Time.time >= nextPunch && canPunch)
                {
                    nextPunch = Time.time + 1 / punchCharge;

                    StartCoroutine(Wait(0.5f));
                    animator.SetTrigger("CrossPunch");
                }
            }
        }
    }

    IEnumerator Wait(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        Punch();
    }

    public void Punch()
    {
        RaycastHit hitInfo;

        if (Physics.Raycast(transform.position, transform.forward, out hitInfo, punchRange))
        {
            ZombieValue Zombie = hitInfo.transform.GetComponent<ZombieValue>();
            if (Zombie != null)
            {
                Zombie.ZomTakeDamage(punchDame);
                GameObject goreEffect = Instantiate(GoredEffect, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
                Destroy(goreEffect, 1f);
            }
        }
    }

    public void SetCanPunch(bool state)
    {
        canPunch = state;
    }
    public bool GetCanPunch()
    {
        return canPunch;
    }
}
