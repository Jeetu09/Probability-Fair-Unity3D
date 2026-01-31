using System.Collections;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.UI;

public class ShopeOneNpc : MonoBehaviour
{
    public GameObject npcUI; // NPC UI panel
    public GameObject interactUI; // UI prompt ("Press to interact")
    public float focusDistance = 5f; // Maximum focus distance
    public Transform player; // Player reference
    public static bool isNpcUiActive = false;

    private FirstPersonMovement playerMovement;
    private FirstPersonLook playerLook;

    [Header("UI Elements")]
    public GameObject StartingPointUIElement;
    void Start()
    {
        StartingPointUIElement.SetActive(true);
        if (interactUI != null)
            interactUI.SetActive(false); // Hide UI initially

        playerMovement = player.GetComponent<FirstPersonMovement>();
        playerLook = player.GetComponentInChildren<FirstPersonLook>();
    }

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, focusDistance))
        {
            if (hit.transform.gameObject == gameObject) // If looking at NPC
            {
                interactUI.SetActive(true); // Show interact UI

                if (Input.GetMouseButtonDown(0)) // If clicked
                {
                    StartingPointUIElement.SetActive(false);
                    OpenNpcUI();
                }
            }
            else
            {
                interactUI.SetActive(false); // Hide UI if not looking
            }
        }
        else
        {
            interactUI.SetActive(false); // Hide UI if no object in focus
        }
    }

    void OpenNpcUI()
    {
        if (npcUI != null)
        {
            npcUI.SetActive(true);
            isNpcUiActive = true;
            UnlockCursor();
        }
    }

    public void CloseNpcUI()
    {
        if (npcUI != null)
        {
            npcUI.SetActive(false);
            isNpcUiActive = false;
            LockCursor();
        }
    }

    private void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
