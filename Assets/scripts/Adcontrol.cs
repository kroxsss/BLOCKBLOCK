using GoogleMobileAds.Api;
using UnityEngine;
using UnityEngine.Serialization;

public class Adcontrol : MonoBehaviour
{
    public bool displayAd = true;
    public InterstitialAdManager InterstitialAd;
    public MenuButton menuButton;
    public string sceneName = "Game";

    public void ShowInterstitialAd()
    {
        if (displayAd)
        {
            InterstitialAd.ShowInterstitialAd();
        }
        else
        {
            menuButton.LoadScene(sceneName);
        }

        displayAd = !displayAd;
    }
}