using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieValue : MonoBehaviour
{
    [Header("Zombie var")]
    public float giveDame = 2f;
    public float coolDownAtk;
    public float zomHealth;
    public float zomCurrentHealth;
    public bool preAtk;
    public bool isDead;

    [Header("Zombie Thing")]
    public NavMeshAgent zomAgent;
    public Transform LookPoint;
    public Transform player;
    public LayerMask PlayerLayer;
    public Animator animator;
    private PlayerController playerBody;

    [Header("Zombie Move Var")]
    public Camera AtkRaycastArea;
    public float ZomSpeed;
    private float speed;
    private float turnVelocity;
    private CapsuleCollider colliders;

    [Header("Zombie State")]
    public float visionRadius;
    public float atkRadius;
    public bool playerInVision = false;
    public bool playerInAtk = false;
    public bool moving = false;
    public int AtkCount = 0;

    private void Awake()
    {
        zomAgent = GetComponent<NavMeshAgent>();
        colliders = GetComponent<CapsuleCollider>();
        zomCurrentHealth = zomHealth;
        speed = ZomSpeed;
    }

    //Zom move and rotate
    public void forMoving()
    {
        if (moving == true)
        {
            animator.SetBool("Run", true);
            ZomSpeed = speed;
            transform.LookAt(player);
        }
        else if (moving == false)
        {
            animator.SetBool("Run", false);
            ZomSpeed = 0;
        }
    }

    public void checkRotation()
    {
        Vector3 direction = new Vector3(transform.position.x, 0, transform.position.z).normalized;
        if (direction.magnitude >= 0.1)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg; //Xoay theo góc di chuyển
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnVelocity, 0.3f);
            transform.rotation = Quaternion.Euler(0, angle, 0);
        }
    }

    public void checkPosition()
    {
        if (Vector3.Distance(transform.position, player.position) <= visionRadius)
        {
            playerInVision = true;
        }
        else if (Vector3.Distance(transform.position, player.position) > visionRadius)
        {
            playerInVision = false;
        }

        if (Vector3.Distance(transform.position, player.position) <= atkRadius)
        {
            playerInAtk = true;
        }
        else if (Vector3.Distance(transform.position, player.position) > atkRadius)
        {
            playerInAtk = false;
        }
    }

    //Move to player
    public void PursuePlayer()
    {
        if (zomAgent.SetDestination(player.position))
        {
            moving = true;
        }
        else
        {
            moving = false;
        }
        checkRotation();
    }


    //Zom dame

    public void AttackPlayer(float waitTime)
    {
        zomAgent.SetDestination(transform.position);
        transform.LookAt(player);
        checkRotation();
        if (!preAtk)
        {
            RaycastHit hitInfo;
            if (Physics.Raycast(AtkRaycastArea.transform.position, AtkRaycastArea.transform.forward, out hitInfo, atkRadius))
            {
                playerBody = hitInfo.transform.GetComponent<PlayerController>();

                if (playerBody != null)
                {
                    StartCoroutine(WaitToGiveDame(waitTime));
                }
            }
            preAtk = true;
            moving = false;
            Invoke(nameof(ActiveAttacking), coolDownAtk);
        }
    }

    void ActiveAttacking()
    {
        moving = true;
        preAtk = false;
    }

    IEnumerator WaitToGiveDame(float waitTime)
    {
        animator.SetTrigger("Attack");
        AtkCount += 1;
        yield return new WaitForSeconds(waitTime);
        if (playerInAtk)
        {
            playerBody.PlayerTakeDamage(giveDame);
        }
    }

    public void ZomTakeDamage(float takeDame)
    {
        zomCurrentHealth -= takeDame;
        if (zomCurrentHealth <= 0)
        {
            ZomDie();
        }
    }

    public void ZomDie()
    {
        zomAgent.SetDestination(transform.position);
        ZomSpeed = 0;
        speed = 0;
        visionRadius = 0;
        atkRadius = 0;
        playerInVision = false;
        playerInAtk = false;
        moving = false;
        animator.SetTrigger("Dead");
        Object.Destroy(gameObject, 5);
        isDead = true;
        colliders.enabled = false;
    }
}
