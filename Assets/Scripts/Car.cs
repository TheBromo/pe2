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

    private Vector3 oldCenterOfMass = Vector3.zero; 
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
            
            // Find center of mass
            Vector3 centerOfMass = rb.mass*rb.position + leftBumper.mass*leftBumper.position;
            centerOfMass /= rb.mass + leftBumper.mass;

            var velocity = (oldCenterOfMass - centerOfMass)/Time.fixedDeltaTime;
            var translationEnergy =(rb.mass + leftBumper.mass) * Vector3.Dot(velocity, velocity)/2; 
            
            // Calculate rotational energy for both bodies
            float rotationEnergy = 0f;

            // Rotational energy of first body (rb)
            Vector3 localAngularVel1 = Quaternion.Inverse(rb.inertiaTensorRotation) * rb.angularVelocity;
            Vector3 angularMomentum1 = Vector3.Scale(rb.inertiaTensor, localAngularVel1);
            rotationEnergy += 0.5f * Vector3.Dot(localAngularVel1, angularMomentum1);

            // Rotational energy of second body (leftBumper)
            Vector3 localAngularVel2 = Quaternion.Inverse(leftBumper.inertiaTensorRotation) * leftBumper.angularVelocity;
            Vector3 angularMomentum2 = Vector3.Scale(leftBumper.inertiaTensor, localAngularVel2);
            rotationEnergy += 0.5f * Vector3.Dot(localAngularVel2, angularMomentum2);
            
            float totalEnergy = translationEnergy + rotationEnergy;
            
            // Distance from each body's COM to total COM
            Vector3 d1 = rb.position - centerOfMass;
            Vector3 d2 = leftBumper.position - centerOfMass;

            // Convert inertiaTensor to world axes if needed
            Vector3 I1_local = rb.inertiaTensor; // In principal axes (body local)
            Vector3 I2_local = leftBumper.inertiaTensor;

            // Parallel axis theorem for z-axis rotation:
            float I1_parallel = I1_local.z + rb.mass * d1.sqrMagnitude; // d1.sqrMagnitude is |d|^2
            float I2_parallel = I2_local.z + leftBumper.mass * d2.sqrMagnitude;
            // i_ges = I_local + m*d+^2
            float I_total = I1_parallel + I2_parallel;
            
            
            // Assume cart mass = m, position = pos, velocity = vel, centerOfMass = com
            Vector3 r = rb.position - centerOfMass;
            Vector3 p = rb.mass * rb.linearVelocity;
            Vector3 L = Vector3.Cross(r, p);

            var angularVelocity = rb.angularVelocity; 
            
            oldCenterOfMass = centerOfMass;
             TimeSeriesData timeSeriesData = new(
                 rb,
                 Time.time - launchTime,
                 rotationEnergy,
                 translationEnergy,
                 I_total,
                 L.magnitude,// @Pascal maybe you need to change this :^)
                  leftBumper.position.z,
                 leftBumper.linearVelocity.y
             );
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