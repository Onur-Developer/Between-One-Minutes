using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;


public class Ammo : MonoBehaviour
{
    public Vector2 direction;
    public float pushPower;
    public bool isActive;
    [SerializeField] private ParticleSystem explosion;
    [SerializeField] private AudioClip clip;
    private SpriteRenderer _sr;
    private AudioSource _au;

    private void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
        _au = GetComponent<AudioSource>();
    }


    IEnumerator Destroy()
    {
        Instantiate(explosion, transform.position, quaternion.identity);
        _sr.enabled = false;
        GetComponent<CircleCollider2D>().enabled = false;
        GetComponentInChildren<TrailRenderer>().enabled = false;
        float randi = Random.Range(.5f, 1.5f);
        _au.pitch = randi;
        _au.PlayOneShot(clip);
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }

    public void PushAgain()
    {
        float directionPoint = isActive ? -1 : 1;
        GetComponent<Rigidbody2D>().AddForce(direction.normalized * directionPoint * pushPower);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.CompareTag("Enemy") && !col.CompareTag("Health") && !col.CompareTag("Player1Active"))
            StartCoroutine(nameof(Destroy));
        else if (gameObject.layer == 9 && col.CompareTag("Enemy"))
            StartCoroutine(nameof(Destroy));
    }
}