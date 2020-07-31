using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    public NeuralNetworkWithArrays network;
    public List<ConfigurableJoint> steerJoints;
    public List<Rigidbody> wheels;
    private Vector3 startPosition, startRotation;
    public float timeSinceStart = 0f;
    private float timeForCheckPos;
    public Information inf;
    private bool canWrite = true;

    [Header("Настройки рейкаста")]
    public Vector3 offsetOfRaycastPoint = new Vector3(0,0,0);
    public float maxRange = 30;
    public List<Transform> rayPoints;
    public LayerMask layerToContact;

    [Header("Настройки движения машины")]
    public float wheelTorque = 1;
    public bool alive = true;
    public Rigidbody rb;

    [Header("Fitness")]
    public float overallFitness;
    public float distanceMultipler = 1.4f;
    public float avgSpeedMultiplier = 0.2f;
    public float sensorMultiplier = 0.1f;

    [Header("Network Options")]
    public List<float> inputsOfNN;

    private Vector3 lastPosition;
    private float totalDistanceTravelled;
    private float avgSpeed;

    public List<float> outputsOfNN;
    private void Start()
    {
        inf.aliveCreature += 1;
        lastPosition = transform.position;
        outputsOfNN.Add(0); outputsOfNN.Add(0);
        startPosition = transform.position;
        startRotation = transform.eulerAngles;

        foreach (Rigidbody rb in wheels)
        {
            rb.maxAngularVelocity = 100;
        }
    }

    private void FixedUpdate()
    {
        InputSensors();
        
        int inp = 0;
        foreach (float input in inputsOfNN)
        {
            network.inputNodes[inp, 0] = input;
            inp++;
        }

        network.Calculate();
        if (alive)
        {
            MoveCar((float)network.outputNodes[0, 0], (float)network.outputNodes[1, 0]);
        }
        else
        {
            if (canWrite)
            {
                inf.aliveCreature -= 1;
                canWrite = false;
            }
        }

        timeSinceStart += Time.fixedDeltaTime;
        timeForCheckPos += Time.fixedDeltaTime;
        CalculateFitness();
    }

    private void CalculateFitness()
    {
        
        if (timeForCheckPos >= 1)
        {
            timeForCheckPos = 0;
            totalDistanceTravelled += Vector3.Distance(transform.position, lastPosition);
            avgSpeed = totalDistanceTravelled / timeSinceStart;
            if (Vector3.Distance(transform.position, lastPosition) < 0.05)
            {
                alive = false;
                inf.aliveCreature -= 0;
            }

            overallFitness = (totalDistanceTravelled * distanceMultipler) + (FitnessOfSensors() * sensorMultiplier) +(avgSpeed * avgSpeedMultiplier);
            lastPosition = transform.position;
        }
    }

    float FitnessOfSensors()
    {
        float fit = 0;
        foreach (float input in inputsOfNN)
        {
            fit += input;
        }
        fit = fit / inputsOfNN.Count;
        return fit;
    }

    private void InputSensors()
    {
        inputsOfNN.Clear();
        RaycastHit hit;

        foreach (Transform rayPoint in rayPoints)
        {
            if (Physics.Raycast(rayPoint.position + offsetOfRaycastPoint, rayPoint.forward, out hit, maxRange, layerToContact))
            {
                inputsOfNN.Add(hit.distance / maxRange);
                Debug.DrawLine(rayPoint.position + offsetOfRaycastPoint, hit.point, Color.green);
            }
            else
            {
                inputsOfNN.Add(maxRange);
            }
        }
    }

    

    private Vector3 inp;
    public void MoveCar(float steer, float torque)
    {
        foreach (Rigidbody wheel in wheels)
        {
            wheel.AddTorque(transform.right * torque * wheelTorque, ForceMode.VelocityChange);
        }
        foreach (ConfigurableJoint joint in steerJoints)
        {
            joint.targetRotation = new Quaternion(0, steer, 0, 1);
        }
    }
}
