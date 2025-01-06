using GoogleMobileAds.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace CubeHopper
{
    public class Rewarded : MonoBehaviour
    {

        private string _adUnitId = "ca-app-pub-5695626826162521/6577100411";
        private string _noRewardAd = "ca-app-pub-5695626826162521/3104759260";

        [SerializeField] private CanvasGroup _prompt;
        private RewardedAd _rewardedAd;
        public static Action OnRewardGiven;
        public static Action OnAdStarted;
        public static Action<int> OnMoneyRewardGiven;
        private void LoadRewardedAd(string _id)
        {
            // Clean up the old ad before loading a new one.
            if (_rewardedAd != null)
            {
                _rewardedAd.Destroy();
                _rewardedAd = null;
            }

            var adRequest = new AdRequest();

            RewardedAd.Load(_id, adRequest,
                (RewardedAd ad, LoadAdError error) =>
                {
                    if (error != null || ad == null)
                    {
                        _prompt.gameObject.SetActive(true);
                        LeanTween.value(_prompt.gameObject, (x) => { _prompt.alpha = x; }, 0, 1, 0.6f).setEaseOutQuad();
                        LeanTween.value(_prompt.gameObject, (x) => { _prompt.alpha = x; }, 1, 0, 0.6f).setEaseOutQuad().setDelay(1.2f).setOnComplete(() => {
                            _prompt.gameObject.SetActive(false);
                        });
                        return;
                    }

                    Debug.Log("Rewarded ad loaded with response : "
                              + ad.GetResponseInfo());

                    _rewardedAd = ad;
                });
        }

        public void ShowRewardedAd()
        {
            LoadRewardedAd(_noRewardAd);
            if (_rewardedAd != null && _rewardedAd.CanShowAd())
            {
                OnAdStarted?.Invoke();
                _rewardedAd.Show((Reward reward) => { OnRewardGiven?.Invoke(); });
            }
        }
        public void GiveMoneyForAd()
        {
           
            LoadRewardedAd(_adUnitId);
            if (_rewardedAd != null && _rewardedAd.CanShowAd())
            {
                _rewardedAd.Show((Reward reward) => {
                    OnMoneyRewardGiven?.Invoke((int)reward.Amount);
                });
            }
        }

    }
}
