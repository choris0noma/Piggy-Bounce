using CubeHopper.Audio;
using CubeHopper.Game;
using System.Collections;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace CubeHopper
{
    public class DeathScreen : MonoBehaviour
    {
        [SerializeField] private Button _adButton;
        [SerializeField] private TextMeshProUGUI _timer;
        [SerializeField] private AudioClip _ClickSound;
        [SerializeField] private GameObject _screen;
        private Coroutine  _coroutine;
        private void OnEnable()
        {
            Player.OnDeath += Open;
            Rewarded.OnRewardGiven += Close;
            Rewarded.OnAdStarted += StopTimer;
        }
        private void OnDisable()
        {
            Player.OnDeath -= Open;
            Rewarded.OnRewardGiven -= Close;
            Rewarded.OnAdStarted -= StopTimer;
        }
        private void Open()
        {
            _screen.SetActive(true);
           _screen.transform.LeanScale(Vector3.one, 0.3f).setEaseOutQuad().setDelay(1.2f).setOnComplete(() =>
           {
               _adButton.transform.LeanScale(new Vector3(1.2f,1.2f,1.2f), 0.5f).setLoopPingPong();
               _coroutine = StartCoroutine(CountDown());
           });
        }
        
        public void Close()
        {
            AudioManager.Instance.PlayAudio(_ClickSound);
            _screen.transform.LeanScale(Vector3.zero, 0.2f).setIgnoreTimeScale(true).setEaseOutQuad().setOnComplete(() =>
            {
                _screen.SetActive(false);
            });
        }
        private void StopTimer()
        {
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
            }
        }
        private IEnumerator CountDown()
        {
            float t = 5;
            while (t > 0)
            {
                t-= Time.deltaTime;
                _timer.text = ((int)t).ToString();
                yield return null;
            }
            RestrartScene();
        }
        public void RestrartScene()
        {
            AudioManager.Instance.PlayAudio(_ClickSound);
            _screen.transform.LeanScale(Vector3.zero, 0.2f).setIgnoreTimeScale(true).setEaseOutQuad().setOnComplete(() =>
            {
                _screen.SetActive(false);
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            });
        }
    }
}
