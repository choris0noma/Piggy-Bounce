using CubeHopper.SavingData;
using GoogleMobileAds.Ump.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CubeHopper
{
    public class CoinRewardButton : MonoBehaviour
    {
        private const string PATH = "/adTime.json";
        [SerializeField] private Image img;
        private const int INTERVAL_IN_SECONDS = 5;
        [SerializeField] private Button btn;
        [SerializeField] private TextMeshProUGUI _timer;

        private IDataService _dataService = new JsonDataService();
        private DateTime _lastTimeRewardClaimed;
        private TimeSpan _timeLeft;
        private bool isEncrypted = true;

        private void OnEnable()
        {
            Rewarded.OnMoneyRewardGiven += SaveTimeOfLastReward;
        }
        private void OnDisable()
        {
            Rewarded.OnMoneyRewardGiven -= SaveTimeOfLastReward;
        }
        private void Awake()
        {
            RetrieveLastTime();
            if (EnoughTimeHasPassed())
            {
                PlayerPrefs.SetInt("isGiven", 0);
                PlayerPrefs.Save();
                ActivateButton();
            }
            else
            {
                _timeLeft = TimeSpan.FromSeconds(INTERVAL_IN_SECONDS) - DateTime.Now.Subtract(_lastTimeRewardClaimed);
                DeactivateButton();
            }
        }
        private void ActivateButton()
        {
            img.color = new Color(1, 1, 1, 1);
            btn.interactable = true;
            _timer.text = "";
        }
        private void DeactivateButton()
        {
            img.color = new Color(1, 1, 1, 0.7f);
            btn.interactable = false;
            DisplayTime();
            StartCoroutine(StartTimer());
        }
        private bool EnoughTimeHasPassed()
        {
            return DateTime.Now.Subtract(_lastTimeRewardClaimed).TotalSeconds >= INTERVAL_IN_SECONDS;
        }
       
        private void DisplayTime()
        {
            _timer.text = _timeLeft.ToString(@"hh\:mm\:ss");
        }
        private IEnumerator StartTimer()
        {
            while (_timeLeft.TotalSeconds > 0)
            {
                yield return new WaitForSecondsRealtime(1f);
                _timeLeft = _timeLeft.Subtract(TimeSpan.FromSeconds(1));
                DisplayTime();
            }
            PlayerPrefs.SetInt("isGiven", 0);
            PlayerPrefs.Save();
            ActivateButton();
        }
       
    
        private void SaveTimeOfLastReward(int m = 0)
        {
            PlayerPrefs.SetInt("isGiven", 1);
            PlayerPrefs.Save();
            _lastTimeRewardClaimed = DateTime.Now;
            if (!_dataService.SaveData(PATH, _lastTimeRewardClaimed, isEncrypted))
            {
                Debug.LogError("Couldn't save time of last reward");
            }
            _timeLeft = TimeSpan.FromSeconds(INTERVAL_IN_SECONDS);
            DeactivateButton();
        }
        private void RetrieveLastTime()
        {
            if (_dataService.CheckPathExistence(PATH))
            {
                _lastTimeRewardClaimed = _dataService.LoadData<DateTime>(PATH, isEncrypted);
            }
            else
            {
                ActivateButton();
            }

        }
    }
}
