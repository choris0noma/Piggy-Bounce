using CubeHopper.Game;
using UnityEngine;

namespace CubeHopper.Camera
{
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField] private AnimationCurve _curve;
        private float OFFSET;
        private void OnEnable()
        {
            Player.OnLand += LerpToPlayer;
        }
        private void OnDisable()
        {
            Player.OnLand -= LerpToPlayer;
        }
        private void Awake()
        {
            OFFSET = UnityEngine.Camera.main.orthographicSize * 2 / 3;
        }
        private void LerpToPlayer(Vector2 landPos)
        {
            transform.LeanMove(new Vector2(0, landPos.y + OFFSET), 1f).setEaseOutQuint();
            //StartCoroutine(FollowPlayer(landPos));
        }
        
        //want to impose my beatles taste to everyone
    }
}
