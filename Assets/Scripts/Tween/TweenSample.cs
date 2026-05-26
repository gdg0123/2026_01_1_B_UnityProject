using UnityEngine;
using DG.Tweening;
using TMPro;

public class TweenSample : MonoBehaviour
{
    [Header("펀치 스캐일 예시")]
    public RectTransform punchUITarget;
    public GameObject punchObjectTarget;

    [Header("숫자 연출 예시")]
    public TMP_Text countText;
    public int currentValue = 0;
    public int addValue = 100;

    private int targetValue;

    [Header("색 변형 연출 예시")]
    public Color flashColor = Color.yellow;

    private Color originalColor;

    [Header("페이드 UI 그룹")]
    public CanvasGroup fadeTarget;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        originalColor = countText.color;
        fadeTarget.alpha = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            PlayPunchUIScale();
        }
        if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            PlayPunchObjectScale();
        }
        if(Input.GetKeyDown(KeyCode.Alpha3))
        {
            PlayUIShake();
        }
        if(Input.GetKeyDown(KeyCode.Alpha4))
        {
            PlayCountUp();
        }
        if(Input.GetKeyDown(KeyCode.Alpha5))
        {
            PlayColorFlash();
        }
        if(Input.GetKeyDown(KeyCode.Alpha6))
        {
            PlayFade();
        }
    }

    public void PlayPunchUIScale()
    {
        if(punchUITarget == null)
           return;

        punchUITarget.DOKill();
        punchUITarget.localScale = Vector3.one;
        punchUITarget.DOPunchScale(Vector3.one * 0.3f, 0.25f, 8, 1.0f);  //방향, 크기, 시간, 진동횟수, 탄성
    }

    public void PlayPunchObjectScale()
    {
        if (punchUITarget == null)
            return;

        punchObjectTarget.transform.DOKill();
        punchObjectTarget.transform.localScale = Vector3.one;
        punchObjectTarget.transform.DOPunchScale(Vector3.one * 0.3f, 0.25f, 8, 1.0f);  //방향, 크기, 시간, 진동횟수, 탄성
    }

    public void PlayUIShake()
    {
        if (punchUITarget == null)
            return;
        punchUITarget.DOKill();
        punchUITarget.DOShakeAnchorPos(0.3f, 20f, 20, 90f); //시간, 강도, 진동횟수, 랜덤성
    }

    public void PlayCountUp()
    {
        if (countText == null)
            return;

        targetValue += addValue;

        DOTween.Kill("CountTween", true);

        DOTween.To(
            () => currentValue,
            value =>
            {
                currentValue = value;
                countText.text = currentValue.ToString();
            },
            targetValue,
            0.5f
        )
        .SetEase(Ease.OutQuad)
        .SetId("CountTween");
    }

    public void PlayColorFlash()
    {
        if (countText == null)
            return;

        countText.DOKill();

        countText.color = originalColor;

        countText.DOColor(flashColor, 0.1f)
            .OnComplete(() =>
            {
                countText.DOColor(originalColor, 0.2f);
            });
    }

    public void PlayFade()
    {
        if (fadeTarget == null)
            return;

        fadeTarget.DOKill();
        fadeTarget.alpha = 0f;

        Sequence seq = DOTween.Sequence();

        seq.Append(fadeTarget.DOFade(1f, 0.2f));
        seq.AppendInterval(0.5f);
        seq.Append(fadeTarget.DOFade(0f, 0.3f));
    }
}
