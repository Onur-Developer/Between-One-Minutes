using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;


public class CameraShake : MonoBehaviour
{
    private float _duration;
    private float _strength;
    private int _vibrato;
    private float _randomless;
    private Tween _shake;
    private AudioSource _audioSource;
    private Animator _animator;
    [SerializeField] private GameObject gameManagerUI;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _animator = GetComponent<Animator>();
    }


    public void ZoomIn(Transform comingObj)
    {
        Time.timeScale = 0;
        GameManager.instance.PlayerDied();
        gameManagerUI.SetActive(false);
        _animator.enabled = true;
        GameManager.instance.backgroundPlayer.enabled = false;
        transform.position = new Vector3(comingObj.position.x, comingObj.position.y, -10);
        _animator.Play("ZoomIn");
    }

   public void StopZoomIn()
    {
        _audioSource.Play();
        GameManager.instance.ReturnMouse();
        transform.position = new Vector3(0, 0, -10);
    }

   public void ZoomOut()
    {
        Time.timeScale = 1;
        GameManager.instance.backgroundPlayer.enabled = true;
        gameManagerUI.SetActive(true);
        _animator.enabled = false;
    }


    public void ShakeCamera(string shakeType)
    {
        switch (shakeType)
        {
            case "EnemyHit":
                _duration = .5f;
                _strength = 5;
                _vibrato = 10;
                _randomless = 90;
                break;
            case "Hit":
                _duration = .5f;
                _strength = 15;
                _vibrato = 20;
                _randomless = 90;
                break;
        }

        if (_shake == null)
            _shake = transform.DOShakeRotation(_duration, _strength, _vibrato, _randomless)
                .OnComplete((() => _shake=null));
    }
}