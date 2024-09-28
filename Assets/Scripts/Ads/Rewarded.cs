using GoogleMobileAds.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CubeHopper
{
    public class Rewarded : MonoBehaviour
    {
        private const int FIXED_AMOUNT_OF_COINS = 40;

        #if UNITY_ANDROID
                private string _adUnitId = "ca-app-pub-3940256099942544/5224354917";
        #elif UNITY_IPHONE
          private string _adUnitId = "ca-app-pub-3940256099942544/1712485313";
        #else
          private string _adUnitId = "unused";
        #endif

        private RewardedAd _rewardedAd;
        public static Action OnRewardGiven;
        public static Action<int> OnMoneyRewardGiven;

        private void LoadRewardedAd()
        {
            // Clean up the old ad before loading a new one.
            if (_rewardedAd != null)
            {
                _rewardedAd.Destroy();
                _rewardedAd = null;
            }

            var adRequest = new AdRequest();

            RewardedAd.Load(_adUnitId, adRequest,
                (RewardedAd ad, LoadAdError error) =>
                {
                    if (error != null || ad == null)
                    {
                        Debug.LogError("Rewarded ad failed to load an ad " +
                                       "with error : " + error);
                        return;
                    }

                    Debug.Log("Rewarded ad loaded with response : "
                              + ad.GetResponseInfo());

                    _rewardedAd = ad;
                });
        }

        public void ShowRewardedAd()
        {
            LoadRewardedAd();
            if (_rewardedAd != null && _rewardedAd.CanShowAd())
            {
                _rewardedAd.Show((Reward reward) => { OnRewardGiven?.Invoke(); });
            }
        }
        public void GiveMoneyForAd()
        {
            int isGiven = PlayerPrefs.GetInt("isGiven", 0);
            if (isGiven != 1)
            {
                LoadRewardedAd();
                if (_rewardedAd != null && _rewardedAd.CanShowAd())
                {
                    _rewardedAd.Show((Reward reward) => {
                        OnMoneyRewardGiven?.Invoke(FIXED_AMOUNT_OF_COINS);
                    });
                }
            }
        }

    }
}
