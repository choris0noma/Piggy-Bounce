using System.Collections.Generic;
using UnityEngine;
using CubeHopper.CameraModule;
using CubeHopper.Platform;

namespace CubeHopper.Game
{
    public class LevelGenerator : MonoBehaviour
    {
        private const float PLATFORM_HALFSIZE = 0.8f;
        [SerializeField] private List<SimplePlatform> _platforms;   
        [SerializeField] private GameObject _previousPlatform ,_currentPlatform, _nextPlatform;
        [Space]
        [Header("Player Reference")]
        [SerializeField] private Player _player;
        private List<int> _prefixSum = new List<int>();
        private int _sumOfWeights = 0;
        private int _length = 0;

        private float placableX;
        private void OnEnable()
        {
            Player.OnLand += InstantiatePlatform;
            Player.OnRelease += DestroyPlatform;
            PlayerDataManager.OnDifficultyChange += UpdateWeights;
            Rewarded.OnSecondChanceGiven += GiveSecondAttempt;
        }

        private void OnDisable()
        {
            Player.OnLand -= InstantiatePlatform;
            Player.OnRelease -= DestroyPlatform;
            PlayerDataManager.OnDifficultyChange -= UpdateWeights;
            Rewarded.OnSecondChanceGiven -= GiveSecondAttempt;
        }

        private void Awake()
        {
            _player = FindObjectOfType<Player>();
            _length = _platforms.Count;
            placableX = CameraScaler.X_SIZE / 2 - PLATFORM_HALFSIZE;
            for (int i = 0; i < _length; i++) 
            {
                _sumOfWeights += _platforms[i].Weight;
                _prefixSum.Add(0);
                for (int j = 0; j<= i; j++) 
                {
                    _prefixSum[i] += _platforms[j].Weight;
                }
            }
        }
        
        //damn, it seems like i am lactose intolerant

        private void UpdateWeights()
        {
            for (int i = 0; i< _length;i++)
            {
                int change = _platforms[i].Rate;
                _sumOfWeights += change;
                for (int j = i; j< _length; j++)
                {
                    _prefixSum[j] += change;
                }
            }
        }
      

        private void InstantiatePlatform(Vector2 playerPos)
        {
            SimplePlatform p = _platforms[GetNextIndex()];
            Vector2 spawnPos = GetSpawnPosition(playerPos, p);

            _nextPlatform = Instantiate(_platforms[GetNextIndex()].gameObject, spawnPos, Quaternion.identity);
        }

        private void DestroyPlatform()
        {
            if (_previousPlatform != null) Destroy(_previousPlatform);
            _previousPlatform = _currentPlatform;
            _currentPlatform.SetActive(false);
            _currentPlatform = _nextPlatform;
        }
        
        public Vector2 GetSpawnPosition(Vector2 playerPosition, SimplePlatform p)
        {
            float randomY = Random.Range(p.YOffset, p.YOffset+ p.YMaxOffset);
            float randomX;

            if (playerPosition.x - PLATFORM_HALFSIZE < -placableX)
                randomX = Random.Range(playerPosition.x + PLATFORM_HALFSIZE*2, placableX);
            else if(playerPosition.x + PLATFORM_HALFSIZE > placableX)
                randomX = Random.Range(-placableX, playerPosition.x - PLATFORM_HALFSIZE*2);
            else 
                randomX = Random.Range(-placableX, placableX);

            return new Vector2(randomX, playerPosition.y + randomY);
        }

        private void GiveSecondAttempt()
        {
            _previousPlatform.SetActive(true);
            _player.transform.position = new Vector3
            (
                _previousPlatform.transform.position.x,
                _previousPlatform.transform.position.y + 1,
                _previousPlatform.transform.position.z
            );
            _nextPlatform = _currentPlatform;
            _currentPlatform = _previousPlatform;
            _player.Resurrect();
        }
        private int GetNextIndex()
        {
            int randomPick = Random.Range(0, _sumOfWeights);
            for (int i = 0; i< _prefixSum.Count; i++)
            {
                if (randomPick < _prefixSum[i]) return i;
            }
            return 0;
        }
    }
}
