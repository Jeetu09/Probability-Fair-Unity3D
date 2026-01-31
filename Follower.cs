using UnityEngine;
using PathCreation;
using TMPro;

public class Follower : MonoBehaviour
{
    [Header("ANimation")]
    public AudioSource bgaudio3;
    public AudioSource Wrong;
    public PathCreator pathCreator;
    public float speed = 5f;
    private float distanceTravelled;
    private Quaternion rotationOffset;

    public GameObject[] collisionObjects;
    private int currentIndex = -1;
    private int lastDiceNumber = 0;
    private int rollCount = 0;
    private int totalSum = 0;

    public Animator RacingCarPoints;
    public TextMeshProUGUI RCCarPointUI;
    private Rigidbody rb;

    private int points = 0;

    void Start()
    {
        bgaudio3.Pause();
        Wrong.Pause();
        rotationOffset = Quaternion.Inverse(pathCreator.path.GetRotationAtDistance(0)) * transform.rotation;
        rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }
    }

    void Update()
    {
        if (DiceNumberTextScript.diceNumber.HasValue && lastDiceNumber == 0 && DiceNumberTextScript.diceNumber.Value != 0)
        {
            if (rb != null)
            {
                rb.constraints = RigidbodyConstraints.None;
            }

            rollCount++;
            totalSum += DiceNumberTextScript.diceNumber.Value;
            if (totalSum > 12)
            {
                totalSum = 12; // Cap totalSum at 12
            }
            Debug.Log("Rolled: " + DiceNumberTextScript.diceNumber.Value);
            Debug.Log("Total Sum: " + totalSum);
        }

        lastDiceNumber = DiceNumberTextScript.diceNumber ?? 0;

        if (rb == null || rb.constraints != RigidbodyConstraints.FreezeAll)
        {
            distanceTravelled += speed * Time.deltaTime;
            transform.position = pathCreator.path.GetPointAtDistance(distanceTravelled);
            transform.rotation = pathCreator.path.GetRotationAtDistance(distanceTravelled) * rotationOffset;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collided with: " + other.gameObject.name);

        if (int.TryParse(other.gameObject.name, out int objectNumber) && objectNumber == totalSum)
        {
            Debug.Log("The number is matched");
            if (rb != null)
            {
                rb.constraints = RigidbodyConstraints.FreezeAll;
            }

            if (totalSum >= 12)
            {
                Debug.Log("You did it");

                // Add 2 points to the UI element
                points += 1;
                bgaudio3.Play();

                if (RCCarPointUI != null)
                {
                    RCCarPointUI.text = "" + points;
                }

                ResetPosition();
                RacingCarPoints.SetTrigger("RacingPointTrigger");
            }
            else if (rollCount == 3)
            {
                Debug.Log("Game finished but sum is less than 12, resetting position");
                Wrong.Play();
                ResetPosition();
            }
        }
    }

    private void ResetPosition()
    {
        transform.position = pathCreator.path.GetPointAtDistance(0);
        distanceTravelled = 0;
        rollCount = 0;
        totalSum = 0;
        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }
    }
}
