 using System;
 using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
 
/**
* Done by KPP (pern) in 2025!
*/
 
public class Car : MonoBehaviour
{
    // the rigid body of the car
    private Rigidbody rb;
 
    // the rigid body of the bumpers
    private Rigidbody leftBumper;
    private Rigidbody rightBumper;
 
    private float forceLeft;
    private float forceRight;
 
    // the Exporter script (if attached to the game object)
    private Exporter exporter;
 
    // the time the car was launched
    private float launchTime;
 
    // flag to remember if car was launched
    private bool isLaunched = false;
 
    // the width of the car (could also be gotten from the collider)
    private readonly float carWidth = 0.3f;
 
    // the width of the bumpers (could also be gotten from the collider)
    private readonly float bumperWidth = 0.1f;
 
    // determines the state of the bumpers
    // 0: both bumpers are fixed
    // 1: left bumper is free to move, no friction
    // 2: left bumper is free to move, with friction
    public int bumperMode = 2;
 
    // helper flag to automatically start the car when recording starts
    // Window > General > Recorder > Start Recording
    private readonly bool recording = true;
 
    // initial velocity of the car
    public float initialVelocity = 1f;
 
    // the length of the uncompressed spring
    public float springLength = 0.15f;
 
    // spring constant
    public float springConstant = 10f;
 
    // friction coefficient bumper (laminare viskose Dämpfung FR=frictionCoefficient * v)
    private float frictionCoefficient = 0f;

    private bool hasCollided = false;
 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // get the rigid body of the car
        rb = GetComponent<Rigidbody>();
 
        // get the rigid body of the bumpers
        leftBumper = GameObject.Find("Bumper left").GetComponent<Rigidbody>();
        rightBumper = GameObject.Find("Bumper right").GetComponent<Rigidbody>();
 
        // get the Exporter script
        exporter = GetComponent<Exporter>();
        Assert.IsNotNull(exporter, "Exporter script not found");
 
 
        // confine motion of the to 1D along z-axis
        rb.constraints = RigidbodyConstraints.FreezePositionX |
        RigidbodyConstraints.FreezePositionY |
        RigidbodyConstraints.FreezeRotationX |
        //RigidbodyConstraints.FreezeRotationY |
        RigidbodyConstraints.FreezeRotationZ;
 
        // set motion of the bumpers
        // Your code here ...
        switch (bumperMode)
        {
            case 0:
                leftBumper.constraints = RigidbodyConstraints.FreezeAll;
                break;
            case 1:
                leftBumper.isKinematic = false;
                leftBumper.constraints = RigidbodyConstraints.FreezeAll;
                leftBumper.constraints &= ~RigidbodyConstraints.FreezePositionZ; // allow movement along z-axis
                break;
            case 2:
                rb.isKinematic = false;
                leftBumper.isKinematic = false;
                break;
            default:
                Assert.Fail("Invalid bumper mode");
                break;
        }
 
        // === solver settings ===
 
        // Controls how often physics updates occur (default: 0.02s or 50 Hz)
        Time.fixedDeltaTime = 0.02f;
 
        // Determines how many times Unity refines the constraint solving per physics step(default: 6)
        Physics.defaultSolverIterations = 6;
 
        // Similar to above but specifically for velocity constraints (default: 1)
        Physics.defaultSolverVelocityIterations = 1;
    }
 
 
 
    // Update is called once per frame
    void Update()
    {
        // launch car
        if (Keyboard.current[Key.Space].wasPressedThisFrame || (recording && !isLaunched))
        {
            // remember that car was launched
            isLaunched = true;
 
            // remember the current time
            launchTime = Time.time;
 
            // Set the initial velocity of the car
            rb.linearVelocity = new Vector3(0f, 0f, initialVelocity);
 
            // log
            Debug.Log("Launching the car");
        }
 
 
        // reload scene
        if (Keyboard.current[Key.R].wasPressedThisFrame)
        {
            // Reload the scene
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
 
 
    private void FixedUpdate()
    {
        // Your code here ...
        const float collisionTolerance = 1e-5f;
        // right
        var distRight= Math.Abs(rightBumper.position.z - rb.position.z) - (carWidth * 0.5f + bumperWidth * 0.5f);
        if (distRight<= collisionTolerance)
        {
            rb.linearVelocity = new Vector3(0f, 0f, -initialVelocity);
        }
        
        // left
        var distLeft= Math.Abs(leftBumper.position.z - rb.position.z) - (carWidth * 0.5f + bumperWidth * 0.5f);
        if (distLeft<= collisionTolerance && !hasCollided)
        {
            var fj = rb.gameObject.AddComponent<FixedJoint>();
            fj.connectedBody = leftBumper;
            hasCollided = true; 
        }
 
 
        if (isLaunched)
        {
            // TimeSeriesData timeSeriesData = new(
            //     rb,
            //     rightBumper, // Pass the rightBumper Rigidbody
            //     Time.time - launchTime,
            //     compressionLeft,
            //     forceLeft,
            //     compressionRight,
            //     forceRight,
            //     rb.angularVelocity.y,
            //     rightBumper.angularVelocity.y,
            //     jointCreatedRightFlag // Pass the flag
            // );
            //exporter.AddData(timeSeriesData);
        }
    }
 
    void OnGUI()
    {
        GUIStyle textStyle = new()
        {
            fontSize = 20,
            normal = { textColor = Color.black }
        };
 
        // keyboard shortcuts
        GUI.Label(new Rect(10, Screen.height - 20, 400, 20),
            "R ... Reload", textStyle);
    }
}