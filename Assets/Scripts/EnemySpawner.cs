using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;
using DG.Tweening;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject crossOutPrefab;
    private Vector2 _crossPos;
    private GameObject _player1;
    private GameObject _cross;
    private Tween _t;
    private Transform _enemyParent;
    private float _enemySpawner;
    private int _portalSpawn;


    private void Awake()
    {
        _player1 = GameManager.instance.playerPool[0].transform.gameObject;
        _enemyParent = GameManager.instance.enemies.transform;
        _enemySpawner = GameManager.instance.enemySpawn;
        _portalSpawn = Convert.ToInt32(_enemySpawner * 2);
    }


    private void OnEnable()
    {
        StartCoroutine(nameof(StartSpawn));
    }

    private void OnDisable()
    {
        StopCoroutine(nameof(StartSpawn));
        if (_t.IsActive())
        {
            _t.Kill();
            if (_cross != null)
                Destroy(_cross);
        }
    }

    IEnumerator StartSpawn()
    {
        CreateCrossOut();
        yield return new WaitForSeconds(_enemySpawner);
        StartCoroutine(nameof(StartSpawn));
    }


    void CreateCrossOut()
    {
        if (_player1.CompareTag("Player1Active"))
            return;
        float xPos = Random.Range(GameManager.instance.xMinDistance, GameManager.instance.xMaxDistance);
        float yPos = Random.Range(GameManager.instance.yMinDistance, GameManager.instance.yMaxDistance);
        _crossPos = new Vector2(xPos, yPos);
        if (crossOutPrefab == null)
            return;
        GameObject myCross = Instantiate(crossOutPrefab, transform);
        _cross = myCross;
        myCross.transform.position = _crossPos;
        _t = myCross.transform.DOScale(new Vector2(.4f, .4f), 0.5f)
            .From()
            .SetEase(Ease.Linear)
            .SetLoops(_portalSpawn, LoopType.Yoyo)
            .OnComplete((() =>
            {
                if (myCross != null)
                    Destroy(myCross.gameObject);
                if (_player1.CompareTag("Player1Active"))
                    return;
                CreateEnemy();
                _t = null;
            }));
    }

    public void CreateEnemy()
    {
        GameObject enemy = Instantiate(enemyPrefab, _enemyParent);
        enemy.transform.position = _crossPos;
    }
}