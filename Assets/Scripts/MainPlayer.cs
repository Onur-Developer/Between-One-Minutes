using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class MainPlayer : MonoBehaviour
{
    [SerializeField] protected float speed;
    [SerializeField] protected float health;
    protected Rigidbody2D _rb;
    protected Vector2 Direction;
    protected Transform ammos;
    protected Transform enemies;
    [SerializeField] protected GameObject playerUI;
    [SerializeField] protected GameObject playerUI2;
    [SerializeField] protected GameObject particleObj;
    [SerializeField] protected GameObject howToPlayPanel;
    [SerializeField] private AudioClip backgroundMusic;
    [SerializeField] private AudioClip hitSound;
    [SerializeField] private AudioClip powerUp;
    [SerializeField] private string[] videoNames;
    private int _videoStacker=-1;
    protected Joystick _joystick;
    private bool _istakeDamage;
    protected SpriteRenderer Sr;
    protected AudioSource _au;


    protected virtual void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        if (GameManager.instance != null)
        {
            ammos = GameManager.instance.ammos;
            enemies = GameManager.instance.enemies.transform;
        }

        Sr = GetComponent<SpriteRenderer>();
        _joystick = GameManager.instance.jf;
        _au = GetComponent<AudioSource>();
    }

    public virtual void TakeDamage(float damage = 10)
    {
        Died();
        PlaySound(hitSound);
    }

   public void PlayTutorialVideo()
    {
        if (videoNames[0] == "" || !GameManager.instance.isTutorial)
            return;
        _videoStacker++;
        _videoStacker = videoNames.Length <= _videoStacker ? 0 : _videoStacker;
        string videoPath = System.IO.Path.Combine(Application.streamingAssetsPath,videoNames[_videoStacker]);
        GameManager.instance.videoBackground.SetActive(true);
        GameManager.instance.videoPlayer.url = videoPath;
        GameManager.instance.videoPlayer.Play();
        string videoCountWrite = (_videoStacker + 1)+"/"+videoNames.Length;
        GameManager.instance.videoCount.text = videoCountWrite;
        GameManager.instance.backgroundPlayer.Stop();
        if (Time.timeScale != 0)
            Time.timeScale = 0; 
    }

    protected void PlaySound(AudioClip sound)
    {
        float randi = Random.Range(.75f, 1.26f);
        _au.pitch = randi;
        _au.PlayOneShot(sound);
    }

    protected virtual void IncreaseHealth()
    {
    }

    void Died()
    {
        if (health <= 0)
            GameManager.instance.cameraShake.ZoomIn(transform);
        else
            IsTakeDamage();
    }

    public void StopPlayer()
    {
        OnStopMove();
    }

    protected virtual void OnStopMove()
    {
        _rb.velocity = Vector2.zero;
    }

    protected virtual void OnEnable()
    {
        playerUI.SetActive(true);
        particleObj.SetActive(true);
        GameManager.instance.jn.mainPlayer = this;
        GameManager.instance.backgroundPlayer.clip = backgroundMusic;
        GameManager.instance.backgroundPlayer.Play();
        if (playerUI2 == null)
            return;
        playerUI2.SetActive(true);
        howToPlayPanel.SetActive(true);
        StartCoroutine(nameof(CloseHowtoPlay));
        PlayTutorialVideo();
    }

    protected virtual void OnDisable()
    {
        playerUI.SetActive(false);
        if (playerUI2 == null)
            return;
        playerUI2.SetActive(false);
        particleObj.SetActive(false);
        howToPlayPanel.SetActive(false);
        _videoStacker = -1;
    }

    IEnumerator CloseHowtoPlay()
    {
        yield return new WaitForSeconds(10f);
        howToPlayPanel.SetActive(false);
    }

    private void FixedUpdate()
    {
        Movement();
    }


    protected void IsTakeDamage()
    {
        _istakeDamage = true;
        GameManager.instance.cameraShake.ShakeCamera("Hit");
        Sr.DOFade(0, .1f)
            .SetEase(Ease.Linear)
            .SetLoops(6, LoopType.Yoyo)
            .OnComplete((() =>
            {
                _istakeDamage = false;
                Sr.color = new Color(Sr.color.r, Sr.color.g, Sr.color.b, 1f);
            }));
    }

    protected virtual void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Enemy"))
        {
            if (!_istakeDamage)
                TakeDamage();
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Enemy") || col.CompareTag("Ammo"))
        {
            if (!_istakeDamage)
                TakeDamage();
        }
        else if (col.CompareTag("Health") && col.transform.parent != transform)
        {
            IncreaseHealth();
            PlaySound(powerUp);
        }
    }


    #region InputActions

    protected virtual void OnMove(InputValue value)
    {
        Direction = value.Get<Vector2>();
        _rb.velocity = Direction * speed;
    }

    protected virtual void Movement()
    {
        if (!GameManager.instance.isJoyStick)
            return;
        Direction = _joystick.Direction;
        _rb.velocity = Direction * speed;
    }

    #endregion
}