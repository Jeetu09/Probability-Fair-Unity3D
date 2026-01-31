using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class FiinalGateNpc : MonoBehaviour
{
    [Header("Loading Animation")]
    public Animator loadingAnimator;  // Animator that controls slide animations

    public GameObject npcUI;
    public GameObject interactUI;
    public float focusDistance = 5f;

    [Header("Player References")]
    public GameObject playerModel;
    public Vector3 newPlayerPosition;

    public static bool isNpcUiActive = false;

    [Header("UI Elements")]
    public GameObject startingPointUI;

    [Header("Gate Unlock Conditions")]
    public GameObject textObj1;
    public GameObject textObj2;
    public GameObject textObj3;
    public GameObject textObj4;

    void Start()
    {
        if (startingPointUI != null)
            startingPointUI.SetActive(true);

        if (interactUI != null)
            interactUI.SetActive(false);
    }

    
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, focusDistance))
        {
            if (hit.transform.gameObject == gameObject)
            {
                if (!interactUI.activeSelf)
                    interactUI.SetActive(true);

                if (Input.GetMouseButtonDown(0))
                {
                    if (startingPointUI != null)
                        startingPointUI.SetActive(false);

                    OpenNpcUI();
                }
            }
            else if (interactUI.activeSelf)
            {
                interactUI.SetActive(false);
            }
        }
        else if (interactUI.activeSelf)
        {
            interactUI.SetActive(false);
        }
    }

    public void OpenNpcUI()
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

    public void FinalGate()
    {
        if (!AreConditionsMet())
        {
            Debug.Log("Incompleted");
            return;
        }

        if (npcUI != null)
            npcUI.SetActive(false);

        LockCursor();

        // Trigger slide-up animation
        if (loadingAnimator != null)
            loadingAnimator.SetTrigger("SlideUp");

        // Load next scene after 4-second delay
        StartCoroutine(LoadFinalSceneAfterDelay(4f));
    }


    IEnumerator LoadFinalSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("FinalMaze");
        isNpcUiActive = false;
    }

    private bool AreConditionsMet()
    {
        if (textObj1 == null || textObj2 == null || textObj3 == null || textObj4 == null)
        {
            Debug.LogWarning("Please assign all text UI objects in the Inspector.");
            return false;
        }

        int num1 = int.Parse(textObj1.GetComponent<TMP_Text>().text);
        int num2 = int.Parse(textObj2.GetComponent<TMP_Text>().text);
        int num3 = int.Parse(textObj3.GetComponent<TMP_Text>().text);
        int num4 = int.Parse(textObj4.GetComponent<TMP_Text>().text);

        bool allNonNegative = (num1 >= 6 && num2 >= 8 && num3 >= 4 && num4 >= 6);

        if (!allNonNegative)
            Debug.Log("Incompleted");

        return allNonNegative;
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
