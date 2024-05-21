using JumperCube.Entities;
using System;
using TMPro;
using UnityEngine;

namespace JumperCube.GameFlow
{
    public class ScoreCounter : MonoBehaviour
    {
        private const int MAX_DIFFICULTY = 12;
        [SerializeField] private TextMeshProUGUI _scoreText;
        [SerializeField] private TextMeshProUGUI _coinText;
        public static Action OnDifficultyChange;

        private int _threshold = 10;
        private int _level = 0;
        private int score;
        private int coins;
        private void OnEnable()
        {
            Player.OnScore += AddScore;
            Player.OnCoinTrigger += AddCoin;
        }
        private void OnDisable()
        {
            Player.OnScore -= AddScore;
            Player.OnCoinTrigger -= AddCoin;
        }
        private void AddCoin()
        {
            coins++;
            _coinText.text = coins.ToString();
        }

        private void AddScore(int value)
        {
            score += value;
            _scoreText.text = score.ToString();
            if (score >= _threshold && _level != MAX_DIFFICULTY)
            {
                IncreaseDifficulty();
            }

        }
        //sometimes i'm a little bit slow :)
        private void IncreaseDifficulty()
        {
            _threshold += _threshold;
            _level++;
            OnDifficultyChange?.Invoke();
        }

    }
}
