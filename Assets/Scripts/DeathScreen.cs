using CubeHopper.Audio;
using CubeHopper.Game;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CubeHopper
{
    public class DeathScreen : MonoBehaviour
    {
        [SerializeField] private AudioClip _ClickSound;
        [SerializeField] private GameObject _screen;
        private void OnEnable()
        {
            Player.OnDeath += Open;
            Rewarded.OnRewardGiven += Close;
        }
        private void OnDisable()
        {
            Player.OnDeath -= Open;
            Rewarded.OnRewardGiven -= Close;
        }
        private void Open()
        {
            _screen.SetActive(true);
            Delay.Create(1.2f, () => { _screen.transform.LeanScale(Vector3.one, 0.3f).setEaseOutQuad(); });
        }
        
        public void Close()
        {
            AudioManager.Instance.PlayAudio(_ClickSound);
            _screen.transform.LeanScale(Vector3.zero, 0.2f).setIgnoreTimeScale(true).setEaseOutQuad().setOnComplete(() =>
            _screen.SetActive(false));
        }
        public void RestrartScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
