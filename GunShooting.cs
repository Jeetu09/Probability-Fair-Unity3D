using UnityEngine;
using TMPro; // Make sure TextMeshPro is installed via Package Manager

public class GunShooting : MonoBehaviour
{
    [Header("SpinTheWheelSound")]
    public AudioSource bgmusic4;
    public AudioSource bgmusic5;
        
    public GameObject bulletPrefab;
    public Camera playerCamera;
    public GameObject[] validTargets, GreenObjects;
    public Animator winAnimation;
    public TextMeshProUGUI scoreText, yesCountText;

    public int maxBullets = 5, score = 0;

    private int bulletsRemaining, yesCount = 0;
    private bool shootingDisabled = false;

    void Start()
    {
        ResetGame();
        bgmusic4.Pause();
        bgmusic5.Pause();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !shootingDisabled && bulletsRemaining > 0)
        {
            if (IsLookingAtValidTarget())
            {
                Shoot();
                bulletsRemaining--;

                if (yesCount >= 3) CheckWin();
            }
        }

        if (bulletsRemaining <= 0 && yesCount < 3)
        {
            shootingDisabled = true;
            bgmusic5.Play();
        }
            
    }

    void Shoot()
    {
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        GameObject bullet = Instantiate(bulletPrefab, playerCamera.transform.position + ray.direction * 2, Quaternion.identity);
        bullet.GetComponent<Rigidbody>().velocity = ray.direction * 20f;

        BulletCollision bc = bullet.AddComponent<BulletCollision>();
        bc.validTargets = validTargets;
        bc.greenObjects = GreenObjects;
        bc.OnYesTriggered += () => { yesCount++; UpdateYesText(); CheckWin(); };

        Destroy(bullet, 3f);
    }

    bool IsLookingAtValidTarget()
    {
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            foreach (var target in validTargets)
                if (hit.collider.gameObject == target && target.activeSelf) return true;
        }
        return false;
    }

    void CheckWin()
    {
        if (yesCount >= 3)
        {
            score += 2;
            UpdateScoreText();
            shootingDisabled = true;
            bgmusic4.Play();
            if (winAnimation != null)
            {
                winAnimation.SetTrigger("ScoreAchievedsecondteddy");
                StartCoroutine(WaitForAnimation());
            }
        }
    }

    System.Collections.IEnumerator WaitForAnimation()
    {
        // Wait until the animator enters the state
        yield return new WaitUntil(() =>
            winAnimation.GetCurrentAnimatorStateInfo(0).IsName("YourWinAnimationStateName"));

        // Wait for the length of the animation
        float animTime = winAnimation.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(animTime);

    }


    public void ResetGame()
    {
        bulletsRemaining = maxBullets;
        yesCount = 0;
        shootingDisabled = false;
        foreach (var t in validTargets) t.SetActive(true);
        AssignColors();
        UpdateScoreText(); // just updates text, not value
    }


    void AssignColors()
    {
        foreach (var obj in GreenObjects)
        {
            obj.GetComponent<Renderer>().material.color = UnityEngine.Random.value > 0.5f ? Color.green : Color.red;
        }
    }

    void UpdateScoreText() => scoreText.text = "" + score;
    void UpdateYesText() => yesCountText.text = "" + yesCount;
}

// 👇 Add the BulletCollision class AFTER the GunShooting class in the same file
public class BulletCollision : MonoBehaviour
{
    public GameObject[] validTargets, greenObjects;
    public delegate void YesTriggered();
    public event YesTriggered OnYesTriggered;

    void OnCollisionEnter(Collision collision)
    {
        for (int i = 0; i < validTargets.Length; i++)
        {
            if (collision.gameObject == validTargets[i])
            {
                validTargets[i].SetActive(false);
                if (greenObjects[i].GetComponent<Renderer>().material.color == Color.green)
                    OnYesTriggered?.Invoke();
                Destroy(gameObject);
                break;
            }
        }
    }
}
