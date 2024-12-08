using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float rotationSpeed = 15f;
    [SerializeField] private float clickMoveThreshold = 1f;

    private NavMeshAgent agent;
    private Camera mainCamera;
    private Animator animator;
    private Vector3 movement;
    private bool isClickMoving;

    private void Start()
    {
        Debug.Log("PlayerController Start called");
        agent = GetComponent<NavMeshAgent>();
        mainCamera = Camera.main;
        animator = GetComponent<Animator>();
        
        if (agent == null)
            Debug.LogError("NavMeshAgent not found on Player!");
        if (mainCamera == null)
            Debug.LogError("Main Camera not found! Make sure it's tagged as MainCamera");

        agent.speed = moveSpeed;
        agent.angularSpeed = rotationSpeed;
    }

    private void Update()
    {
        HandleInput();
        UpdateAnimation();
    }

    private void HandleInput()
    {
        // WASD Movement
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        movement = new Vector3(horizontal, 0f, vertical).normalized;

        if (movement.magnitude > 0.1f)
        {
            Debug.Log($"Moving with input: {movement}");
            // Cancel click-to-move when using WASD
            isClickMoving = false;
            agent.ResetPath();
            
            // Move the character
            Vector3 moveDirection = Quaternion.Euler(0f, mainCamera.transform.eulerAngles.y, 0f) * movement;
            transform.position += moveDirection * moveSpeed * Time.deltaTime;
            
            // Rotate the character
            if (moveDirection != Vector3.zero)
            {
                Quaternion toRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
                transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
            }
        }

        // Click-to-move
        if (Input.GetMouseButtonDown(1)) // Right click
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Debug.Log($"Click-moving to position: {hit.point}");
                agent.SetDestination(hit.point);
                isClickMoving = true;
            }
        }

        // Check if we've reached click-to-move destination
        if (isClickMoving && !agent.pathPending)
        {
            if (agent.remainingDistance <= agent.stoppingDistance + clickMoveThreshold)
            {
                isClickMoving = false;
                agent.ResetPath();
            }
        }
    }

    private void UpdateAnimation()
    {
        if (animator != null)
        {
            float speed = isClickMoving ? agent.velocity.magnitude : movement.magnitude;
            animator.SetFloat("Speed", speed);
        }
    }
}
