
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiceScript : MonoBehaviour
{
    static Rigidbody rb;
    public static Vector3 diceVelocity;

    [Header("Manual Settings")]
    public Vector3 initialRotation = Vector3.zero;
    public float forceMultiplier = 300f;
    public Vector3 torqueMultiplier = new Vector3(100, 100, 100);

    public GameObject focusObject; // Object to focus on
    public GameObject uiElement; // UI element to show when looking at the object

    private Vector3 lastPosition;
    private bool canRoll = true;
    private int rollCount = 0;
    private int totalSum = 0;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        lastPosition = transform.position;

        if (uiElement != null)
            uiElement.SetActive(false); // Hide UI at start
    }

    void Update()
    {
        diceVelocity = rb.velocity;

        bool isLooking = IsLookingAtObject();
        if (uiElement != null)
            uiElement.SetActive(isLooking); // Enable/Disable UI based on focus

        if (canRoll && isLooking && Input.GetMouseButtonDown(0))
        {
            StartCoroutine(RollDice());
        }
    }

    bool IsLookingAtObject()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            return hit.collider.gameObject == focusObject;
        }
        return false;
    }

    IEnumerator RollDice()
    {
        canRoll = false;
        DiceNumberTextScript.diceNumber = 0;
        transform.position = lastPosition;
        transform.localRotation = Quaternion.Euler(initialRotation);
        rb.isKinematic = false;
        rb.AddForce(transform.up * forceMultiplier);
        rb.AddTorque(
            Random.Range(0, torqueMultiplier.x),
            Random.Range(0, torqueMultiplier.y),
            Random.Range(0, torqueMultiplier.z)
        );

        yield return new WaitForSeconds(3f);

        int rolledNumber = DiceNumberTextScript.diceNumber ?? 0;
        totalSum += rolledNumber;
        rollCount++;

        float delay = rolledNumber * 1.5f;
        Debug.Log("Waiting " + delay + " seconds before next roll");
        yield return new WaitForSeconds(delay);

        if (rollCount == 3)
        {
            if (totalSum >= 12)
            {
                Debug.Log("Output is greater than or equal to 12!");
            }
            Debug.Log("Reset");
            rollCount = 0;
            totalSum = 0;
        }

        rb.isKinematic = true;
        canRoll = true;
    }
}
