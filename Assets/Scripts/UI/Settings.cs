using CubeHopper.Audio;
using UnityEngine;
using UnityEngine.UI;

namespace CubeHopper.UI
{
    public class Settings : MonoBehaviour
    {
        [SerializeField] private GameObject _settingsPanel;
        [SerializeField] private Toggle _vibro, _sound, _music;
        public static bool isPaused { get; private set; }

        private void Start()
        {
            _settingsPanel.transform.localScale = Vector3.zero;
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
            isPaused = true;
            Time.timeScale = 0f;
            _settingsPanel.SetActive(true);
            _settingsPanel.transform.LeanScale(Vector3.one, 0.3f).setEaseInQuad().setIgnoreTimeScale(true);
        }
        public void Close() 
        {
            isPaused = false;
            Time.timeScale = 1f;
            _settingsPanel.transform.LeanScale(Vector3.zero, 0.2f).setIgnoreTimeScale(true).setEaseInQuad().setOnComplete(()=> 
            _settingsPanel.SetActive(false));
        }
    }
}
