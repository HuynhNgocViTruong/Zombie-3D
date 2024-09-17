using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{


    [Header("Player Movement")]
    public float speed;
    public float run;
    public PlayerPunch playerPunch;

    [Header("Player var")]
    private float playerHealth = 100;
    public float currentHealth;
    public bool playerDead;


    [Header("Player Camera")]
    public Transform Camera;

    [Header("Player Animator and Gravity")]
    public CharacterController cC;
    public float gravity;
    public Animator animator;

    [Header("Player Jump and velocity")]
    public float turnTime;
    private float turnVelocity;
    public float jumpRange;
    private Vector3 velocity;
    public Transform surfaceCheck;
    private bool OnSurface;
    public float surfaceDistance;
    public LayerMask surfaceMask;
    private bool isRun = false;

    void Start()
    {
        //Tập trung chuột vào giữa màn hình
        Cursor.lockState = CursorLockMode.Locked;
        currentHealth = playerHealth;
    }

    void Update()
    {
        if (!playerDead)
        {
            //Áp dụng trọng lực lên player đảm bảo player trên đất
            OnSurface = Physics.CheckSphere(surfaceCheck.position, surfaceDistance, surfaceMask);
            if (OnSurface && velocity.y < 0)
            {
                velocity.y = -2;
            }
            velocity.y += gravity * Time.deltaTime;
            cC.Move(velocity * Time.deltaTime);

            if (playerPunch.GetCanPunch())
            {
                animator.SetBool("Aim", false);
            }
            else if (!playerPunch.GetCanPunch())
            {
                animator.SetBool("Aim", true);
            }

            isRunning();
            Jump();
        }
    }

    void isRunning()
    {
        if (Input.GetButtonDown("Runing"))
        {
            isRun = !isRun;
        }

        if (isRun)
        {
            Run();
        }
        else
        {
            playerMove();
        }
    }

    void playerMove()
    {
        float horizontal_axis = Input.GetAxisRaw("Horizontal");
        float vertical_axis = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3(horizontal_axis, 0, vertical_axis).normalized;

        if(direction.magnitude >= 0.1)
        {
            if (playerPunch.GetCanPunch())
            {
                animator.SetBool("Walk", true);
                animator.SetBool("Run", false);
                animator.SetBool("RifleWalk", false);
                animator.SetBool("RifleRun", false);
            }
            else if (!playerPunch.GetCanPunch())
            {
                animator.SetBool("RifleWalk", true);
                animator.SetBool("RifleRun", false);
                animator.SetBool("Walk", false);
                animator.SetBool("Run", false);

            }

            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + Camera.eulerAngles.y; //Xoay theo góc di chuyển + theo hướng camera
            //transform.rotation = Quaternion.Euler(0, targetAngle, 0);
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnVelocity, turnTime);//giúp việc xoay mịn dễ nhìn thấy hơn
            transform.rotation = Quaternion.Euler(0, angle, 0);

            Vector3 moveDirection = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;//di chuyển
            cC.Move(moveDirection.normalized * speed * Time.deltaTime);
        }
        else
        {
            animator.SetBool("Walk", false);
            animator.SetBool("RifleWalk", false);
        }
    }

    void Jump()
    {
        if (Input.GetButtonDown("Jump") && OnSurface)
        {
            velocity.y = Mathf.Sqrt(jumpRange * -2 * gravity);
            JumpState();
            //animator.SetTrigger("Jump");
            //StartCoroutine(Wait(0.5f));
        }
        else
        {
            animator.ResetTrigger("Jump");
            animator.ResetTrigger("JumpRifle");
        }
    }

    void JumpState()
    {
        if (playerPunch.GetCanPunch())
        {
            animator.SetTrigger("Jump");
        }else if (!playerPunch.GetCanPunch())
        {
            animator.SetTrigger("JumpRifle");
        }
    }

    IEnumerator Wait(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        velocity.y = Mathf.Sqrt(jumpRange * -2 * gravity);
    }

    void Run()
    {
        float horizontal_axis = Input.GetAxisRaw("Horizontal");
        float vertical_axis = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3(horizontal_axis, 0, vertical_axis).normalized;

        if (direction.magnitude >= 0.1)
        {
            if (playerPunch.GetCanPunch())
            {
                animator.SetBool("Walk", false);
                animator.SetBool("Run", true);
                animator.SetBool("RifleWalk", false);
                animator.SetBool("RifleRun", false);
            }
            else if (!playerPunch.GetCanPunch())
            {
                animator.SetBool("RifleWalk", false);
                animator.SetBool("RifleRun", true);
                animator.SetBool("Walk", false);
                animator.SetBool("Run", false);

            }

            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + Camera.eulerAngles.y; //Xoay theo góc di chuyển + theo hướng camera
                                                                                                              //transform.rotation = Quaternion.Euler(0, targetAngle, 0);
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnVelocity, turnTime);//giúp việc xoay mịn dễ nhìn thấy hơn
            transform.rotation = Quaternion.Euler(0, angle, 0);

            Vector3 moveDirection = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;//di chuyển
            cC.Move(moveDirection.normalized * run * Time.deltaTime);
        }
        else
        {
            animator.SetBool("Run", false);
            animator.SetBool("RifleRun", false);
        }
    }

    public void PlayerTakeDamage(float takeDame)
    {
        currentHealth -= takeDame;
        if(currentHealth <= 0)
        {
            PLayerDie();
        }
    }

    void PLayerDie()
    {
        Cursor.lockState = CursorLockMode.None; //Mở khóa chuột
        playerDead = true;
        animator.SetTrigger("Dead");
        Object.Destroy(gameObject, 3);
    }
}
