using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopZombie : MonoBehaviour
{
    [Header("Zombie Var")]
    public ZombieValue zombieValue;

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
            zombieValue.AttackPlayer(1.3f);
        }
    }

    void Guard()
    {

    }
}
