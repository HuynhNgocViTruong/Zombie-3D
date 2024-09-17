using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MutantZombie : MonoBehaviour
{
    [Header("Zombie Var")]
    public ZombieValue zombieValue;
    private PlayerController playerBody;

    void Update()
    {
        if (!zombieValue.isDead)
        {
            if (zombieValue.player != null)
            {
                zombieValue.forMoving();
                zombieValue.checkPosition();
                checkVision();
            }
        }
    }

    void checkVision()
    {
        if (!zombieValue.playerInVision && !zombieValue.playerInAtk)
        {
            Guard();
        }
        else if (zombieValue.playerInVision && !zombieValue.playerInAtk)
        {
            if (!zombieValue.preAtk)
            {
                zombieValue.PursuePlayer();
            }
        }
        else if (zombieValue.playerInVision && zombieValue.playerInAtk)
        {
            if(zombieValue.AtkCount <= 5)
            {
                zombieValue.AttackPlayer(1.5f);
            }else if (zombieValue.AtkCount >= 6)
            {
                JumpAttack();
            }
        }
    }

    void Guard()
    {
        //StartCoroutine(WaitToRoar(10));
    }

    IEnumerator WaitToRoar(float waitTime)
    {
        zombieValue.zomAgent.SetDestination(transform.position);
        yield return new WaitForSeconds(waitTime);
        zombieValue.animator.SetTrigger("Roar");
    }

    void JumpAttack()
    {
        zombieValue.zomAgent.SetDestination(transform.position);
        transform.LookAt(zombieValue.player);
        zombieValue.checkRotation();
        if (!zombieValue.preAtk)
        {
            RaycastHit hitInfo;
            if (Physics.Raycast(zombieValue.AtkRaycastArea.transform.position, zombieValue.AtkRaycastArea.transform.forward, out hitInfo, zombieValue.atkRadius))
            {
                playerBody = hitInfo.transform.GetComponent<PlayerController>();

                if (playerBody != null)
                {
                    StartCoroutine(WaitToGiveDame(1.7f));
                }
            }
            zombieValue.preAtk = true;
            zombieValue.moving = false;
            Invoke(nameof(ActiveAttacking), 5);
        }
    }

    void ActiveAttacking()
    {
        zombieValue.moving = true;
        zombieValue.preAtk = false;
    }

    IEnumerator WaitToGiveDame(float waitTime)
    {
        zombieValue.animator.SetTrigger("JumpAttack");
        yield return new WaitForSeconds(waitTime);
        if (zombieValue.playerInAtk)
        {
            playerBody.PlayerTakeDamage(10);
        }
        zombieValue.AtkCount = 0;
    }
}
