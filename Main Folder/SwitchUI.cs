using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SwitchUI : MonoBehaviour
{
    public Animator animator;
    public float animationDuration = 1.0f; // Set this to match your animation length

    public void StartGame()
    {
        animator.SetTrigger("LoadUI");
        StartCoroutine(LoadSceneAfterAnimation());
    }

    private IEnumerator LoadSceneAfterAnimation()
    {
        yield return new WaitForSeconds(animationDuration);
        SceneManager.LoadScene("game");
    }
}
