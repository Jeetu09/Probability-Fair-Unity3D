using UnityEngine;

public class ShopFourNpc : MonoBehaviour
{
    public GameObject GunMesh, GunModel, GunHolder, npcUI, interactUI, playerModel;
    public Vector3 newPlayerPosition;
    public float focusDistance = 5f;
    public GunShooting gunShootingScript;
    public MonoBehaviour[] playerMovementScripts;

    private MeshRenderer gunRenderer, gunModelRenderer;
    public static bool isNpcUiActive = false, isGateOpening = false;

    void Start()
    {
        gunRenderer = GunMesh.GetComponent<MeshRenderer>();
        gunModelRenderer = GunModel.GetComponent<MeshRenderer>();
        gunRenderer.enabled = true;
        gunModelRenderer.enabled = false;
        interactUI?.SetActive(false);
        gunShootingScript.enabled = false;
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

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, focusDistance))
        {
            GameObject hitObj = hit.transform.gameObject;

            // Show interact UI if focusing on any interactable
            if (hitObj == GunMesh || hitObj == GunHolder || hitObj == gameObject)
            {
                interactUI?.SetActive(true);
            }
            else
            {
                interactUI?.SetActive(false);
            }

            if (hitObj == GunMesh && Input.GetMouseButtonDown(0))
            {
                gunRenderer.enabled = false;
                gunModelRenderer.enabled = true;
                gunShootingScript.enabled = true;
                gunShootingScript.ResetGame();

                SetPlayerPosition();
                TogglePlayerMovement(false);
                LockCursor();
            }
            else if (hitObj == GunHolder && Input.GetMouseButtonDown(0))
            {
                gunRenderer.enabled = true;
                gunModelRenderer.enabled = false;
                gunShootingScript.enabled = false;
                gunShootingScript.ResetGame();

                TogglePlayerMovement(true);
                LockCursor();
            }
            else if (hitObj == gameObject && Input.GetMouseButtonDown(0))
            {
                npcUI?.SetActive(true);
                isNpcUiActive = true;
                UnlockCursor();
            }
        }
        else
        {
            interactUI?.SetActive(false);
        }
    }

    void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void SetPlayerPosition()
    {
        if (playerModel != null)
        {
            playerModel.transform.position = newPlayerPosition;
        }
    }

    void TogglePlayerMovement(bool isEnabled)
    {
        foreach (var script in playerMovementScripts)
        {
            if (script != null)
                script.enabled = isEnabled;
        }
        FirstPersonMovement.isMovementLocked = !isEnabled;
    }
}
