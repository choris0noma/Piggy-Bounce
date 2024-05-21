using System.Collections.Generic;
using UnityEngine;
using JumperCube.Entities;

namespace JumperCube.GameFlow
{
    public class LevelGenerator : MonoBehaviour
    {
        [SerializeField] private List<Platform> _platforms;
        [SerializeField] private GameObject currentPlatform, nextPlatform;
        
        private List<int> _prefixSum = new List<int>();
        private int _sumOfWeights = 0;
        private int _length = 0;

        private float placableX;
        private void OnEnable()
        {
            Player.OnLand += InstantiatePlatform;
            Player.OnRelease += DestroyPlatform;
            ScoreCounter.OnDifficultyChange += UpdateWeights;

        }

        private void OnDisable()
        {
            Player.OnLand -= InstantiatePlatform;
            Player.OnRelease -= DestroyPlatform;
            ScoreCounter.OnDifficultyChange -= UpdateWeights;
        }

        private void Awake()
        {
            _length = _platforms.Count;
            placableX = CameraScaler.X_SIZE / 2 - 1;

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
        private void Start()
        {
            //InstantiatePlatform(new Vector2(0, -3.606073f));
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
            Platform p = _platforms[GetNextIndex()];
            Vector2 spawnPos = GetSpawnPosition(playerPos, p);

            nextPlatform = Instantiate(_platforms[GetNextIndex()].gameObject, spawnPos, Quaternion.identity);
        }

        private void DestroyPlatform()
        {
            if (currentPlatform != null) Destroy(currentPlatform);
            currentPlatform = nextPlatform;
        }

        public Vector2 GetSpawnPosition(Vector2 playerPosition, Platform p)
        {
            float offset = p.YOffset;
            float randomX = Random.Range(-placableX, placableX);
            float randomY = Random.Range(offset, offset + p.YMaxDistance);
            return new Vector2(randomX, playerPosition.y + randomY);
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
