using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    [Header("Wheels Collider")]
    public WheelCollider frontLeftCollider;
    public WheelCollider frontRightCollider;
    public WheelCollider backLeftCollider;
    public WheelCollider backRightCollider;

    [Header("Wheels Transform")]
    public Transform frontLeftTransform;
    public Transform frontRightTransform;
    public Transform backLeftTransform;
    public Transform backRightTransform;
    public Transform door;

    [Header("Vehicles Engine")]
    public float accelerationForce = 100f;
    public float breakingForce = 300f;
    private float presentbreakingForce = 0f;
    private float presentAccelerationForce = 0f;

    [Header("Vehicles Steering")]
    public float wheelsTorque = 20f;
    private float turnAngle;

    [Header("Vehicles Security")]
    public PlayerController playerController;
    private float radius = 10;
    public bool isOpen = false;

    [Header("Disable")]
    public GameObject AimCam;
    public GameObject AimCanvas;
    public GameObject TPCam;
    public GameObject TPCanvas;
    public GameObject player;


    void Update()
    {
        GetInVehicle();
        DisableCam();
    }

    void GetInVehicle()
    {
        if(Vector3.Distance(transform.position, playerController.transform.position) < radius)
        {
            if (Input.GetKey(KeyCode.F))
            {
                isOpen = true;
                radius = 5000f;
            }
            else if (Input.GetKey(KeyCode.G))
            {
                Vector3 nextDoor = new Vector3(door.transform.position.x, door.transform.position.y + 3, door.transform.position.z);
                playerController.transform.position = nextDoor;
                isOpen = false;
                radius = 10;
            }
        }
    }

    void DisableCam()
    {
        if(isOpen == true)
        {
            AimCam.SetActive(false);
            AimCanvas.SetActive(false);
            TPCam.SetActive(false);
            TPCanvas.SetActive(false);
            player.SetActive(false);

            MoveVehicle();
            VehicleSteering();
            Breaking();
        }
        else if(isOpen == false)
        {
            AimCam.SetActive(true);
            AimCanvas.SetActive(true);
            TPCam.SetActive(true);
            TPCanvas.SetActive(true);
            player.SetActive(true);
        }
    }

    void MoveVehicle()
    {
        presentAccelerationForce = accelerationForce * -Input.GetAxis("Vertical");

        frontLeftCollider.motorTorque = presentAccelerationForce;
        frontRightCollider.motorTorque = presentAccelerationForce;
        backLeftCollider.motorTorque = presentAccelerationForce;
        backRightCollider.motorTorque = presentAccelerationForce;
    }

    void VehicleSteering()
    {
        turnAngle = wheelsTorque * Input.GetAxis("Horizontal");
        frontLeftCollider.steerAngle = turnAngle;
        frontRightCollider.steerAngle = turnAngle;

        //turn the Wheel
        SteeringWheel(frontLeftCollider, frontLeftTransform);
        SteeringWheel(frontRightCollider, frontRightTransform);
        SteeringWheel(backLeftCollider, backLeftTransform);
        SteeringWheel(backRightCollider, backRightTransform);
    }

    void SteeringWheel(WheelCollider wc, Transform wt)
    {
        Vector3 position;
        Quaternion rotation;

        wc.GetWorldPose(out position, out rotation);

        wt.position = position;
        wt.rotation = rotation;
    }

    void Breaking()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            presentbreakingForce = breakingForce;
        }
        else
        {
            presentbreakingForce = 0;
        }

        frontLeftCollider.brakeTorque = presentbreakingForce;
        frontRightCollider.brakeTorque = presentbreakingForce;
        backLeftCollider.brakeTorque = presentbreakingForce;
        backRightCollider.brakeTorque = presentbreakingForce;
    }
}
