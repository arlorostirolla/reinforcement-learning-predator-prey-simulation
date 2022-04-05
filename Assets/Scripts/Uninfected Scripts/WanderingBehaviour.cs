using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WanderingBehaviour : MonoBehaviour
{
    // DEBUG test velocity to see velocity of gameObject
    public Vector2 currentVelocity;

    // public float maxVelocity;
    // public float minVelocity;

    // DEBUG test object that shows the mid point between a group of boids
    public GameObject testObject;

    // Detection radius to detect nearby boids
    public float detectionRadius = 3f;
    // Average starting velocity for the boid on instantiate/spawn
    public float startVelocitySpeedAvg = 2f;
    // Radius to start seperating boids
    public float seperationRadius = 1f;

    // Enable/Disable Boids functions
    public bool cohesionEnabled = true;
    public bool seperationEnabled = true;
    public bool alignmentEnabled = true;



    // Interval between code checks
    private int interval = 3;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // Instantiate early movement within the boid
        rb.velocity = RandomEarlyMovement(-startVelocitySpeedAvg, startVelocitySpeedAvg);
    }

    // Function that Instantiates early movement, until entering a boid group and inheriting velocity from them.
    private Vector2 RandomEarlyMovement(float min, float max)
    {
        float x = Random.Range(min, max);
        float y = Random.Range(min, max);
        return new Vector2(x, y);
    }

    // Hit Collider check, searching for nearby 2D Colliders that are both Uninfected Civillians and not themself.
    void nearbyCivillians(Vector2 centre, float radius)
    {
        // Get all nearby colliders, includes itself... for some reason
        Collider2D[] hitcolliders = Physics2D.OverlapCircleAll(centre, radius, 1);

        // Instantiate checking how many nearby colliders/uninfected to go through the respective loops to develop the array of colliders that ARE boids.
        int nearbyColliders = 0;
        int nearbyUninfected = 0;
        if(hitcolliders.Length > 1)
        {
            // Instantiate the array of rigidbody colliders.
            Rigidbody2D[] otherrb2D = new Rigidbody2D[hitcolliders.Length];
            while (nearbyColliders < hitcolliders.Length)
            {
                // Checks for nearby colliders that are uninfectedCivillians as well as not the current gameObject
                if (hitcolliders[nearbyColliders].gameObject.tag == "uninfectedCivillian" && hitcolliders[nearbyColliders].gameObject != this.gameObject)
                {
                    // Puts the rigidbody into the new array.
                    otherrb2D[nearbyUninfected] = hitcolliders[nearbyColliders].GetComponent<Rigidbody2D>();
                    nearbyUninfected++;
                }                    
                nearbyColliders++;
            }

            // Function checks to run the boids functions seperately if required.
            if (cohesionEnabled == true && nearbyUninfected > 0)
            {
                cohesion(otherrb2D);
            }
            if (seperationEnabled == true && nearbyUninfected > 0)
            {
                seperation(otherrb2D, seperationRadius);
            }
            if (alignmentEnabled == true && nearbyUninfected > 0)
            {
                alignment(otherrb2D);
            }
        }
        
    }

    void cohesion(Rigidbody2D[] nearbyUninfectedRB)
    {
        // First vector2 is the current transform of the object
        Vector2 avgTransform = this.transform.position;

        // Starts at 1 since there is itself
        int transformAmnt = 1;

        // Goes through the array parsed in to get the average value of the transforms.
        for (int i = 0; i < nearbyUninfectedRB.Length; i++)
        {
            if (nearbyUninfectedRB[i] != null)
            {
                avgTransform.x += nearbyUninfectedRB[i].transform.position.x;
                avgTransform.y += nearbyUninfectedRB[i].transform.position.y;
                transformAmnt++;
            }
        }

        // Divides the Average transforms by the transform amount to get an average/centre point between the group of boids nearby
        avgTransform /= transformAmnt;

        // Then we calculate the central direction by the classic direction calculation of Vector A - Vector B and then normalizing to get a direction.
        Vector2 centreDirection = avgTransform - (Vector2)this.transform.position;
        testObject.transform.position = avgTransform; // DEBUG
        // print(centreDirection.normalized); // DEBUG

        // Basic addforce based on the direction given, would like to improve
        rb.AddForce(centreDirection.normalized);

    }

    void seperation(Rigidbody2D[] nearbyUninfectedRB, float seperationRadius)
    {
        // Iterates through the array to calculate the distance between two units within the boids
        // and if the distance < seperationRadius then an opposite force is pushed against the boid to facilitate with seperation
        for(int i = 0; i < nearbyUninfectedRB.Length; i++)
        {
            if(nearbyUninfectedRB[i] != null)
            {
                Vector2 movingtowards = rb.position - nearbyUninfectedRB[i].position;
                float distance = movingtowards.magnitude;
                Vector2 direction = movingtowards / distance;
                if (distance < seperationRadius)
                {
                    rb.AddForce(direction);
                }
            }
        }
    }

    void alignment(Rigidbody2D[] nearbyUninfectedRB)
    {
        // Similar code to Cohesion, but gets the average of the velocitys to get an average direction vector
        Vector2 avgVelocity = rb.velocity;

        int velocityAmnt = 1;

        for(int i = 0; i < nearbyUninfectedRB.Length; i++)
        {
            if (nearbyUninfectedRB[i] != null)
            {
                avgVelocity += nearbyUninfectedRB[i].velocity;
                velocityAmnt++;
            }
        }
        avgVelocity /= velocityAmnt;

        // Would like to introduce rng or maybe have alignment be a bit weaker? Snaps into place quite quickly.
        rb.velocity = (rb.velocity + avgVelocity) / 2;

    }

    void Update()
    {
        // Efficiency Function, I don't think it's neccessary to call nearby colliders 60/144 times a second.
        if (Time.frameCount % interval == 0)
        {
            nearbyCivillians(this.transform.position, detectionRadius);
        }

        // Potential velocity clamping/ possibly delving with velocity rng? DEBUG
        /*
        float velocityX = Mathf.Clamp(rb.velocity.x, minVelocity, maxVelocity);
        float velocityY = Mathf.Clamp(rb.velocity.y, minVelocity, maxVelocity);

        rb.velocity = new Vector2(velocityX, velocityY);
        */


        currentVelocity = rb.velocity; // DEBUG

    }
}
