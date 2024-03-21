using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PiggyBounce
{
    public class DeathZoneFollow: MonoBehaviour
    {
        private float OFFSET;
        private void OnEnable()
        {
            Player.OnLand += CalculatePosition;
        }
        private void OnDisable()
        {
            Player.OnLand -= CalculatePosition;
        }

        private void Awake()
        {
            OFFSET = -(Camera.main.orthographicSize + 2f);
        }

        private void CalculatePosition(Vector2 landPosition)
        {
            transform.position = new Vector3(0, landPosition.y + OFFSET, transform.position.z);
        }
    }
}
