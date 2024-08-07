using System;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering.PostProcessing;

public class Settings : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private TextMeshProUGUI musicText;
    [SerializeField] private TextMeshProUGUI sfxText;
    [SerializeField] private TextMeshProUGUI brighnessText;
    [SerializeField] private PostProcessVolume postProcessBrighness;
    private const string MixerMusic = "MusicVolume";
    private const string MixerSfx = "SFXVolume";
    private float _musicValue = -10f;
    private float _sfxValue = -10f;
    private float _brighnessValue=.8f;


    private void Start()
    {
        LoadVolumes();
    }


    public void MusicSet(bool value)
    {
        float parameterValue;
        bool result = audioMixer.GetFloat(MixerMusic, out parameterValue);
        if (result)
        {
            float count = value ? 0.1f : -0.1f;
            _musicValue += count;
            _musicValue = Mathf.Clamp(_musicValue, -80f, 20f);
            audioMixer.SetFloat(MixerMusic, _musicValue);
            musicText.text = (80 + _musicValue).ToString("0");
            SaveVolume(MixerMusic, _musicValue);
        }
    }

    public void SfxSet(bool value)
    {
        float parameterValue;
        bool result = audioMixer.GetFloat(MixerSfx, out parameterValue);
        if (result)
        {
            float count = value ? 0.1f : -0.1f;
            _sfxValue += count;
            _sfxValue = Mathf.Clamp(_sfxValue, -80f, 20f);
            audioMixer.SetFloat(MixerSfx, _sfxValue);
            sfxText.text = (80 + _sfxValue).ToString("0");
            SaveVolume(MixerSfx, _sfxValue);
        }
    }

    public void BrighnessSet(bool value)
    {
        float count = value ? 0.005f : -0.005f;
        _brighnessValue += count;
        _brighnessValue = Mathf.Clamp(_brighnessValue, 0f, 1f);
        brighnessText.text = _brighnessValue.ToString("F1");
        postProcessBrighness.weight = _brighnessValue;
        PlayerPrefs.SetFloat("Brighness",_brighnessValue);
    }

    void SaveVolume(string volumeName, float volume)
    {
        PlayerPrefs.SetFloat(volumeName, volume);
    }

    void LoadVolumes()
    {
        if (PlayerPrefs.HasKey(MixerMusic))
        {
            audioMixer.SetFloat(MixerMusic, PlayerPrefs.GetFloat(MixerMusic));
            _musicValue = PlayerPrefs.GetFloat(MixerMusic);
            musicText.text = (80 + _musicValue).ToString("0");
        }

        if (PlayerPrefs.HasKey(MixerSfx))
        {
            audioMixer.SetFloat(MixerSfx, PlayerPrefs.GetFloat(MixerSfx));
            _sfxValue = PlayerPrefs.GetFloat(MixerSfx);
            sfxText.text = (80 + _sfxValue).ToString("0");
        }

        if (PlayerPrefs.HasKey("Brighness"))
        {
            postProcessBrighness.weight = PlayerPrefs.GetFloat("Brighness");
            _brighnessValue = PlayerPrefs.GetFloat("Brighness");
            brighnessText.text = _brighnessValue.ToString("F1");
        }
    }
}