using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

class HandRotationDetection : MonoBehaviour
{

    [SerializeField] private Transform controller;
    [SerializeField] private Transform centerEyeAnchor;
    [SerializeField] private Rigidbody ballRB;
    public bool isTwisting { get; private set; }
    public float lastTwistTime { get; private set; }
    public float gracePeriod = 0.3f;


    [Header("Settings")]
    public float maxIdleTime = 0.2f;


    private float idleTimer = 0f;


    void Update()
    {
        if (controller.position.y < centerEyeAnchor.position.y)
        {
            Reset();
            return;
        }

        Vector3 velocity = ballRB.linearVelocity;

        Debug.LogWarning("Velocity: " + velocity.magnitude);
        if (velocity.magnitude < 0.4f)
        {
            idleTimer += Time.deltaTime;
            if (idleTimer > maxIdleTime)
            {
                Reset();
            }
            return;
        }
        else
        {
            idleTimer = 0;
        }
        isTwisting = true;
    
    }

    private void Reset()
    {
        isTwisting = false;
        idleTimer = 0f;
    }

}

