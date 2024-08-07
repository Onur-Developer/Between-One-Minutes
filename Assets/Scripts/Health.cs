using UnityEngine;
using DG.Tweening;


public class Health : MonoBehaviour
{
    [SerializeField] private float healthIncrease;
    [SerializeField] private float collidingCount;
    private SpriteRenderer _sr;


    private void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        Invoke(nameof(ActiveCollider),1);
        _sr.DOFade(0, .4f)
            .SetEase(Ease.Linear)
            .SetLoops(9,LoopType.Yoyo)
            .SetDelay(10)
            .OnComplete((() => Destroy(gameObject)));
        GameManager.instance.RestartEvent += RestartButton;
    }

    void RestartButton()
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        GameManager.instance.RestartEvent -= RestartButton;
    }

    void ActiveCollider()
    {
        GetComponent<CapsuleCollider2D>().enabled = true;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        collidingCount++;
        if (col.gameObject.CompareTag("HealthBar"))
        {
            Player2 p2 = GameObject.FindWithTag("Player2").GetComponent<Player2>();
            healthIncrease += collidingCount * 5;
            p2.TakeDamage(-healthIncrease);
            Destroy(gameObject);
        }
        else if (collidingCount >= 4)
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if(col.CompareTag("Player") || col.CompareTag("Player1Active"))
            Destroy(gameObject);
        else if (col.CompareTag("Shield"))
        {
            Player2 cntrl = col.GetComponent<Player2>();
            if(cntrl==null)
                Destroy(gameObject);
        }
    }
}