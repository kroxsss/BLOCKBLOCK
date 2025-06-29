using UnityEngine;

public class Adcontrol : MonoBehaviour
{
    public bool reklamgoster = true;
    public AdManager manager;
    public MenuButton menuButton;
    public string sceneName ="Game";

    public void ShowInterstitialAd()
    {
        if (reklamgoster==true)
        {
            manager.ShowInterstitialAd();
            reklamgoster = false;

        }
        else if (reklamgoster == false)
        {
            menuButton.LoadScene(name=sceneName);
            reklamgoster = true;
        }
        
    }
}
