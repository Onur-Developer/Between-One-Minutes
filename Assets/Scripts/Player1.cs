using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Player1 : MainPlayer
{
    private bool _isActive;
    private Tween _processTween;
    private Animator _anim;
    [SerializeField] private ParticleSystem backgroundParticle;
    [SerializeField] private Transform animatorChild;
    [SerializeField] private Image processImage;
    [SerializeField] private Transform heartPanel;
    [SerializeField] private Image heartImage;
    private float _oldSpeed;
    private float _timer;
    [SerializeField] private float specialSpeed;
    [SerializeField] private Material bulletMaterial;
    [SerializeField] private Color bulletColor;
    [SerializeField] private TrailRenderer trailRenderer;
    [SerializeField] private AudioClip timeStop;
    [SerializeField] private AudioClip timeContinue;
    [SerializeField] private AudioClip touchWeb;

    protected override void Awake()
    {
        base.Awake();
        _oldSpeed = speed;
        _anim = GetComponentInChildren<Animator>();
        Sr = GetComponentInChildren<SpriteRenderer>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        _processTween = processImage.DOFillAmount(1f, 8.5f)
            .SetEase(Ease.Linear)
            .OnComplete(SpecialActive)
            .OnUpdate((() =>
            {
                _timer += Time.deltaTime;
                if (_timer > 7.4f && !_au.isPlaying)
                    PlaySound(timeStop);
            }));
        _processTween.SetAutoKill(false);
        for (int i = 0; i < health; i++)
        {
            Instantiate(heartImage, heartPanel);
        }

        _timer = 0;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        if (_processTween.IsActive())
            _processTween.Kill();
        for (int i = 0; i < heartPanel.childCount; i++)
        {
            Destroy(heartPanel.GetChild(i).gameObject);
        }

        processImage.fillAmount = 0;
        if (_isActive)
            SpecialActive();

        health = 3;
    }

    public override void TakeDamage(float damage = 10)
    {
        if (_isActive)
            return;
        health -= damage / 10;
        for (int i = 0; i < damage; i++)
        {
            Destroy(heartPanel.GetChild(heartPanel.childCount - 1).gameObject);
        }

        base.TakeDamage(damage);
    }

    protected override void IncreaseHealth()
    {
        base.IncreaseHealth();
        health++;
        Instantiate(heartImage, heartPanel);
    }


    void PlayBack()
    {
        processImage.DOFillAmount(0f, 3.5f)
            .SetEase(Ease.Linear).OnComplete((() =>
            {
                if (gameObject.activeSelf)
                {
                    _processTween.Restart();
                    SpecialActive();
                }
            }));
    }

    void ParticleSet(bool active)
    {
        if (active)
            backgroundParticle.Pause();
        else
            backgroundParticle.Play();
    }


    void SpecialActive()
    {
        _timer = 0;
        _isActive = !_isActive;
        tag = _isActive ? "Player1Active" : "Player";
        speed = _isActive ? specialSpeed : _oldSpeed;
        _anim.SetBool("isSpecial", _isActive);
        trailRenderer.emitting = _isActive;
        trailRenderer.enabled = _isActive;
        if (!_isActive)
            PlaySound(timeContinue);
        ParticleSet(_isActive);
        if (_isActive)
            StartCoroutine(nameof(RotateObj));

        if (_isActive)
        {
            if (ammos.childCount > 0)
            {
                for (int i = 0; i < ammos.childCount; i++)
                {
                    ammos.GetChild(i).GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                }
            }

            if (enemies.childCount > 0)
            {
                for (int i = 0; i < enemies.transform.childCount; i++)
                {
                    enemies.transform.GetChild(i).GetComponent<Enemy>().speed = 0;
                    enemies.transform.GetChild(i).GetComponent<Enemy>().anim.enabled = false;
                    enemies.transform.GetChild(i).GetComponent<CircleCollider2D>().isTrigger = true;
                }
            }

            PlayBack();
        }
        else
        {
            if (ammos.childCount > 0)
            {
                for (int i = 0; i < ammos.childCount; i++)
                {
                    ammos.GetChild(i).GetComponent<Ammo>().PushAgain();
                }
            }

            if (enemies.transform.childCount > 0)
            {
                for (int i = 0; i < enemies.transform.childCount; i++)
                {
                    enemies.transform.GetChild(i).GetComponent<Enemy>().speed =
                        GameManager.instance.enemyspeed;
                    enemies.transform.GetChild(i).GetComponent<Enemy>().anim.enabled = true;
                    enemies.transform.GetChild(i).GetComponent<CircleCollider2D>().isTrigger = false;
                }
            }
        }
    }

    IEnumerator RotateObj()
    {
        while (_isActive)
        {
            animatorChild.Rotate(new Vector3(0, 0, 300 * Time.deltaTime));
            yield return null;
        }

        animatorChild.rotation = Quaternion.Euler(0, 0, 0);
    }

    protected override void OnCollisionEnter2D(Collision2D col)
    {
    }

    protected override void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Ammo") && _isActive)
        {
            //Renderer renderer = col.gameObject.GetComponent<Renderer>();
            Renderer renderer2 = col.transform.GetChild(0).GetComponent<Renderer>();
            col.GetComponent<SpriteRenderer>().color = bulletColor;
            renderer2.material = bulletMaterial;
            //col.GetComponent<SpriteRenderer>().color = Color.white;
            col.GetComponent<Ammo>().isActive = true;
            col.gameObject.layer = 9;
            PlaySound(touchWeb);
        }

        base.OnTriggerEnter2D(col);
    }
}