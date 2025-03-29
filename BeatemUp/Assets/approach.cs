using UnityEngine;

public class approach : MonoBehaviour
{
    public Transform target;
    public float speed = 100f;
    public Rigidbody rb;
    private bool isFollowing = false;

    void Start()
    {
        target = GameObject.FindWithTag("Player").transform;
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (isFollowing && target != null)
        {
            FollowPlayer();
        }
    }
    void FollowPlayer()
    {
<<<<<<< Updated upstream
        float stoppingDistance = 1.25f;
=======
        float stoppingDistance = 1.5f;
>>>>>>> Stashed changes
        if (Vector3.Distance(transform.position, target.position) > stoppingDistance)
        {
            Vector3 pos = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
            rb.MovePosition(pos);
            transform.LookAt(target);
<<<<<<< Updated upstream
=======
            SeparateFromOthers();
>>>>>>> Stashed changes
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isFollowing = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isFollowing = false;
        }
    }

    void SeparateFromOthers()
    {
        float detectionRadius = 0.5f; // Small radius for close avoidance
        float separationStrength = 0.02f; // Smaller push amount

        Collider[] nearbyEnemies = Physics.OverlapSphere(transform.position, detectionRadius);

        foreach (Collider enemy in nearbyEnemies)
        {
            if (enemy.gameObject != this.gameObject && enemy.CompareTag("Enemy"))
            {
                Vector3 directionAway = transform.position - enemy.transform.position;
                float distance = directionAway.magnitude;

                if (distance < 1.5f) // Only separate if TOO close (adjustable)
                {
                    transform.position += directionAway.normalized * separationStrength;
                }
            }
        }
    }
}
