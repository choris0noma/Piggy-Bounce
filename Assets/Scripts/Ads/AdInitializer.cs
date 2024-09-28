using GoogleMobileAds;
using GoogleMobileAds.Api;
using UnityEngine;
using UnityEngine.Events;

public class AdInitializer : MonoBehaviour
{
    [SerializeField] private UnityEvent OnAdsInitialized;
    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
    public void Start()
    {
        MobileAds.Initialize((InitializationStatus initStatus) =>
        {
            OnAdsInitialized?.Invoke();
        });
    }
}