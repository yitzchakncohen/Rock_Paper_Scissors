using System;
using GoogleMobileAds.Api;
using UnityEngine;

namespace RockPaperScissors.Ads
{
    public class AdsManager : MonoBehaviour
    {
        // These ad units are configured to always serve test ads.
#if UNITY_ANDROID
        private string _adUnitId = "ca-app-pub-3940256099942544/5354046379";
#elif UNITY_IPHONE
        private string _adUnitId = "ca-app-pub-3940256099942544/6978759866";
#else
        private string _adUnitId = "unused";
#endif
        private RewardedInterstitialAd rewardedInterstitialAd;
        public bool adsInitialized {get; private set;} = false;

        public void Start()
        {
            // Initialize the Google Mobile Ads SDK.
            MobileAds.Initialize((InitializationStatus initStatus) =>
            {
                // This callback is called once the MobileAds SDK is initialized.
                adsInitialized = true;
                LoadRewardedInterstitialAd();
            });
        }

        /// <summary>
        /// Loads the rewarded interstitial ad.
        /// </summary>
        public void LoadRewardedInterstitialAd()
        {
            // Clean up the old ad before loading a new one.
            if (rewardedInterstitialAd != null)
            {
                    rewardedInterstitialAd.Destroy();
                    rewardedInterstitialAd = null;
            }

            Debug.Log("Loading the rewarded interstitial ad.");

            // create our request used to load the ad.
            var adRequest = new AdRequest();
            adRequest.Keywords.Add("unity-admob-sample");

            // send the request to load the ad.
            RewardedInterstitialAd.Load(_adUnitId, adRequest,
                (RewardedInterstitialAd ad, LoadAdError error) =>
                {
                    // if error is not null, the load request failed.
                    if (error != null || ad == null)
                    {
                        Debug.LogError("rewarded interstitial ad failed to load an ad " +
                                        "with error : " + error);
                        return;
                    }

                    Debug.Log("Rewarded interstitial ad loaded with response : "
                                + ad.GetResponseInfo());

                    rewardedInterstitialAd = ad;
                    RegisterReloadHandler(ad);
                });
        }

        public void ShowRewardedInterstitialAd(Action<Reward> rewardCallback)
        {
            const string rewardMsg =
                "Rewarded interstitial ad rewarded the user. Type: {0}, amount: {1}.";

            if (rewardedInterstitialAd != null && rewardedInterstitialAd.CanShowAd())
            {
                rewardedInterstitialAd.Show((Reward reward) =>
                {
                    rewardCallback(reward);
                    Debug.Log(string.Format(rewardMsg, reward.Type, reward.Amount));
                });
                rewardedInterstitialAd.Destroy();
            }
        }

        private void RegisterReloadHandler(RewardedInterstitialAd ad)
        {
            // Raised when the ad closed full screen content.
            ad.OnAdFullScreenContentClosed += () =>
            {
                Debug.Log("Rewarded interstitial ad full screen content closed.");

                // Reload the ad so that we can show another as soon as possible.
                LoadRewardedInterstitialAd();
            };
            // Raised when the ad failed to open full screen content.
            ad.OnAdFullScreenContentFailed += (AdError error) =>
            {
                Debug.LogError("Rewarded interstitial ad failed to open " +
                            "full screen content with error : " + error);

                // Reload the ad so that we can show another as soon as possible.
                LoadRewardedInterstitialAd();
            };
        }
    }
    
}
