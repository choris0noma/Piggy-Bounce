using CubeHopper.Platform;
using CubeHopper.SavingData;
using System;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;

namespace CubeHopper.Game
{
    public class PlayerDataManager : MonoBehaviour
    {
        private const int MAX_DIFFICULTY = 12;
        private const string FILE_PATH = "/data.json";

        [SerializeField] private TextMeshProUGUI _scoreText;
        [SerializeField] private TextMeshProUGUI _moneyText;
        public static Action OnDifficultyChange;
        private int _threshold = 10;
        private int _level = 0;
        private int maxScore = 0;

        private IDataService _dataService = new JsonDataService();   
        private PlayerStats stats;
        private bool isEncrypted = true;
        public int Money => stats.Money;
       
        

        public void GiveReward(int amount)
        {
            stats.Money += amount;
            _moneyText.text = stats.Money.ToString();
            if (!_dataService.SaveData(FILE_PATH, stats, isEncrypted))
            {
                Debug.LogError("could not save money");
            };
        }

        public void ExecutePurchase(int cost)
        {
            print(stats.Money);
            stats.Money -= cost;
            _moneyText.text = stats.Money.ToString();
            if (!_dataService.SaveData(FILE_PATH, stats, isEncrypted))
            {
                Debug.LogError("could not save money");
            }
        }

        private void OnEnable()
        {
            Player.OnScore += AddScore;
            Player.OnCoinTrigger += AddCoin;
            DailyReward.OnRewardClaimed += GiveReward;
            Rewarded.OnRewardGiven += GiveReward;
        }
        private void OnDisable()
        {
            Player.OnScore -= AddScore;
            Player.OnCoinTrigger -= AddCoin;
            DailyReward.OnRewardClaimed -= GiveReward;
            Rewarded.OnRewardGiven -= GiveReward;
        }
        private void Awake()
        {
            Application.targetFrameRate = 60;
            LoadPlayerData();
            maxScore = stats.Score;
            _moneyText.text = stats.Money.ToString();
            _scoreText.text = stats.Score.ToString();
        }
       
        public void ResetScore()
        {
            _scoreText.text = "0";
            stats.Score = 0;
        }
        private void AddCoin()
        {
            stats.Money++;
            _moneyText.text = stats.Money.ToString();
        }

        private void AddScore(int value)
        {
            stats.Score += value;
            _scoreText.text = stats.Score.ToString();
            if (stats.Score > maxScore)
            {
                maxScore = stats.Score;
            }
            SavePlayerData();
            if (stats.Score >= _threshold && _level != MAX_DIFFICULTY)
            {
                IncreaseDifficulty();
            }

        }
        //sometimes i'm a little bit slow :(
        private void IncreaseDifficulty()
        {
            _threshold += _threshold;
            _level++;
            OnDifficultyChange?.Invoke();
        }
        private void SavePlayerData()
        {
            PlayerStats dataToSave = stats.Score < maxScore ? new PlayerStats(maxScore, stats.Money): stats;
            
            if (!_dataService.SaveData(FILE_PATH, dataToSave, isEncrypted))
            {
                Debug.LogError("EEEee");
            }
        }

        private void LoadPlayerData()
        {
            if (_dataService.CheckPathExistence(FILE_PATH))
            {
                stats = _dataService.LoadData<PlayerStats>(FILE_PATH, isEncrypted);
            }
            else 
            {
                stats = new PlayerStats(0, 0);
                SavePlayerData();
            }
        }

    }
}

struct PlayerStats
{
    public int Score;
    public int Money;
    public PlayerStats(int score, int money)
    {
        Score = score;
        Money = money;
    }
}
