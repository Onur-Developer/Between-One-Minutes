using System.Collections;
using UnityEngine;

public class AudioTouch : MonoBehaviour
{
    [SerializeField] private bool isMusic;
    [SerializeField] private bool isBrighness;
    [SerializeField] private bool value;
    [SerializeField] private Settings settings;
    private bool _isPlayerIn;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Mouse"))
        {
            _isPlayerIn = true;
            StartCoroutine(nameof(ChangeVolume));
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Mouse"))
        {
            _isPlayerIn = false;
        }
    }

    IEnumerator ChangeVolume()
    {
        while (_isPlayerIn)
        {
            if (isMusic)
                settings.MusicSet(value);
            else if(isBrighness)
                settings.BrighnessSet(value);
            else
                settings.SfxSet(value);
            yield return new WaitForSeconds(0.009f);
        }
    }
}