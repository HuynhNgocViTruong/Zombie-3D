using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ak47 : MonoBehaviour
{
    [Header("Gun Thing")]
    public Camera cam;
    public float giveDame = 7;
    public float shootRange = 350;
    public float FireCharge = 15;
    private float NextShoot;
    private int ammo = 40;
    public int mag = 5; //Băng đạn
    private int presentAmmo;
    public float ReloadTime;
    private bool Reloading = false;
    public PlayerController player;
    public PlayerPunch playerPunch;
    public Animator animator;

    [Header("Gun Effect")]
    public ParticleSystem Spark;
    public GameObject WoodedEffect;
    public GameObject GoredEffect;


    void Awake()
    {
        presentAmmo = ammo;
    }

    // Update is called once per frame
    void Update()
    {
        if(player != null)
        {
            if (!player.playerDead)
            {
                if (Reloading) return;

                if (presentAmmo <= 0)
                {
                    StartCoroutine(Reload());
                    return;
                }

                if (Input.GetButton("Fire1") && Time.time >= NextShoot)
                {
                    NextShoot = Time.time + 2f / FireCharge;
                    Shoot();
                }
            }
        }
    }

    private void Shoot()
    {
        if(mag == 0)
        {
            playerPunch.SetCanPunch(true);
            gameObject.SetActive(false);
            return;
        }
        presentAmmo--;
        if(presentAmmo == 0)
        {
            mag--;
        }

        Spark.Play();
        RaycastHit hitInfo;

        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hitInfo, shootRange))
        {
            //Debug.Log(hitInfo.transform.name);
            Zombie01 zom = hitInfo.transform.GetComponent<Zombie01>();
            ZombieValue Zombie = hitInfo.transform.GetComponent<ZombieValue>();
            if (zom != null)
            {
                zom.ZomTakeDamage(giveDame);
                GameObject goreEffect = Instantiate(GoredEffect, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
                Destroy(goreEffect, 1f);
            }else if (Zombie != null)
            {
                Zombie.ZomTakeDamage(giveDame);
                GameObject goreEffect = Instantiate(GoredEffect, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
                Destroy(goreEffect, 1f);
            }
            else
            {
                GameObject WoodGo = Instantiate(WoodedEffect, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
                Destroy(WoodGo, 1f);
            }
            /*GameObject WoodGo = Instantiate(WoodedEffect, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
            Destroy(WoodGo, 1f);*/

        }
    }

    IEnumerator Reload()
    {
        player.speed = 0;
        player.run = 0;
        Reloading = true;
        animator.SetTrigger("Reload");
        yield return new WaitForSeconds(ReloadTime);

        animator.ResetTrigger("Reload");
        presentAmmo = ammo;
        player.speed = 2;
        player.run = 4;
        Reloading = false;
    }

    public void reset()
    {
        presentAmmo = ammo;
        mag = 5;
    }
}
