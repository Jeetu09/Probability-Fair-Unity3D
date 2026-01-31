using UnityEngine;
using System.Collections;
using TMPro;

public class BallAndBag : MonoBehaviour
{
    [Header("SpinTheWheelSound")]
    public AudioSource bgmusic2;
    public AudioSource bgmusic3;

    public GameObject[] targetObjects;
    public GameObject specialObject;
    public Animator specialObjectAnimator;
    public GameObject colorChangingObject;
    public TextMeshPro[] numberTexts;
    public GameObject focusUI;

    private MeshRenderer[] targetMeshRenderers;
    private int lastEnabledIndex = -1;
    private bool isAnyTargetEnabled = false;
    private bool isAnimationPlaying = false;
    private bool canClickSpecialObject = false;
    public float animationDuration = 5f;

    private Color[] colors = { Color.red, Color.green, Color.blue, Color.yellow };
    private string[] colorNames = { "Red", "Green", "Blue", "Yellow" };
    private Color lastSelectedColor;
    private string lastSelectedColorName;
    private string selectedTargetColorName;

    public Animator scoreAnimatorCaptwo;
    public TextMeshProUGUI scoreText;
    private int score = 0;
    private int[] randomNumbers = new int[4];

    [Header("Wheel Animators")]
    public Animator wheelAnimator1;
    public Animator wheelAnimator2;

    private float wheel1PlaybackTime = 0f;
    private float wheel2PlaybackTime = 0f;

    void Start()
    {
        bgmusic3.Pause();
        bgmusic2.Pause();
        UpdateScoreText();
        GenerateRandomNumbers();

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

        if (focusUI != null)
        {
            focusUI.SetActive(false);
        }
    }

    void Update()
    {
        if (isAnimationPlaying)
        {
            if (focusUI != null) focusUI.SetActive(false);
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            bool isFocusing = false;

            foreach (var target in targetObjects)
            {
                if (hit.transform.gameObject == target)
                {
                    isFocusing = true;
                    if (Input.GetMouseButtonDown(0))
                    {
                        EnableTarget(target);
                        PlayWheelAnimations();
                    }
                }
            }

            if (hit.transform.gameObject == specialObject && isAnyTargetEnabled && canClickSpecialObject)
            {
                isFocusing = true;
                if (Input.GetMouseButtonDown(0))
                {
                    StartCoroutine(PlaySpecialAnimation());
                    ChangeObjectColor();
                }
            }

            if (focusUI != null)
            {
                focusUI.SetActive(isFocusing);
            }
        }
        else
        {
            if (focusUI != null) focusUI.SetActive(false);
        }
    }

    IEnumerator PlaySpecialAnimation()
    {
        isAnimationPlaying = true;
        if (focusUI != null) focusUI.SetActive(false);

        // Pause wheel animations and record time
        wheel1PlaybackTime = wheelAnimator1.GetCurrentAnimatorStateInfo(0).normalizedTime;
        wheel2PlaybackTime = wheelAnimator2.GetCurrentAnimatorStateInfo(0).normalizedTime;
        wheelAnimator1.speed = 0;
        wheelAnimator2.speed = 0;

        specialObjectAnimator.ResetTrigger("PlayAnimation");
        specialObjectAnimator.SetTrigger("PlayAnimation");

        yield return new WaitForSeconds(animationDuration);

        if (lastEnabledIndex >= 0 && lastEnabledIndex < targetObjects.Length)
        {
            targetMeshRenderers[lastEnabledIndex].enabled = false;
        }

        isAnimationPlaying = false;
        isAnyTargetEnabled = false;
        canClickSpecialObject = false;

        GenerateRandomNumbers();

        if (selectedTargetColorName == lastSelectedColorName)
        {
            Debug.Log("✅ Matched! Score increased.");
            score += 2;
            if (scoreAnimatorCaptwo != null)
            {
                scoreAnimatorCaptwo.SetTrigger("ScoreAchievedsecondCap");
            }
            UpdateScoreText();
            bgmusic2.Play();
        }
        else
        {
            bgmusic3.Play();
            Debug.Log("❌ No match. No score.");
        }

        ResumeWheelAnimations();
    }

    private void EnableTarget(GameObject target)
    {
        DisableAllTargetsExcept(target);
        if (target.GetComponent<MeshRenderer>() != null)
        {
            target.GetComponent<MeshRenderer>().enabled = true;
            lastEnabledIndex = System.Array.IndexOf(targetObjects, target);
            isAnyTargetEnabled = true;
            canClickSpecialObject = true;

            // Determine expected color name based on selected target
            if (target.name.Contains("Red")) selectedTargetColorName = "Red";
            else if (target.name.Contains("Green")) selectedTargetColorName = "Green";
            else if (target.name.Contains("Blue")) selectedTargetColorName = "Blue";
            else if (target.name.Contains("Yellow")) selectedTargetColorName = "Yellow";
            else selectedTargetColorName = "";
        }
    }

    private void DisableAllTargetsExcept(GameObject targetToKeep)
    {
        foreach (var target in targetObjects)
        {
            if (target != targetToKeep && target.GetComponent<MeshRenderer>() != null)
            {
                target.GetComponent<MeshRenderer>().enabled = false;
            }
        }
    }

    private void ChangeObjectColor()
    {
        if (colorChangingObject != null)
        {
            int maxIndex = 0;
            for (int i = 1; i < randomNumbers.Length; i++)
            {
                if (randomNumbers[i] > randomNumbers[maxIndex])
                {
                    maxIndex = i;
                }
            }

            lastSelectedColor = colors[maxIndex];
            lastSelectedColorName = colorNames[maxIndex];

            colorChangingObject.GetComponent<MeshRenderer>().material.color = lastSelectedColor;
            Debug.Log("Color changed to: " + lastSelectedColorName);
        }
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "" + score;
        }
    }

    private void GenerateRandomNumbers()
    {
        int total = 100;
        for (int i = 0; i < 3; i++)
        {
            randomNumbers[i] = UnityEngine.Random.Range(10, total - (3 - i) * 10 + 1) / 10 * 10;
            total -= randomNumbers[i];
        }
        randomNumbers[3] = total;

        for (int i = 0; i < numberTexts.Length; i++)
        {
            if (numberTexts[i] != null)
            {
                numberTexts[i].text = randomNumbers[i].ToString();
            }
        }
    }

    private void PlayWheelAnimations()
    {
        if (wheelAnimator1 != null) wheelAnimator1.speed = 1;
        if (wheelAnimator2 != null) wheelAnimator2.speed = 1;
    }

    private void ResumeWheelAnimations()
    {
        if (wheelAnimator1 != null)
        {
            wheelAnimator1.Play(0, 0, wheel1PlaybackTime % 1f);
            wheelAnimator1.speed = 1;
        }

        if (wheelAnimator2 != null)
        {
            wheelAnimator2.Play(0, 0, wheel2PlaybackTime % 1f);
            wheelAnimator2.speed = 1;
        }
    }
}
