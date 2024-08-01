using GoogleMobileAds;
using GoogleMobileAds.Api;
using UnityEngine;
using UnityEngine.Events;

public class GoogleMobileAdsDemoScript : MonoBehaviour
{
    [SerializeField] private UnityEvent OnAdsInitialized;
    public void Start()
    {
        MobileAds.Initialize((InitializationStatus initStatus) =>
        {
            OnAdsInitialized?.Invoke();
        });
    }
}