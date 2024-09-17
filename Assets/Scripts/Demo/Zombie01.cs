using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Zombie01 : MonoBehaviour
{
    [Header("Zombie var")]
    public float giveDame = 2f;
    public float coolDownAtk;
    private float zomHealth = 100;
    public float zomCurrentHealth;
    bool preAtk;
    bool isDead;

    [Header("Zombie Thing")]
    public NavMeshAgent zomAgent;
    public Transform LookPoint;
    public Transform player;
    public LayerMask PlayerLayer;
    public Animator animator;
    private PlayerController playerBody;

    [Header("Zombie Move Var")]
    public GameObject[] walkPoint;
    private int currentZomPosition = 0;
    public Camera AtkRaycastArea;
    public float ZomSpeed = 3;
    private float turnVelocity;
    private float walkPointRadius = 2;
    private CapsuleCollider colliders;

    [Header("Zombie State")]
    public float visionRadius;
    public float atkRadius;
    public bool playerInVision = false;
    public bool playerInAtk = false;
    private bool moving = false;



    private void Awake()
    {
        zomAgent = GetComponent<NavMeshAgent>();
        colliders = GetComponent<CapsuleCollider>();
        zomCurrentHealth = zomHealth;
    }

    void Update()
    {
        if (!isDead)
        {
            if (player != null)
            {
                forMoving();
                checkPosition();
                checkVision();
            }
        }
    }

    void checkVision()
    {
        if (!playerInVision && !playerInAtk)
        {
            Guard();
        }
        else if (playerInVision && !playerInAtk)
        {
            if (!preAtk)
            {
                PursuePlayer();
            }
        }
        else if (playerInVision && playerInAtk)
        {
            AttackPlayer();
        }
    }

    /// <summary>
    
    /// </summary>

    void checkPosition()
    {
        if(Vector3.Distance(transform.position, player.position) <= visionRadius)
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

    void checkRotation()
    {
        Vector3 direction = new Vector3(transform.position.x, 0, transform.position.z).normalized;
        if (direction.magnitude >= 0.1)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg; //Xoay theo góc di chuyển
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnVelocity, 0.5f);
            transform.rotation = Quaternion.Euler(0, angle, 0);
        }
    }

    void forMoving()
    {
        if (moving == true)
        {
            animator.SetBool("Run", true);
            ZomSpeed = 3;
        }
        else if (moving == false)
        {
            animator.SetBool("Run", false);
            ZomSpeed = 0;
        }
    }

    void Guard()
    {
        if(Vector3.Distance(walkPoint[currentZomPosition].transform.position, transform.position) < walkPointRadius)
        {
            currentZomPosition = Random.Range(0, walkPoint.Length);
            if(currentZomPosition >= walkPoint.Length)
            {
                currentZomPosition = 0;
            }
        }
        transform.position = Vector3.MoveTowards(transform.position, walkPoint[currentZomPosition].transform.position, Time.deltaTime * ZomSpeed);
        //thay đổi góc nhìn
        transform.LookAt(walkPoint[currentZomPosition].transform.position);
        checkRotation();
        moving = true;
    }

    void PursuePlayer()
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

    IEnumerator WaitToGiveDame(float waitTime)
    {
        animator.SetTrigger("Attack");
        yield return new WaitForSeconds(waitTime);
        if (playerInAtk)
        {
            playerBody.PlayerTakeDamage(giveDame);
        }
        
    }

    void AttackPlayer()
    {
        zomAgent.SetDestination(transform.position);
        transform.LookAt(player);
        checkRotation();
        if (!preAtk)
        {
            RaycastHit hitInfo;
            if(Physics.Raycast(AtkRaycastArea.transform.position, AtkRaycastArea.transform.forward, out hitInfo, atkRadius))
            {
                playerBody = hitInfo.transform.GetComponent<PlayerController>();

                if (playerBody != null)
                {
                    StartCoroutine(WaitToGiveDame(1.5f));
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
