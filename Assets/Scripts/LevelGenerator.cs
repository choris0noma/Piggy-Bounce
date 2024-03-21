using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;

namespace PiggyBounce
{
    public class LevelGenerator : MonoBehaviour
    {
        [SerializeField] private List<Platform> _platforms;
        [SerializeField] private GameObject currentPlatform, nextPlatform;
        
        private List<int> _prefixSum = new List<int>();
        private int _sumOfWeights = 0;

        private float placableX;
        private void OnEnable()
        {
            Player.OnLand += InstantiatePlatform;
        }

        private void OnDisable()
        {
            Player.OnLand -= InstantiatePlatform;
        }

        private void Awake()
        {
            placableX = CameraScaler.X_SIZE / 2 - 1;

            for (int i = 0; i < _platforms.Count; i++) 
            {
                _sumOfWeights += _platforms[i].Weight;
                _prefixSum.Add(0);
                for (int j = 0; j<= i; j++) 
                {
                    _prefixSum[i] += _platforms[j].Weight;
                }
            }
        }

        private void InstantiatePlatform(Vector2 playerPos)
        {
            Platform p = _platforms[GetNextIndex()];
            Vector2 spawnPos = GetSpawnPosition(playerPos, p);

            if (currentPlatform!= null) Destroy(currentPlatform);
            currentPlatform = nextPlatform;
            nextPlatform = Instantiate(_platforms[GetNextIndex()].gameObject, spawnPos, Quaternion.identity);
            
            
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
