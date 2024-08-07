using System;
using UnityEngine;
using UnityEngine.UI;


public class TutorialTouch : MonoBehaviour
{
    private Image myImage;
    [SerializeField] private Sprite deactiveImage;
    [SerializeField] private Sprite activeImage;
    [SerializeField] private bool value;
    [SerializeField] private TutorialTouch tutorialTouch;


    private void Awake()
    {
        myImage = GetComponent<Image>();
    }

    private void Start()
    {
        int controlValue = value ? 1 : 0;
        myImage.sprite = PlayerPrefs.GetInt("Tutorial", 1)==controlValue ? activeImage : deactiveImage;
    }


    public void CloseImage()
    {
        myImage.sprite = deactiveImage;
    }


    void TutorialSave()
    {
        int tutorialValue = value ? 1 : 0;
        PlayerPrefs.SetInt("Tutorial",tutorialValue);
    }
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Mouse"))
        {
            GameManager.instance.isTutorial = value;
            myImage.sprite = activeImage;
            tutorialTouch.CloseImage();
            TutorialSave();
        }
    }
}
