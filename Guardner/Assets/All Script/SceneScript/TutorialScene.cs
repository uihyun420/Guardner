using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem.iOS;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TutorialScene : MonoBehaviour
{
    [SerializeField] private Image[] images;
    [SerializeField] private float time = 1f;
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private Button button;

    private void Start()
    {
        StartCoroutine(ShowImages());
    }

    private void Awake()
    {
        button.onClick.AddListener(OnClickGameStartButton);
    }

    private IEnumerator ShowImages()
    {
        foreach (var image in images)
        {
            image.gameObject.SetActive(false);
        }

        for (int i = 0; i < images.Length; i++)
        {
            var canvasGroup = images[i].GetComponent<CanvasGroup>();
            if (canvasGroup == null)
                canvasGroup = images[i].gameObject.AddComponent<CanvasGroup>();

            images[i].gameObject.SetActive(true);
            yield return StartCoroutine(Fade(canvasGroup, 0f, 1f, fadeDuration)); // 페이드 인

            yield return new WaitForSeconds(time);

            if (i < images.Length - 1)
            {
                yield return StartCoroutine(Fade(canvasGroup, 1f, 0f, fadeDuration)); // 페이드 아웃
                images[i].gameObject.SetActive(false);
            }
        }
    }  

    private IEnumerator Fade(CanvasGroup canvasGroup, float alpha, float to, float duration)
    {
        float elapsed = 0f;
        canvasGroup.alpha = alpha;
        while(elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(alpha, to, elapsed / duration);
            yield return null;
        }
        canvasGroup.alpha = to;
    }

    private void OnClickGameStartButton()
    {
        SceneManager.LoadScene("GameScene");
    }
}


