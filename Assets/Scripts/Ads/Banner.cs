using GoogleMobileAds.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CubeHopper
{
    public class Banner : MonoBehaviour
    {
        #if UNITY_ANDROID
                private string _adUnitId = "ca-app-pub-3940256099942544/6300978111";
        #else
                  private string _adUnitId = "unused";
        #endif
        [SerializeField] private int _bannerWidth;
        [SerializeField] private int _bannerHeight;
        [SerializeField] private int x;
        [SerializeField] private int y;

        BannerView _bannerView;
      


        public void RequestBanner()
        {
            // Clean up banner ad before creating a new one.
            if (_bannerView != null)
            {
                _bannerView.Destroy();
            }

            AdSize adaptiveSize =
                    AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);

            _bannerView = new BannerView(_adUnitId, adaptiveSize, AdPosition.Bottom);
            AdRequest adRequest = new AdRequest();

            _bannerView.LoadAd(adRequest);
        }

       
  
    }
}
