using System;
using UnityEngine.SceneManagement;
using UnityEngine;
using JumperCube.Util;
using JumperCube.Motion;
using UnityEngine.UIElements;
using UnityEditor;

namespace JumperCube.Entities
{
    [RequireComponent(typeof(Rigidbody2D), typeof(LineRenderer))]
    public class Player : MonoBehaviour
    {
        
        private const int PLATFORM_LAYER = 6;
        private const int DEATH_LAYER = 9;
        private const int COIN_LAYER = 11;

        [Header("Touch Drag settings")]
        [SerializeField][Range(1, 10)] private float _power = 5f;
        [SerializeField][Range(1, 10)] private float _maxDrag = 5f;
        [Space]
        [Header("Time rendering")]
        [SerializeField][Range(1,5)] private float _duration = 1f;
        [SerializeField][Range(0.1f,1)] private float _timeStep = 0.1f;
        [Space]
        [Header("Sprites")]
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Sprite _flying;
        [SerializeField] private Sprite _idle;
        [Space]
        [SerializeField] private ParticleSystem _particleSystem;
        [Space]
        [SerializeField] private CameraFollow _cameraFollow;
        [Space]
        [SerializeField] private AudioClip _coinPickUp;

        private Rigidbody2D _rigidBody;
        private LineRenderer _lineRenderer;
        private Camera _cam;
        private CameraFollow _camFollow;

        private Vector2 _startPos;

        private int _steps;
        private float _averageVelocity;
        private bool isOnGround = true;

        public static Action<Vector2> OnLand;
        public static Action OnRelease;
        public static Action<int> OnScore;
        public static Action OnCoinTrigger;

        private void Awake()
        {
            _rigidBody = GetComponent<Rigidbody2D>();
            _lineRenderer = GetComponent<LineRenderer>();

            _steps = (int)(_duration / _timeStep);
            _averageVelocity = Physics2D.gravity.y * _rigidBody.gravityScale / 2;
            
            _cam = Camera.main;
            _camFollow = _cam.GetComponent<CameraFollow>();
        }
        private void Start()
        {
            if (UItools.IsOnUI()) return;

            if (!isOnGround) return;

            if (Input.GetMouseButtonDown(0))
                DragStart();

            if (Input.GetMouseButton(0))
                PlotTrajectory();

            if (Input.GetMouseButtonUp(0))
                DragRelease();
            
        }
        private void Update()
        {
            if (UItools.IsOnUI()) return;
            FlipSprite();

            if (!isOnGround && Input.GetMouseButtonDown(0))
            {
                _rigidBody.velocity = Vector2.zero;
                _rigidBody.AddForce(new Vector2(0, -(_power) * 3), ForceMode2D.Impulse);
                // a little more of fun into gameplay
            }
            if (Input.GetMouseButtonDown(0))
                DragStart();
            else if (Input.GetMouseButton(0))
                PlotTrajectory();
            else if (Input.GetMouseButtonUp(0))
                DragRelease();
        }
        
        private void FlipSprite()
        {
            if (_rigidBody.velocity.x > 0)
                transform.localScale = new Vector2(1, 1);
            else if (_rigidBody.velocity.x < 0)
                transform.localScale = new Vector2(-1, 1);
        }
        
       
        
        private void OnTriggerEnter2D(Collider2D collision)
        {
            int layer = collision.gameObject.layer;

            if (layer == DEATH_LAYER) Die();
            if (layer == COIN_LAYER)
            {
                Destroy(collision.gameObject);
                AudioManager.Instance.PlayAudio(_coinPickUp);
                _particleSystem.Emit(100);
                OnCoinTrigger?.Invoke();
            }
            if (layer == PLATFORM_LAYER)
            {
                _rigidBody.velocity = Vector2.zero;
                collision.transform.parent.parent.GetComponent<Platform>().DeactivatePlatform();
                isOnGround = true;
                OnLand?.Invoke(transform.position);
                _spriteRenderer.sprite = _idle;
                OnScore?.Invoke(1);
            }
        }
        
        private void Die()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        private void DragStart()
        {
            _startPos = _cam.ScreenToWorldPoint(Input.mousePosition);
        }
        private void DragRelease()
        {
            Vector2 releasePos = _cam.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = _startPos - releasePos;
            if (IsDragCancelled(releasePos) || AngleCheck(direction))
            {
                _lineRenderer.positionCount = 0;
                return;
            }
            Vector2 forceDirection = Vector3.ClampMagnitude(direction, _maxDrag)* _power;
            _spriteRenderer.sprite = _flying;
            _rigidBody.AddForce(forceDirection, ForceMode2D.Impulse);
            OnRelease?.Invoke();
            isOnGround = false;
            _lineRenderer.positionCount = 0;
            
        }
       
        private bool AngleCheck(Vector2 launchDirection)
        {
            float angle = -Mathf.Atan2(launchDirection.y, launchDirection.x) * Mathf.Rad2Deg;
            if (angle >= 0 && angle <= 180) return true;
            else return false;
            //quick way around 
        }
        private void PlotTrajectory()
        {

            Vector2 mousePos = _cam.ScreenToWorldPoint(Input.mousePosition);
            if (IsDragCancelled(mousePos) || !isOnGround)
            {
                _lineRenderer.positionCount = 0;
                return;
            }
            Vector3[] points = new Vector3[_steps];

            Vector3 direction = _startPos - mousePos;
            Vector3 launchPos = transform.position;
           
            Vector3 forceDirection = Vector3.ClampMagnitude(direction, _maxDrag) * _power;

            for (int i = 0; i<_steps; i++)
            {
                float time = i * _timeStep;
                Vector3 newPos = launchPos  +  forceDirection * time;
                newPos.y +=  _averageVelocity * Mathf.Pow(time, 2);

                points[i] = newPos;
            }

            _lineRenderer.positionCount = _steps;
            _lineRenderer.SetPositions(points);
        }
        private bool IsDragCancelled(Vector2 mousePos)
        {
            return Vector2.Distance(mousePos, _startPos) < _maxDrag / 3;
        }

    }
}
