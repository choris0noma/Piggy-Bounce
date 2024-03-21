using System;
using UnityEngine.SceneManagement;
using UnityEngine;


namespace PiggyBounce
{
    [RequireComponent(typeof(Rigidbody2D), typeof(LineRenderer))]
    public class Player : MonoBehaviour
    {
        private const int PLATFORM_LAYER = 6;
        private const int DEATH_LAYER = 9;

        [SerializeField] private Transform _feet;
        [Header("Touch Drag settings")]
        [SerializeField][Range(1, 10)] private float _power = 5f;
        [SerializeField][Range(1, 10)] private float _maxDrag = 5f;
        [Space]
        [Header("Time rendering")]
        [SerializeField][Range(1,5)] private float _duration = 1f;
        [SerializeField][Range(0.1f,1)] private float _timeStep = 0.1f;
       
        private Rigidbody2D _rigidBody;
        private LineRenderer _lineRenderer;
        private Camera _cam;
        
        private Vector2 _startPos;

        private int _steps;
        private float _averageVelocity;
        private bool isOnGround = true;

        public delegate void PlatformLandHandler(Vector2 landPosition);
        public static event PlatformLandHandler OnLand;


        private void Awake()
        {
            _rigidBody = GetComponent<Rigidbody2D>();
            _lineRenderer = GetComponent<LineRenderer>();
            
            _steps = (int)(_duration / _timeStep);
            _averageVelocity = Physics2D.gravity.y * _rigidBody.gravityScale / 2;
            
            _cam = Camera.main;
        }

        private void Update()
        {
            //isOnGround = Physics2D.OverlapCircle(_feet.position, 0.5f, _platformLayer) && _rigidBody.velocity == Vector2.zero;

            if (!isOnGround) return;

            if (Input.GetMouseButtonDown(0)) 
                DragStart();

            if (Input.GetMouseButton(0))
                PlotTrajectory();
            
            if (Input.GetMouseButtonUp(0)) 
                DragRelease();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            int layer = collision.gameObject.layer;

            if (layer == DEATH_LAYER) Die();

            if (layer == PLATFORM_LAYER)
            {
                
                _rigidBody.velocity = Vector2.zero;
                isOnGround = true;
                OnLand?.Invoke(transform.position);
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
            Vector2 forceDirection = Vector3.ClampMagnitude(direction, _maxDrag) * _power;
            isOnGround = false;
            _rigidBody.AddForce(forceDirection, ForceMode2D.Impulse);
            _lineRenderer.positionCount = 0;
            
        }
        private void PlotTrajectory()
        {
            Vector3[] points = new Vector3[_steps];

            Vector2 mousePos = _cam.ScreenToWorldPoint(Input.mousePosition);

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

    }
}
