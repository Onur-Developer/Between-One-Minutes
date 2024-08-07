using UnityEngine;
using DG.Tweening;
using TMPro;

public class GameSettings : MonoBehaviour
{
    private GameManager _myİnstance;

    [Header("Wall")] [SerializeField] private RectTransform[] WallUI;
    [SerializeField] private RectTransform indentation;
    [SerializeField] private TextMeshProUGUI cycleText;

    [Header("Particle Systems")] [SerializeField]
    private ParticleSystem[] backgroundParticles;

    private AudioSource _au;

    private void Start()
    {
        _myİnstance = GameManager.instance;
        _myİnstance.CompleteCircleEvent += UpgradeParticles;
        _myİnstance.CompleteCircleEvent += UpgradeWallUI;
        _myİnstance.CompleteCircleEvent += CompleteCycle;
        _myİnstance.RestartEvent += ResetWallUI;
        _myİnstance.RestartEvent += ResetParticles;
        _au = GetComponent<AudioSource>();
    }

    void CompleteCycle()
    {
        cycleText.gameObject.SetActive(true);
        cycleText.DOFade(0, .5f)
            .SetLoops(7,LoopType.Yoyo)
            .SetEase(Ease.Linear)
            .OnComplete((() =>
            {
                cycleText.gameObject.SetActive(false);
                cycleText.DOFade(1, .1f);
            }));
        _au.Play();
    }

    void UpgradeParticles()
    {
        for (int i = 0; i < backgroundParticles.Length; i++)
        {
            var mainModule = backgroundParticles[i].main;
            mainModule.startLifetimeMultiplier += 1;
            ParticleSystem.ShapeModule shapeModule = backgroundParticles[i].shape;
            Vector3 newPosition = shapeModule.position;
            newPosition.x += -5;
            mainModule.startSpeedMultiplier +=1;
            shapeModule.position = newPosition;
            shapeModule.radius += 2;
        }
        
    }

    void UpgradeWallUI()
    {
        for (int i = 0; i < WallUI.Length; i++)
        {
            WallUI[i].sizeDelta = new Vector2(WallUI[i].sizeDelta.x + 5.5f, WallUI[i].sizeDelta.y + 3.3f); // 7.5f 4.3f
        }

        indentation.anchoredPosition = new Vector2(indentation.anchoredPosition.x,
            indentation.anchoredPosition.y-1.58f);
        indentation.sizeDelta = new Vector2(indentation.sizeDelta.x,
            indentation.sizeDelta.y + .15f);
    }

    void ResetWallUI()
    {
        for (int i = 0; i < WallUI.Length; i++)
        {
            WallUI[i].sizeDelta = new Vector2(54, 30.2f);
        }
        indentation.anchoredPosition = new Vector2(-18.73f,-14.4439f);
        indentation.sizeDelta = new Vector2(1,1.339f);
    }

    void ResetParticles()
    {
        for (int i = 0; i < backgroundParticles.Length; i++)
        {
            var mainModule = backgroundParticles[i].main;
            mainModule.startLifetimeMultiplier = 7.5f;
            ParticleSystem.ShapeModule shapeModule = backgroundParticles[i].shape;
            Vector3 newPosition = shapeModule.position;
            newPosition.x = -30;
            mainModule.startSpeedMultiplier =7.5f;
            shapeModule.position = newPosition;
            shapeModule.radius = 20;
        }
    }
}
