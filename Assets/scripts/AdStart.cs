using UnityEngine;
using GoogleMobileAds.Api;

public class AD : MonoBehaviour
{
    void Start()
    {
        // Google Mobile Ads SDK’yı başlat
        MobileAds.Initialize(initStatus => {
            Debug.Log("Google Mobile Ads SDK başlatıldı.");
        });
    }
}