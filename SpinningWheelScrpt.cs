using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class SpinningWheelScrpt : MonoBehaviour
{
    [Header("SpinTheWheelSound")]
    public AudioSource bgmusic;
    public AudioSource bgmusic2;
    public GameObject[] targetObjects;
    public GameObject specialObject;
    public GameObject rotatingObject;
    public GameObject interactUI;
    public float focusDistance = 5f;
    public float initialRotationSpeed = 300f;
    public Vector3 rotationAxis = Vector3.up;
    public float slowDownTime = 2f;
    public TextMeshProUGUI scoreText;
    public Animator scoreAnimator;
    public Animator scoreAnimatortwo;

    public GameObject redEquObject;
    public GameObject greenEquObject;
    public GameObject blueEquObject;
    public GameObject yellowEquObject;

    public GameObject noScoreText; // 🆕 Add this in Inspector

    private Dictionary<string, GameObject> textMeshObjects;
    private MeshRenderer[] targetMeshRenderers;
    private bool isRotating = false;
    private float rotationSpeed = 0f;
    private int lastEnabledIndex = -1;
    private Collider lastCollidedObject = null;
    private string ringnum = "";
    private int score = 0;

    void Start()
    {
        bgmusic.Pause();
        bgmusic2.Pause();
        textMeshObjects = new Dictionary<string, GameObject>
        {
            { "Red", redEquObject },
            { "Green", greenEquObject },
            { "Blue", blueEquObject },
            { "Yellow", yellowEquObject }
        };

        foreach (var textMesh in textMeshObjects.Values)
        {
            if (textMesh != null)
            {
                textMesh.SetActive(false);
            }
        }

        if (noScoreText != null)
        {
            noScoreText.SetActive(false); // 🆕 Initially hide
        }

        targetMeshRenderers = new MeshRenderer[targetObjects.Length];
        for (int i = 0; i < targetObjects.Length; i++)
        {
            if (targetObjects[i] != null)
            {
                targetMeshRenderers[i] = targetObjects[i].GetComponent<MeshRenderer>();
                if (targetMeshRenderers[i] != null)
                {
                    targetMeshRenderers[i].enabled = false;
                }
            }
        }

        interactUI.SetActive(false);
        UpdateScoreText();
    }

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        bool isLookingAtInteractable = false;

        if (Physics.Raycast(ray, out hit, focusDistance))
        {
            foreach (var target in targetObjects)
            {
                if (hit.transform.gameObject == target)
                {
                    isLookingAtInteractable = true;
                    if (Input.GetMouseButtonDown(0))
                    {
                        DisableAllTargetsExcept(target);

                        if (target.GetComponent<MeshRenderer>() != null && rotationSpeed == 0f)
                        {
                            target.GetComponent<MeshRenderer>().enabled = true;
                            lastEnabledIndex = System.Array.IndexOf(targetObjects, target);
                            ringnum = target.name;
                            Debug.Log("Enabled MeshRenderer for: " + ringnum);

                            // 🆕 Hide "No Score" text when new ring is clicked
                            if (noScoreText != null)
                            {
                                noScoreText.SetActive(false);
                            }

                            if (textMeshObjects.ContainsKey(ringnum) && textMeshObjects[ringnum] != null)
                            {
                                textMeshObjects[ringnum].SetActive(true);
                            }
                        }
                    }
                }
            }

            if (hit.transform.gameObject == specialObject)
            {
                isLookingAtInteractable = true;
                if (Input.GetMouseButtonDown(0))
                {
                    if (lastEnabledIndex == -1) return;
                    if (!isRotating)
                    {
                        isRotating = true;
                        rotationSpeed = initialRotationSpeed;
                        interactUI.SetActive(false);
                        StartCoroutine(StopRotationAfterRandomTime());
                    }
                }
            }
        }

        if (!isRotating)
        {
            interactUI.SetActive(isLookingAtInteractable);
        }

        if (rotatingObject != null && rotationSpeed > 0f)
        {
            rotatingObject.transform.Rotate(rotationAxis, rotationSpeed * Time.deltaTime, Space.Self);
        }
    }

    private IEnumerator StopRotationAfterRandomTime()
    {
        float randomTime = UnityEngine.Random.Range(5f, 10f);
        yield return new WaitForSeconds(randomTime);

        float elapsedTime = 0f;
        float startSpeed = rotationSpeed;
        while (elapsedTime < slowDownTime)
        {
            rotationSpeed = Mathf.Lerp(startSpeed, 0f, elapsedTime / slowDownTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        rotationSpeed = 0f;
        isRotating = false;
        Debug.Log("Rotation stopped after: " + randomTime + " seconds");

        HandleTriggerEvent();
        interactUI.SetActive(true);

        if (lastEnabledIndex != -1 && targetMeshRenderers[lastEnabledIndex] != null)
        {
            targetMeshRenderers[lastEnabledIndex].enabled = false;

            string disabledObjectName = targetObjects[lastEnabledIndex].name;
            if (textMeshObjects.ContainsKey(disabledObjectName) && textMeshObjects[disabledObjectName] != null)
            {
                textMeshObjects[disabledObjectName].SetActive(false);
            }

            lastEnabledIndex = -1;
        }
    }

    private void HandleTriggerEvent()
    {
        if (lastCollidedObject != null)
        {
            if (lastCollidedObject.gameObject.name == ringnum)
            {
                Debug.Log("Yeep got it !!");
                score += 2;
                if (scoreAnimatortwo != null)
                {
                    scoreAnimatortwo.SetTrigger("ScoreAchievedsecond");
                }
                
            }
            else if (lastCollidedObject.gameObject.CompareTag(targetObjects[lastEnabledIndex].tag))
            {
                Debug.Log("Yeep got it !!");
                score += 1;
                if (scoreAnimator != null)
                {
                    scoreAnimator.SetTrigger("ScoreAchieved");
                }
                bgmusic.Play();
            }
            else
            {
                bgmusic2.Play();
                Debug.Log("No Score Given!");

                // 🆕 Show noScoreText if nothing matched
                if (noScoreText != null)
                {
                    noScoreText.SetActive(true);
                }
            }
            UpdateScoreText();
        }
        else
        {
            Debug.Log("No object was touched");

            // 🆕 Show noScoreText if nothing touched
            if (noScoreText != null)
            {
                noScoreText.SetActive(true);
            }
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        lastCollidedObject = collider;
    }

    private void DisableAllTargetsExcept(GameObject targetToKeep)
    {
        foreach (var target in targetObjects)
        {
            if (target != targetToKeep && target.GetComponent<MeshRenderer>() != null && rotationSpeed == 0f)
            {
                target.GetComponent<MeshRenderer>().enabled = false;

                string targetName = target.name;
                if (textMeshObjects.ContainsKey(targetName) && textMeshObjects[targetName] != null)
                {
                    textMeshObjects[targetName].SetActive(false);
                }
            }
        }
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "" + score;
        }
    }
}
