using System.Collections;
using TMPro;
using UnityEngine;

public class Player4 : MainPlayer
{
  [SerializeField] private TextMeshProUGUI healthPower;
  private bool _isSmall=true;
  [Header("Player Properties")] [SerializeField]
  private float firstSpeed;
  [SerializeField] private Vector2 firstScale;
  [SerializeField] private float endSpeed;
  [SerializeField] private Vector2 endScale;


  public float Health
  {
    get
    {
      return health;
    }
    set
    {
      health += value;
      healthPower.text = Health.ToString();
    }
  }

  protected override void OnEnable()
  {
    base.OnEnable();
    speed = firstSpeed;
    transform.localScale = firstScale;
    Health = 1 - Health;
    _isSmall = true;
    tag = "Shield";
    var moduleNoise = particleObj.GetComponentInChildren<ParticleSystem>().noise;
    moduleNoise.enabled = false;
    StartCoroutine(nameof(ChangeDimensions));
  }


  public override void TakeDamage(float damage = 10)
  {
    damage = _isSmall ? damage : -damage;
    Health = damage;
    if(damage < 0)
      base.TakeDamage(damage);
  }

  protected override void IncreaseHealth()
  {
    base.IncreaseHealth();
    Health = 10;
  }


  IEnumerator ChangeDimensions()
  {
    yield return new WaitForSeconds(30);
    var moduleNoise = particleObj.GetComponentInChildren<ParticleSystem>().noise;
    moduleNoise.enabled = true;
    speed = endSpeed;
    transform.localScale = endScale;
    _isSmall = false;
  }
}
