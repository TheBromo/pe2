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
    public int bumperMode = 1;

    // helper flag to automatically start the car when recording starts
    // Window > General > Recorder > Start Recording
    private readonly bool recording = true;




    // initial velocity of the car
    public float initialVelocity = 1.0f;

    // the length of the uncompressed spring
    public float springLength = 15f;

    // spring constant
    public float springConstant = 10f;

    // friction coefficient bumper (laminare viskose D�mpfung FR=frictionCoefficient * v)
    public float frictionCoefficient = 0f;


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
        rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;

        // set motion of the bumpers
        // Your code here ...
        switch (bumperMode)
        {
            case 0:
                // fix bumper in place
                leftBumper.constraints = RigidbodyConstraints.FreezeAll;
                rightBumper.constraints = RigidbodyConstraints.FreezeAll;
                break;
            case 1:
            case 2:
                // allow bumper to move along z-axis
                leftBumper.isKinematic = false;
                leftBumper.constraints = RigidbodyConstraints.FreezePositionX
                | RigidbodyConstraints.FreezePositionY
                | RigidbodyConstraints.FreezeRotationX
                | RigidbodyConstraints.FreezeRotationY
                | RigidbodyConstraints.FreezeRotationZ;


                // constrain motion of the bumper to 1D along z-axis
                leftBumper.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
                break;
            default:
                Assert.Fail("Invalid bumper mode");
                break;
        }


        // Note: switch off collider in the inspector, because depending on the spring paramters, it can happen that the car touches the bumpers
        // and then the movement looks very strange and debugging is difficult (try increasing initial velocity). Without collider one sees
        // that the car penetrates the bumpers and debugging becomes easier.


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

            // Your code here ...
            rb.linearVelocity = new Vector3(0, 0, 1);


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

        // === left spring
        // float distanceToLeftBumper = Mathf.Abs(rb.position.z - leftBumper.position.z) - (carWidth / 2 + bumperWidth / 2);
        // float compressionLeft = Mathf.Max(0f, springLength - distanceToLeftBumper);
        // float springForceLeft = springConstant * compressionLeft;
        // rb.AddForce(new Vector3(0, 0, springForceLeft), ForceMode.Force);

        // === right spring
        float compressionRight = 0,springForceRight=0;
        // --- Aufgabe 1 uncomment to make this and comment task 2 out, to the right side spring 
        // float distanceToRightBumper = Mathf.Abs(rb.position.z - rightBumper.position.z) - (carWidth / 2 + bumperWidth / 2);
        // float compressionRight = Mathf.Max(0f, springLength - distanceToRightBumper);
        // float springForceRight = springConstant * compressionRight;
        // rb.AddForce(new Vector3(0, 0, -springForceRight), ForceMode.Force);

        // --- Aufgabe 2    
        float overlapToRightBumper = (carWidth / 2 + bumperWidth / 2) - Mathf.Abs(rb.position.z - rightBumper.position.z);
        if (overlapToRightBumper > 0f)
        {
            rb.linearVelocity = new Vector3(0, 0, -rb.linearVelocity.z);
        }

        float overlapToLeftBumper = (carWidth / 2 + bumperWidth / 2) - Mathf.Abs(rb.position.z - leftBumper.position.z);
        if (overlapToLeftBumper > 0f)
        {
            //connect left bumper to car with fixed joint
            FixedJoint fixedJoint = leftBumper.gameObject.AddComponent<FixedJoint>();
            fixedJoint.connectedBody = rb;
        }

        if (bumperMode == 1 || bumperMode == 2)
        {
            // -- Aufgabe 3
            // leftBumper.AddForce(Vector3.back * springForceLeft, ForceMode.Force);
            if (bumperMode == 2)
            {
                // -- Aufgabe 4 
                float forceFriction = frictionCoefficient * leftBumper.linearVelocity.z;
                leftBumper.AddForce(Vector3.back * forceFriction, ForceMode.Force);
            }
        }


        // === time series data

        // store time series record
        if (isLaunched)
        {
            // Your code here ... (adapt as needed)
            TimeSeriesData timeSeriesData = new(
                rb,
                Time.time - launchTime,
                0,
                0,
                compressionRight,
                springForceRight,
                leftBumper.position.z,
                leftBumper.linearVelocity.z);

            exporter.AddData(timeSeriesData);
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
