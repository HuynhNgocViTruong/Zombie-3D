using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialZombie : MonoBehaviour
{
    [Header("Zombie Var")]
    public ZombieValue zombieValue;
    public GameObject[] walkPoint;
    private int currentZomPosition = 0;
    private float walkPointRadius = 2;
    public GameObject specialZombie;

    void Update()
    {
        if (zombieValue.isDead == false)
        {
            if (zombieValue.player != null)
            {
                zombieValue.forMoving();
                zombieValue.checkPosition();
                checkVision();
            }
        }else if(zombieValue.isDead == true)
        {
            Object.Destroy(specialZombie, 5);
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
            zombieValue.AttackPlayer(1.5f);
        }
    }

    void Guard()
    {
        if (Vector3.Distance(walkPoint[currentZomPosition].transform.position, transform.position) < walkPointRadius)
        {
            currentZomPosition = Random.Range(0, walkPoint.Length);
            if (currentZomPosition >= walkPoint.Length)
            {
                currentZomPosition = 0;
            }
        }
        transform.position = Vector3.MoveTowards(transform.position, walkPoint[currentZomPosition].transform.position, Time.deltaTime * zombieValue.ZomSpeed);
        //thay đổi góc nhìn
        transform.LookAt(walkPoint[currentZomPosition].transform.position);
        zombieValue.checkRotation();
        zombieValue.moving = true;
    }
}
