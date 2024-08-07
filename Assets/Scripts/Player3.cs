using System.Collections;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class Player3 : MainPlayer
{
    private float _endFillValue = 1f;
    private Tween _processTween;
    private Coroutine _processCoroutine;

    [Header("Properties")] [SerializeField]
    private float fillDuration;

    [SerializeField] private bool isShieldActive;

    [Header("UI Elements")] [SerializeField]
    private Image sliderProcess;

    [Header("Other Stuff")] [SerializeField]
    private GameObject shield;

    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private Sprite shieldSprite;
    [SerializeField] private ParticleSystem shieldParticle;
    [SerializeField] private AudioClip shieldActive;

    protected override void OnEnable()
    {
        base.OnEnable();
        _endFillValue = 1f;
        sliderProcess.fillAmount = .1f;
        _processTween = sliderProcess.DOFillAmount(_endFillValue, fillDuration)
            .SetEase(Ease.Linear)
            .OnComplete(ChangeShieldActive);
        Sr.sprite = defaultSprite;
    }

    protected override void OnDisable()
    {
        StopCoroutine(nameof(ProcessStartAgain));
        base.OnDisable();
        if (_processTween.IsPlaying())
            _processTween.Kill();

        shield.SetActive(false);
        gameObject.tag = "Player";
        isShieldActive = false;
    }

    public override void TakeDamage(float damage = 10)
    {
        if (!gameObject.CompareTag("Shield"))
        {
            _processTween.Kill();
            sliderProcess.fillAmount -= damage / 100;
            health = sliderProcess.fillAmount > 0.1f ? sliderProcess.fillAmount : 0;
            base.TakeDamage(damage);
            if (_processCoroutine == null && gameObject.activeSelf && health >= .1f)
                _processCoroutine = StartCoroutine(ProcessStartAgain(2.5f));
        }
    }

    protected override void IncreaseHealth()
    {
        base.IncreaseHealth();
        sliderProcess.fillAmount += .1f;
        if (_processTween.IsActive())
            _processTween.Kill();
        float newFillDuration = fillDuration - (1 - sliderProcess.fillAmount)*10;
        if (!_processTween.IsActive())
            _processTween = sliderProcess.DOFillAmount(_endFillValue, newFillDuration)
                .SetEase(Ease.Linear)
                .OnComplete(ChangeShieldActive);
        _processCoroutine = null;
    }

    IEnumerator ProcessStartAgain(float waiting)
    {
        yield return new WaitForSeconds(waiting);
        float newFillDuration = fillDuration - Mathf.Min(sliderProcess.fillAmount * 10,7.5f);
        if (!_processTween.IsActive())
            _processTween = sliderProcess.DOFillAmount(_endFillValue, newFillDuration)
                .SetEase(Ease.Linear)
                .OnComplete(ChangeShieldActive);
        _processCoroutine = null;
    }

    void ChangeShieldActive()
    {
        isShieldActive = !isShieldActive;
        if (isShieldActive)
        {
            shieldParticle.Play();
            PlaySound(shieldActive);
        }
        else
        {
            shieldParticle.Stop();
            shieldParticle.Clear();
        }
        shield.SetActive(isShieldActive);
        _endFillValue = isShieldActive ? .1f : 1f;
        gameObject.tag = isShieldActive ? "Shield" : "Player";
        Sr.sprite = isShieldActive ? shieldSprite : defaultSprite;
        _processTween = sliderProcess.DOFillAmount(_endFillValue, fillDuration)
            .SetEase(Ease.Linear)
            .OnComplete(ChangeShieldActive);
    }
}