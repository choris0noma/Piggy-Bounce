using JumperCube.Entities;
using UnityEngine;

namespace JumperCube.Motion
{
    public class Follow : MonoBehaviour
    {
        private Transform _player;
        private float OFFSET;
        private void Awake()
        {
            _player = FindObjectOfType<Player>().transform;
            OFFSET = (Camera.main.orthographicSize * 2) / 3; //just one third, idk just because
        }
        private void LateUpdate()
        {
            transform.position = new Vector3(0, _player.position.y + (OFFSET), transform.position.z);
        }
      
    }
}
