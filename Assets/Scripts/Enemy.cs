using System.Collections;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour
{
    private Vector2 _destination;
    private MainPlayer _player;
    private float _launchPower;
    private SpriteRenderer _sr;

    [Header("Distance Settings")] [SerializeField]
    private float xMinDistance;

    [SerializeField] private float xMaxDistance;
    [SerializeField] private float yMinDistance, yMaxDistance;

    [Header("Enemy Properties")] [HideInInspector]
    public float speed;

    [SerializeField] private float attackCoolDownMin;
    [SerializeField] private float attackCoolDownMax;

    [Header("Other Stuff")] [SerializeField]
    private GameObject ammoPrefab;

    [SerializeField] private GameObject healthObject;
    private Transform _players;
    [SerializeField] private ParticleSystem explosion;
    [SerializeField] private ParticleSystem deadBodyInstance;
    public Animator anim;
    [SerializeField] private AudioClip hit;
    [SerializeField] private AudioClip spawn;
    [SerializeField] private AudioClip throwWeb;
    private ParticleSystem _part;
    private Tween _attackTween;


    private void Awake()
    {
        speed = GameManager.instance.enemyspeed;
        xMinDistance = GameManager.instance.xMinDistance;
        xMaxDistance = GameManager.instance.xMaxDistance;
        yMinDistance = GameManager.instance.yMinDistance;
        yMaxDistance = GameManager.instance.yMaxDistance;
        _players = GameManager.instance.players;
        _launchPower = GameManager.instance.lauchPower;
        anim = GetComponent<Animator>();
        attackCoolDownMax = GameManager.instance.enemyAttackCooldownMax;
        _sr = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        FindPlayer();
        SetDestination();
        StartCoroutine(nameof(AttackCorotine));
        ChooseSpider();
        PlaySound(spawn);
    }

    private void OnDestroy()
    {
        if(_attackTween!=null)
            _attackTween.Kill();
        if (_part == null)
        {
            Instantiate(explosion, transform.position, quaternion.identity);
            ParticleSystem deadBody= Instantiate(deadBodyInstance, transform.position, Quaternion.identity);
            deadBody.textureSheetAnimation.SetSprite(0,_sr.sprite);
        }
    }

    IEnumerator Destroy()
    {
        _sr.enabled = false;
        GetComponent<CircleCollider2D>().enabled = false;
        PlaySound(hit);
        GameManager.instance.cameraShake.ShakeCamera("EnemyHit");
       _part= Instantiate(explosion, transform.position, quaternion.identity);
      ParticleSystem deadBody= Instantiate(deadBodyInstance, transform.position, Quaternion.identity);
      deadBody.textureSheetAnimation.SetSprite(0,_sr.sprite);
       yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }

    void PlaySound(AudioClip clip)
    {
        float randi = Random.Range(.5f, 1.5f);
        GetComponent<AudioSource>().pitch = randi;
        GetComponent<AudioSource>().PlayOneShot(clip);
    }


    private void Update()
    {
        Move();
        CheckPosition();
    }

    void FindPlayer()
    {
        _player = _players.GetComponentInChildren<MainPlayer>();
    }

    void ChooseSpider()
    {
        int random = Random.Range(0, 2);
        anim.SetBool("isBlue", random == 0);
    }


    void SetDestination()
    {
        float xPos = Random.Range(xMinDistance, xMaxDistance);
        float yPos = Random.Range(yMinDistance, yMaxDistance);
        _destination = new Vector2(xPos, yPos);
    }

    void Move()
    {
        transform.position = Vector2.MoveTowards(transform.position,
            _destination, speed * Time.deltaTime);
    }

    void CheckPosition()
    {
        if (Vector2.Distance(transform.position, _destination) < .5f)
            SetDestination();
    }

    void Attack()
    {
       _attackTween= transform.DOScale(new Vector2(1.5f, 1.5f), .4f)
            .From()
            .SetEase(Ease.InCirc)
            .OnComplete(CreateAmmo);
    }

    void CreateAmmo()
    {
        if (speed == 0 || !_sr.enabled)
            return;
        Vector2 _direction = _player.transform.position - transform.position;
        GameObject _ammo = Instantiate(ammoPrefab, transform.position, transform.rotation);
        _ammo.transform.parent = GameManager.instance.ammos;
        _ammo.GetComponent<Rigidbody2D>().AddForce(_direction.normalized * _launchPower);
        _ammo.GetComponent<Ammo>().direction = _direction;
        _ammo.GetComponent<Ammo>().pushPower = _launchPower * 2f;
        PlaySound(throwWeb);
    }

    IEnumerator AttackCorotine()
    {
        float attackCoolDown = Random.Range(attackCoolDownMin, attackCoolDownMax);
        yield return new WaitForSeconds(attackCoolDown);
        if (speed != 0)
            Attack();
        StartCoroutine(nameof(AttackCorotine));
    }

    void Died()
    {
        float dropChance = Random.Range(0f, 1f);
        if (dropChance <= .1f)
            Instantiate(healthObject, transform.position, transform.rotation);
        GameManager.instance.KillingEnemies = 10;
        StartCoroutine(nameof(Destroy));
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == 9 || col.CompareTag("Shield"))
        {
            Died();
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Shield"))
        {
            Died();
        }
    }
}