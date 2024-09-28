using System;
using UnityEngine.SceneManagement;
using UnityEngine;
using CubeHopper.CameraModule;
using System.Collections.Generic;
using CubeHopper.UI;
using CubeHopper.Audio;
using CubeHopper.Platform;
using System.Runtime.CompilerServices;
using UnityEngine.Rendering;

namespace CubeHopper.Game
{
    public class Player : MonoBehaviour
    {
        
        private const int PLATFORM_LAYER = 6;
        private const int OBSTACLE_LAYER = 7;
        private const int DEATH_LAYER = 9;
        private const int COIN_LAYER = 11;
        private const int BORDER_LAYER = 13;
        
        private const float BOUNCE_REDUCTION = 0.75f;
        private float HALF_PLAYER_SIZE = 0.5f;
        private int REFLECTION_LAYER = (1 << OBSTACLE_LAYER) | (1 << BORDER_LAYER);
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
        [SerializeField] private ParticleSystem _deathParticles;
        [SerializeField] private ParticleSystem _coinParticles;
        [Space]
        [SerializeField] private CameraFollow _cameraFollow;
        [Space]
        [Header("Sound")]
        [SerializeField] private AudioClip _coinSound;
        [SerializeField] private AudioClip _launchSound;
        [SerializeField] private AudioClip _losingSound;
        [SerializeField] private AudioClip _landingSound;
        [Space]
        [Header("Colors")]
        [SerializeField] private Gradient _coinPickUpColor;
        [SerializeField] private Gradient _deathColor;
        [Space]
        [SerializeField] private TrailRenderer _trailRenderer;
        [SerializeField] private GameObject _dot;
        [SerializeField] private List<GameObject> _dots = new List<GameObject>();


        private Rigidbody2D _rigidBody;
        private Camera _cam;

        private Vector2 _startPos;

        private int _steps;
        private float _gravitationalVelocity;
        private bool isOnGround = true;

        public static Action<Vector2> OnLand;
        public static Action OnRelease;
        public static Action<int> OnScore;
        public static Action OnCoinTrigger;

        private float _previous_height = 0;
        private float _diff = 0;
        private int _particleCount = 50;

        public static Action OnDeath;
        private void OnEnable()
        {
            CameraFollow.OnCameraStop += AdjustStarPos;
        }
        private void OnDisable()
        {
            CameraFollow.OnCameraStop -= AdjustStarPos;
        }
        private void AdjustStarPos()
        {
            _diff = transform.position.y - _previous_height;
            _startPos.y += _diff;
        }
        private void Awake()
        {
            _rigidBody = GetComponent<Rigidbody2D>();
            HALF_PLAYER_SIZE = transform.localScale.x / 2;
            _steps = (int)Mathf.Round(_duration / _timeStep);
            _gravitationalVelocity = Physics2D.gravity.y * _rigidBody.gravityScale / 2;

            _cam = Camera.main;

            for (int i = 0; i < _steps; i++)
            {
                _dots.Add(Instantiate(_dot));
                _dots[i].SetActive(false);
            }
        }
        
