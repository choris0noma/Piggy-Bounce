using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

namespace PiggyBounce
{
    public class Follow : MonoBehaviour
    {
        [SerializeField] protected Transform _player;
        private float OFFSET;
        private void Awake()
        {
            _player = FindObjectOfType<Player>().transform;
            CalculateOffset();
        }
        private void LateUpdate()
        {
            CalculatePosition();
        }
        private void CalculatePosition()
        {
            transform.position = new Vector3(0, _player.position.y + (OFFSET), transform.position.z);
        }
        private void CalculateOffset()
        {
            OFFSET = Camera.main.orthographicSize * 2 / 3;
        }

    }
}
