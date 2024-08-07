using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Player2 : MainPlayer
{
    #region TrailVariables

    [Header("Trail Values")] [SerializeField]
    private float dashSpeed;

    [SerializeField] private float dashCooldown;
    [SerializeField] private float trailCooldown;
    private bool _isDash;
    private Vector2 _dashDirection;
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private Sprite dashSprite;
    [SerializeField] private ParticleSystem trailParticle;
    [SerializeField] private ParticleSystem trailParticle2;

    #endregion

    private Tween _progressTween;
    private Transform _healthObject;

    #region UIElements

    [Header("UI Elements")] [SerializeField]
    private Image dashButton;

    [SerializeField] private Image traitImage;
    [SerializeField] private Image healthBarProgress;
    [SerializeField] private GameObject healthBar;
    [SerializeField] private AudioClip dash;
    [SerializeField] private Image buttonImage;

    #endregion

    #region InputActions

    protected override void OnMove(InputValue value)
    {
        if (!_isDash)
        {
            base.OnMove(value);
            _dashDirection = Direction != Vector2.zero ? Direction : _dashDirection;
        }
    }

    protected override void OnStopMove()
    {
        if (!_isDash)
            base.OnStopMove();
    }

    protected override void Movement()
    {
        base.Movement();
        _dashDirection = Direction != Vector2.zero ? Direction : _dashDirection;
    }


    protected override void OnEnable()
    {
        base.OnEnable();
        traitImage.fillAmount = 0;
        _progressTween = traitImage.DOFillAmount(1, trailCooldown)
            .SetEase(Ease.Linear)
            .OnComplete((() =>
            {
                dashButton.raycastTarget = true;
                buttonImage.color = new Color(buttonImage.color.r, buttonImage.color.g,
                    buttonImage.color.b, 1f);
            }));
        _progressTween.SetAutoKill(false);
        playerUI.SetActive(true);
        healthBar.SetActive(true);
        health = 100;
        healthBarProgress.fillAmount = health / 100;
        Sr.sprite = defaultSprite;
        _joystick.gameObject.SetActive(true);
        buttonImage.color = new Color(buttonImage.color.r, buttonImage.color.g,
            buttonImage.color.b, 0f);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        healthBar.SetActive(false);
        if (_progressTween.IsActive())
            _progressTween.Kill();
        var main = trailParticle.main;
        main.startColor = Color.yellow;
        main.startSpeed = 10;
        trailParticle2.Stop();
        gameObject.tag = "Player2";
        _isDash = false;
    }

    void OnDash()
    {
        if (!_isDash && dashButton.raycastTarget && _healthObject == null)
        {
            _rb.AddForce(_dashDirection * dashSpeed);
            _isDash = true;
            dashButton.raycastTarget = false;
            traitImage.fillAmount = 0;
            _progressTween.Restart();
            StartCoroutine(nameof(DashFunction));
            PlaySound(dash);
        }
        else if (_healthObject != null)
        {
            ThrowHealth();
        }
    }

    #endregion

    void ChangeEnemiesTrigger()
    {
        if (enemies.childCount > 0)
        {
            for (int i = 0; i < enemies.childCount; i++)
            {
                enemies.GetChild(i).GetComponent<CircleCollider2D>().isTrigger = CompareTag("Shield");
            }
        }
    }

    IEnumerator DashFunction()
    {
        _joystick.gameObject.SetActive(false);
        _joystick.OnPointerUp(new PointerEventData(EventSystem.current));
        Sr.sprite = dashSprite;
        gameObject.tag = "Shield";
        ChangeEnemiesTrigger();
        var main = trailParticle.main;
        main.startColor = Color.blue;
        main.startSpeed = 150;
        trailParticle.Clear();
        trailParticle2.Play();
        buttonImage.DOFade(0, .1f);
        StartCoroutine(nameof(RotateParticle));
        yield return new WaitForSeconds(dashCooldown);
        main.startColor = Color.yellow;
        main.startSpeed = 10;
        gameObject.tag = "Player2";
        trailParticle.Clear();
        trailParticle2.Stop();
        trailParticle2.Clear();
        Sr.sprite = defaultSprite;
        _isDash = false;
        _rb.velocity = Vector2.zero;
        ChangeEnemiesTrigger();
        _joystick.gameObject.SetActive(true);
    }

    IEnumerator RotateParticle()
    {
        while (_isDash && gameObject.activeSelf)
        {
            float angle = Mathf.Atan2(_rb.velocity.y, _rb.velocity.x) * Mathf.Rad2Deg;
            trailParticle2.transform.rotation = Quaternion.AngleAxis(angle + 90, Vector3.forward);
            yield return null;
        }
    }

    public override void TakeDamage(float damage = 10)
    {
        if (!gameObject.CompareTag("Shield"))
        {
            base.TakeDamage(damage);
            healthBarProgress.DOFillAmount((health - damage) / 100, .5f)
                .SetEase(Ease.Linear)
                .OnComplete((() =>
                {
                    health -= damage;
                }));
        }
    }

    protected override void IncreaseHealth()
    {
    }

    void ThrowHealth()
    {
        _healthObject.GetComponent<CapsuleCollider2D>().isTrigger = false;
        _healthObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        _healthObject.GetComponent<Rigidbody2D>().AddForce(_dashDirection.normalized * dashSpeed);
        _healthObject = null;
    }

    protected override void OnTriggerEnter2D(Collider2D col)
    {
        base.OnTriggerEnter2D(col);
        if (col.gameObject.CompareTag("Health") && _healthObject == null)
        {
            _healthObject = col.transform;
            _healthObject.parent = transform;
        }
    }
}