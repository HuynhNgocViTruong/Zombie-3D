using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchCamera : MonoBehaviour
{
    [Header("Camera")]
    public GameObject aimCam;
    public GameObject aimCanvas;
    public GameObject ThirdPersonCam;
    public GameObject ThirdPersonCanvas;

    private bool AimShoot = false;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire2"))
        {
            AimShoot = !AimShoot;
        }

        if (AimShoot)
        {
            ThirdPersonCam.SetActive(false);
            ThirdPersonCanvas.SetActive(false);
            aimCam.SetActive(true);
            aimCanvas.SetActive(true);
        }
        else
        {
            ThirdPersonCam.SetActive(true);
            ThirdPersonCanvas.SetActive(true);
            aimCam.SetActive(false);
            aimCanvas.SetActive(false);
        }
    }
}
