using UnityEngine;
using GoogleMobileAds.Api;
using System;

public class InterstitialAdManager : MonoBehaviour
{
    private InterstitialAd interstitialAd;

    // Test reklam ID (kendi AdMob ID’inle değiştir)
    private string adUnitId = "ca-app-pub-7204731156645232/5372093595";

    private void Start()
    {
        // AdMob başlatma
        MobileAds.Initialize(initStatus => { });

        // İlk reklamı yükle
        LoadInterstitialAd();
    }

    private void LoadInterstitialAd()
    {
        Debug.Log("Interstitial reklam yükleniyor...");

        // Önceki reklamı temizle
        if (interstitialAd != null)
        {
            interstitialAd.Destroy();
            interstitialAd = null;
        }

        // Yeni SDK’da direkt böyle
        AdRequest adRequest = new AdRequest();

        // Reklamı yükle
        InterstitialAd.Load(adUnitId, adRequest, (InterstitialAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null)
            {
                Debug.LogError("Interstitial reklam yüklenemedi: " + error);
                return;
            }

            Debug.Log("Interstitial reklam yüklendi.");
            interstitialAd = ad;

            // Eventler
            interstitialAd.OnAdFullScreenContentOpened += () =>
            {
                Debug.Log("Reklam açıldı.");
            };

            interstitialAd.OnAdFullScreenContentClosed += () =>
            {
                Debug.Log("Reklam kapandı. Yeni reklam yükleniyor...");
                LoadInterstitialAd(); // reklam kapandıktan sonra yeni reklam yükle
            };

            interstitialAd.OnAdFullScreenContentFailed += (AdError err) =>
            {
                Debug.LogError("Reklam gösterilemedi: " + err);
                LoadInterstitialAd(); // hata olursa tekrar yükle
            };
        });
    }

    public void ShowInterstitialAd()
    {
        if (interstitialAd != null && interstitialAd.CanShowAd())
        {
            interstitialAd.Show();
            Debug.Log("Interstitial reklam gösterildi.");
        }
        else
        {
            Debug.Log("Interstitial reklam hazır değil.");
        }
    }
}