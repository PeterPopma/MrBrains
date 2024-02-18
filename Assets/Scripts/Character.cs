using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    const int DISTANCE_BETWEEN_DIRECTION_CHANGES = 8;
    Animator animator;
    private CharacterController characterController;
    public float moveSpeed;
    private int animIDSpeed;
    private int currentDirectionChange;
    private int numberOfDirectionChanges;
    private float distanceSinceLastDirectionChange;
    private float totalDistanceTravelled;
    private float timeLeftShowDirectionChange;
    private float timeLeftCheer;
    private List<double> directionChanges = new List<double>();
    private Light light;
    private bool reachedTarget;
    private bool isFinished;

    public int NumberOfDirectionChanges { get => numberOfDirectionChanges; set => numberOfDirectionChanges = value; }
    public List<double> DirectionChanges { get => directionChanges; set => directionChanges = value; }
    public int CurrentRotation { get => currentDirectionChange; set => currentDirectionChange = value; }

    // Start is called before the first frame update
    void Start()
    {
        light = GetComponent<Light>(); 
        light.enabled = false;
        distanceSinceLastDirectionChange = totalDistanceTravelled = 0;
        animIDSpeed = Animator.StringToHash("Speed");
        animator = GetComponent<Animator>();
        animator.SetFloat(animIDSpeed, 4); 
        characterController = GetComponent<CharacterController>();
    }

    public void MakeMovementPlan(List<double> directions)
    {
        directionChanges = directions;
        SetNewDirection();
    }

    public void OnFootstep()
    {

    }

    public void ReachedTarget()
    {
        if (!reachedTarget)
        {
            CharacterSpawner.Instance.AddPersonWalkResult(totalDistanceTravelled, currentDirectionChange);
            animator.SetFloat(animIDSpeed, 0);
            animator.SetLayerWeight(2, 1);
            timeLeftCheer = 1;
            Destroy(characterController);
        }
        reachedTarget = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (timeLeftCheer > 0)
        {
            timeLeftCheer -= Time.deltaTime;
            if(timeLeftCheer < 0)
            {
                Destroy(gameObject);
            }
        }

        if (timeLeftShowDirectionChange>0)
        {
            timeLeftShowDirectionChange -= Time.deltaTime;
            if( timeLeftShowDirectionChange < 0)
            {
                light.enabled = false;
            }
        }

        if (transform.position.y < -.1)
        {
            if (!isFinished)
            {
                isFinished = true;
                CharacterSpawner.Instance.AddPersonWalkResult(totalDistanceTravelled, currentDirectionChange);
                animator.SetLayerWeight(1, 1);
            }
            characterController.Move(new Vector3(0.0f, -14f * Time.deltaTime, 0.0f));
            transform.Rotate(new Vector3(200*Time.deltaTime,0,0));
            if (transform.position.y < -50)
            {
                Destroy(gameObject);
            }
        }
        else
        { 
            if (!reachedTarget)
            {
                Move();
            }

            if (distanceSinceLastDirectionChange > DISTANCE_BETWEEN_DIRECTION_CHANGES)
            {
                SetNewDirection();
                numberOfDirectionChanges++;
                light.enabled = true;
                timeLeftShowDirectionChange = 0.2f;
            }
        }
    }

    public void SetNewDirection()
    {
        distanceSinceLastDirectionChange = 0;
        transform.Rotate(0, (float)directionChanges[currentDirectionChange], 0);
        currentDirectionChange++;
        if (currentDirectionChange >= directionChanges.Count)
        { 
            currentDirectionChange = 0;
            Debug.Log("Rotation list is not long enough!");
        }
    }

    private void Move()
    {
        if (!characterController.isGrounded)
        {
            // simulate gravity
            characterController.Move(new Vector3(0.0f, -2f * Time.deltaTime, 0.0f));
        }
        distanceSinceLastDirectionChange += Time.deltaTime * moveSpeed;
        totalDistanceTravelled += Time.deltaTime * moveSpeed;
        characterController.Move(transform.forward * Time.deltaTime * moveSpeed);
    }
}
