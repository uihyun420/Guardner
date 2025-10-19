using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TutorialScene : MonoBehaviour
{
    [SerializeField] private Image[] images;
    [SerializeField] private float time = 1f;
    [SerializeField] private float fadeDuration = 0.5f;

    [SerializeField] private BattleUi battleUi;
    [SerializeField] private GameObject map;

    [SerializeField] private Button tutorialSkipButton;
    private void Start()
    {
        battleUi.gameObject.SetActive(false);
        map.SetActive(false);
        StartCoroutine(ShowImages());
    }

    private void Awake()
    {
        tutorialSkipButton.onClick.AddListener(onClickTutorialSkipButton);
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

            yield return StartCoroutine(Fade(canvasGroup, 1f, 0f, fadeDuration)); // 페이드 아웃
            images[i].gameObject.SetActive(false);
        }

        battleUi.gameObject.SetActive(true);
        map.SetActive(true);
    }
    private void onClickTutorialSkipButton()
    {
        SceneManager.LoadScene("GameScene");
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

    private IEnumerator TutorialProgress()
    {
        yield return null;
    }
}


