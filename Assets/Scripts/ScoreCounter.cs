using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace PiggyBounce
{
    public class ScoreCounter : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _scoreText;
        [SerializeField] private TextMeshProUGUI _coinText;

        public static Action<int> OnChangingDifficulty;
        
        int score;
        int coins;
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

        }
    }
    public enum Levels
    {
        EASY,
        NORMAL,
        HARD,
        HELL
    }
}