        private void Update()
        {
            
            if (UItools.IsOnUI() || Settings.isPaused ) return;

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

        private bool AngleCheck(Vector2 launchDirection)
        {
            float angle = -Mathf.Atan2(launchDirection.y, launchDirection.x) * Mathf.Rad2Deg;
            if (angle >= 0 && angle <= 180) return true;
            else return false;
        }


        private void OnTriggerEnter2D(Collider2D collision)
        {
            int layer = collision.gameObject.layer;

            if (layer == DEATH_LAYER) Die();
            if (layer == COIN_LAYER)
            {
                Destroy(collision.gameObject);
                AudioManager.Instance.PlayAudio(_coinSound);
                Handheld.Vibrate();
                Instantiate(_coinParticles , transform.position, Quaternion.identity);
                OnCoinTrigger?.Invoke();
            }
            if (layer == PLATFORM_LAYER)
            {
                AudioManager.Instance.PlayAudio(_landingSound);
                Handheld.Vibrate();
                _rigidBody.velocity = Vector2.zero;
                collision.transform.parent.parent.GetComponent<SimplePlatform>().DeactivatePlatform();
                isOnGround = true;
                OnLand?.Invoke(transform.position);
                _spriteRenderer.sprite = _idle;
                OnScore?.Invoke(1);
            }
        }
        bool isResurrected = false;
        
        public void Resurrect()
        {
            isResurrected = true;
            gameObject.SetActive(true);
        }
        private void Die()
        {
            AudioManager.Instance.PlayAudio(_losingSound);
            Instantiate(_deathParticles, transform.position, Quaternion.identity);
            _spriteRenderer.sprite = _idle;
            isOnGround = true;
            gameObject.SetActive(false);
            if (isResurrected)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
            else
            {
                OnDeath?.Invoke();
            }
        }
        private void DragStart()
        {
            _startPos = _cam.ScreenToWorldPoint(Input.mousePosition);
        }
     
        private void DragRelease()
        {
            _previous_height = transform.position.y;
            Vector2 releasePos = _cam.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = _startPos - releasePos;
            if (IsDragCancelled(releasePos) || AngleCheck(direction))
            {
                foreach (GameObject dot in _dots) { dot.SetActive(false);}
                return;
            }
            Vector2 forceDirection = Vector3.ClampMagnitude(direction, _maxDrag)* _power;
            _spriteRenderer.sprite = _flying;
            _rigidBody.AddForce(forceDirection, ForceMode2D.Impulse);
            AudioManager.Instance.PlayAudio(_launchSound);
            OnRelease?.Invoke();
            isOnGround = false;
            foreach (GameObject dot in _dots) { dot.SetActive(false); }
        }

        private void PlotTrajectory()
        {
            Vector2 mousePos = _cam.ScreenToWorldPoint(Input.mousePosition);
            if (IsDragCancelled(mousePos) || !isOnGround)
            {
                foreach (GameObject dot in _dots)
                {
                    dot.SetActive(false);
                }
                return;
            }

            Vector2 direction = _startPos - mousePos;
            Vector2 launchPos = new Vector2(transform.position.x, transform.position.y);

            Vector2 forceDirection = Vector2.ClampMagnitude(direction, _maxDrag) * _power;
            Vector2 prevPos = launchPos;
            float time = 0f;
            for (int i = 0; i < _steps; i++)
            {
                time += _timeStep;
                float scale = Mathf.Lerp(0.25f, 0.1f, (float)i / _steps);
                Vector2 newPos = launchPos + forceDirection * time;
                newPos.y += _gravitationalVelocity * Mathf.Pow(time, 2) - HALF_PLAYER_SIZE;
                RaycastHit2D ray = Physics2D.Linecast(prevPos, newPos, REFLECTION_LAYER);
                if (ray)
                {
                    launchPos = ray.point + ray.normal*0.01f;
                    forceDirection = Vector2.Reflect(forceDirection, ray.normal)  * BOUNCE_REDUCTION;
                    prevPos = launchPos;
                    time = _timeStep;
                    newPos = launchPos + forceDirection*time;
                    newPos.y += _gravitationalVelocity * Mathf.Pow(time, 2) - HALF_PLAYER_SIZE;
                }
                else
                {
                    prevPos = newPos;
                }

                _dots[i].transform.position = newPos;
                _dots[i].transform.localScale = new Vector2(scale, scale);
            }

            float closeness = Vector2.Distance(mousePos, _startPos) / _maxDrag;
            foreach (GameObject dot in _dots)
            {
                dot.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, closeness);
                dot.SetActive(true);
            }
        }

        private bool IsDragCancelled(Vector2 mousePos)
        {
            return Vector2.Distance(mousePos, _startPos) < _maxDrag / 3;
        }

        public void SetSkin(Sprite idleSprite, Sprite flySprite, Gradient traceColor)
        {
            _idle = idleSprite;
            _flying = flySprite;
            _spriteRenderer.sprite = _idle;
            _trailRenderer.colorGradient = traceColor;
        }
    }
}
