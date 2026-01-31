using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class MainGateLogic : MonoBehaviour
{

    private bool hasShownCongo = false;

    [Header("Congrac")]
    public GameObject CongoUI;
    public GameObject PlayerCamera; // Drag object with FirstPersonLook
    public GameObject move;         // Drag object with FirstPersonMovement
    public TMP_Text eqtext;   // Another input field to copy from

    [Header("Text")]
    public TMP_InputField playerNameInput;

    public TMP_Text textObj1;
    public TMP_Text textObj2;
    public TMP_Text textObj3;
    public TMP_Text textObj4;

    [Header("Animation")]
    public GameObject npcUI;
    public GameObject interactUI;
    public float focusDistance = 5f;
    public Transform player;
    public static bool isNpcUiActive = false;
    public Animator gateAnimator;

    private FirstPersonMovement playerMovement;
    private FirstPersonLook playerLook;

    [Header("UI Elements")]
    public GameObject StartingPointUIElement;
    public Button gateButton;

    [Header("Point Bar")]
    public GameObject pointBar;

    public static string savedPlayerName = "";

    void Start()
    {
        CongoUI.SetActive(false);
        pointBar.SetActive(false);

        if (StartingPointUIElement != null)
            StartingPointUIElement.SetActive(true);

        if (interactUI != null)
            interactUI.SetActive(false);

        if (player != null)
        {
            playerMovement = player.GetComponent<FirstPersonMovement>();
            playerLook = player.GetComponentInChildren<FirstPersonLook>();
        }

        if (playerNameInput != null)
        {
            playerNameInput.onValueChanged.AddListener(delegate { CheckInputField(); });
            CheckInputField();
        }
    }

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        CheckTextValues();

        if (Physics.Raycast(ray, out hit, focusDistance))
        {
            if (hit.transform.gameObject == gameObject)
            {
                if (interactUI != null)
                    interactUI.SetActive(true);

                if (Input.GetMouseButtonDown(0))
                {
                    if (StartingPointUIElement != null)
                        StartingPointUIElement.SetActive(false);

                    OpenNpcUI();
                    
                }
            }
            else if (interactUI != null)
                interactUI.SetActive(false);
        }
        else if (interactUI != null)
            interactUI.SetActive(false);
    }

    public void Exit()
    {
        SceneManager.LoadScene("Starting Menu");
    }

    public void Resume()
    {
        CongoUI.SetActive(false);
        LockCursor();

        // ✅ Re-enable movement
        if (move != null)
        {
            FirstPersonMovement movement = move.GetComponent<FirstPersonMovement>();
            if (movement != null) movement.enabled = true;
        }

        // ✅ Re-enable camera look
        if (PlayerCamera != null)
        {
            FirstPersonLook look = PlayerCamera.GetComponent<FirstPersonLook>();
            if (look != null) look.enabled = true;
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

    public void OpenGateAnimation()
    {
        if (playerNameInput != null && !string.IsNullOrWhiteSpace(playerNameInput.text))
        {
            savedPlayerName = playerNameInput.text;
            pointBar.SetActive(true);


            if (npcUI != null)
            {
                npcUI.SetActive(false);
                isNpcUiActive = false;
                LockCursor();
            }

            if (gateAnimator != null)
            {
                gateAnimator.SetTrigger("OpenGate");
                CheckTextValues();
            }
                


            eqtext.text = playerNameInput.text; // ✅ Correct direction
        }
        else
        {
            Debug.Log("Please enter your name before opening the gate.");
        }
    }

    void CheckInputField()
    {
        if (gateButton != null)
            gateButton.interactable = !string.IsNullOrWhiteSpace(playerNameInput.text);
    }

    void CheckTextValues()
    {
        if (GetValue(textObj1) >= 6 &&
            GetValue(textObj2) >= 8 &&
            GetValue(textObj3) >= 4 &&
            GetValue(textObj4) >= 6)
        {
            if (!hasShownCongo) // ✅ Show only once
            {
                CongoUI.SetActive(true);
                UnlockCursor();
                hasShownCongo = true; // ✅ Mark as shown

                if (move != null)
                {
                    FirstPersonMovement movement = move.GetComponent<FirstPersonMovement>();
                    if (movement != null) movement.enabled = false;
                }

                if (PlayerCamera != null)
                {
                    FirstPersonLook look = PlayerCamera.GetComponent<FirstPersonLook>();
                    if (look != null) look.enabled = false;
                }
            }
        }
        else
        {
            //Debug.Log("❌ One or more values are 0 or less.");
        }
    }


    float GetValue(TMP_Text textObj)
    {
        if (textObj == null) return 0;
        float value;
        float.TryParse(textObj.text, out value);
        return value;
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
