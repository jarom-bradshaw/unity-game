using UnityEngine;

public class approach : MonoBehaviour
{
    public Transform target;
    public float speed = 2f;
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
        float stoppingDistance = 1.25f;
        if (Vector3.Distance(transform.position, target.position) > stoppingDistance)
        {
            Vector3 pos = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
            rb.MovePosition(pos);
            transform.LookAt(target);
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
}
