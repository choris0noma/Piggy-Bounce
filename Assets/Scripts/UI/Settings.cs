using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace JumperCube
{
    public class Settings : MonoBehaviour
    {
        [SerializeField] private GameObject _settingsPanel;
        [SerializeField] private Toggle _vibro, _sound, _music;

        private void Start()
        {
            LoadData();
        }

        public void LoadToggle(bool value, string name)
        {
            switch (name) 
            {
                case "Vibration":
                    _vibro.isOn = value;
                    PlayerPrefs.SetInt("Vibration", value ? 1 : 0);
                    break;
                case "Sound":
                    _sound.isOn = value;
                    AudioManager.Instance.SetMuteStateAudio(value);
                    PlayerPrefs.SetInt("Sound", value ? 1 : 0);
                    break;
                case "Music":
                    _music.isOn = value;
                    AudioManager.Instance.SetMuteStateMusic(value);
                    PlayerPrefs.SetInt("Music", value ? 1 : 0);
                    break;
            }
            PlayerPrefs.Save();
        }
        private void LoadData()
        {
            _vibro.isOn = PlayerPrefs.GetInt("Vibration", 1) == 1;
            _sound.isOn = PlayerPrefs.GetInt("Sound", 1) == 1;
            _music.isOn = PlayerPrefs.GetInt("Music", 1) == 1;
        }
        public void Open()
        {
            _settingsPanel.SetActive(true);
            Time.timeScale = 0f;
        }
        public void Close() 
        {
            _settingsPanel.SetActive(false);
            Time.timeScale = 1f;
        }
    }
}
