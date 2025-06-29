using GoogleMobileAds.Api;
using GoogleMobileAds.Ump.Api;
using UnityEngine;
using System;

public class AdManager : MonoBehaviour
{
    private BannerView bannerView;
    private InterstitialAd interstitialAd;

    void Start()
    {
        MobileAds.Initialize(initStatus =>
        {
            Debug.Log("AdMob initialized");
            LoadBannerAd();
            LoadInterstitialAd();
        });
    }

    #region Banner

    public void LoadBannerAd()
    {
#if UNITY_ANDROID
        string bannerAdUnitId = "ca-app-pub-7204731156645232/7889627487";
#elif UNITY_IOS
        string bannerAdUnitId = "ca-app-pub-7204731156645232/7889627487";
#else
        string bannerAdUnitId = "unexpected_platform";
#endif

        AdSize adSize = AdSize.Banner;
        bannerView = new BannerView(bannerAdUnitId, adSize, AdPosition.Bottom);

        AdRequest adRequest = new AdRequest();
        bannerView.LoadAd(adRequest);
    }

    public void HideBanner()
    {
        bannerView?.Hide();
    }

    public void ShowBanner()
    {
        bannerView?.Show();
    }

    #endregion

    #region Interstitial

    public void LoadInterstitialAd()
    {
#if UNITY_ANDROID
        string adUnitId = "ca-app-pub-7204731156645232/5372093595";
#elif UNITY_IOS
        string adUnitId = "ca-app-pub-7204731156645232/5372093595";
#else
        string adUnitId = "unexpected_platform";
#endif

        InterstitialAd.Load(adUnitId, new AdRequest(), (InterstitialAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null)
            {
                Debug.LogError("Interstitial failed to load: " + error);
                return;
            }

            interstitialAd = ad;
            Debug.Log("Interstitial loaded");

            interstitialAd.OnAdFullScreenContentClosed += () =>
            {
                Debug.Log("Interstitial kapandı");
                Time.timeScale = 1; // Oyun durmuşsa devam et
                LoadInterstitialAd(); // Tekrar yükle
            };

            interstitialAd.OnAdFullScreenContentFailed += (AdError error) =>
            {
                Debug.LogError("Interstitial gösterilemedi: " + error.GetMessage());
            };
        });
    }

    public void ShowInterstitialAd()
    {
        if (interstitialAd != null && interstitialAd.CanShowAd())
        {
            Time.timeScale = 0; // Oyun duruyorsa kullan
            interstitialAd.Show();
        }
        else
        {
            Debug.Log("Interstitial henüz hazır değil.");
        }
    }

    #endregion
}


