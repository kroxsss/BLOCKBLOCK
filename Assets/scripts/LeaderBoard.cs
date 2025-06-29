using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms; // Bu using ifadesini eklemeyi unutmayın
using System.Collections.Generic; // List için gerekli olabilir

public class LeaderBoard : MonoBehaviour
{
    // Scores sınıfınızın bir referansı. Bunu Unity Inspector'dan atamanız GEREKİYOR.
    // Eğer Scores sınıfınız bir ScriptableObject ise, onu Project penceresinden sürükleyip buraya bırakın.
    // Eğer sahnedeki başka bir GameObject üzerindeyse, o GameObject'i sürükleyip bırakın.
    public Scores _scores_; 

    // Google Play Console'dan aldığınız liderlik tablosu ID'niz
    // Kendi ID'nizle güncelleyin.
    [Header("Liderlik Tablosu Ayarı")]
    [Tooltip("Google Play Console'dan aldığınız liderlik tablosu ID'si")]
    public string myLeaderboardID = "CgkI-ZLH0OEOEAIQAQ"; // Kendi liderlik tablosu ID'nizle değiştirin!


    public string mStatus { get; set; } // Durum mesajları için


    // Awake metodu, Start'tan önce çağrılır ve genellikle ilk kurulumlar için kullanılır.
    void Awake()
    {
        // Debug loglarını etkinleştir (hata ayıklama için faydalıdır)
        PlayGamesPlatform.DebugLogEnabled = true;

        // Google Play Games platformunu etkinleştir
        PlayGamesPlatform.Activate();
    }


    // Start metodu, MonoBehaviour oluşturulduktan sonra Update'in ilk yürütülmesinden önce bir kez çağrılır.
    public void Start()
    {
        // Oyuncunun kimlik doğrulamasını deneyin.
        // Skor gönderme ve liderlik tablosu gösterme işlemleri genellikle başarılı kimlik doğrulamasından sonra yapılmalıdır.
        SignInAndCheckScore();

        // Liderlik tablosu verilerini yükleme (Bu kısım zaten iyi çalışıyor gibi görünüyor)
        // Ancak bu çağrıları, skor gönderme işleminin başarısından SONRA yapmak daha mantıklı olabilir.
        // Şimdilik olduğu gibi bırakıyoruz, eğer problem olursa zamanlamayı ayarlayabiliriz.
        ILeaderboard lb = PlayGamesPlatform.Instance.CreateLeaderboard();
        lb.id = myLeaderboardID; // Kendi ID'nizi kullanın
        lb.LoadScores(ok =>
        {
            if (ok)
            {
                LoadUsersAndDisplay(lb);
            }
            else
            {
                Debug.LogError("Liderlik tablosu yüklenirken hata oluştu!");
            }
        });

        PlayGamesPlatform.Instance.LoadScores(
            GPGSIds.leaderboard_leaders_in_smoketesting, // Bu ID'nin doğru olduğundan emin olun veya kendi ID'nizi kullanın
            LeaderboardStart.PlayerCentered,
            100,
            LeaderboardCollection.Public,
            LeaderboardTimeSpan.AllTime,
            (data) =>
            {
                mStatus = "Liderlik tablosu verileri geçerli: " + data.Valid;
                mStatus += "\n yakl.:" + data.ApproximateCount + " içeriyor: " + data.Scores.Length;
                Debug.Log(mStatus); // Konsola da yazdırın
            });
    }

    /// <summary>
    /// Google Play Games'e giriş yapmayı dener ve başarılı olursa en iyi skoru gönderir.
    /// </summary>
    void SignInAndCheckScore()
    {
        if (Social.localUser.authenticated)
        {
            Debug.Log("Zaten Google Play Oyunlar'da oturum açıldı.");
            // Oturum açılmışsa skoru göndermeyi deneyin
            ReportBestScore();
            return;
        }

        Debug.Log("Google Play Oyunlar'da oturum açılıyor...");
        PlayGamesPlatform.Instance.Authenticate((success) =>
        {
            if (success != SignInStatus.Success)
            {
                Debug.Log("Google Play Oyunlar'a başarıyla giriş yapıldı!");
                // Giriş başarılı olursa en iyi skoru gönder
                ReportBestScore();
            }
            else
            {
                Debug.LogError("Google Play Oyunlar'a giriş yapılamadı. Bağlantıyı veya Play Hizmetlerini kontrol edin.");
                // Giriş başarısız olursa skoru göndermeye çalışma
            }
        });
    }

    /// <summary>
    /// Oyuncunun en iyi skorunu liderlik tablosuna gönderir.
    /// </summary>
    void ReportBestScore()
    {
        if (_scores_ == null)
        {
            Debug.LogError("HATA: '_scores_' nesnesi atanmamış veya başlatılmamış! Skor gönderilemez.");
            return;
        }

        if (!Social.localUser.authenticated)
        {
            Debug.LogWarning("Oyuncu kimliği doğrulanmadı. Skor gönderilemiyor.");
            return;
        }

        long currentBestScore = _scores_.bestScores_.score; // _scores_ nesnenizden skoru alın
        Debug.Log("Gönderilmeye çalışılan skor: " + currentBestScore + " | Liderlik Tablosu ID: " + myLeaderboardID);

        PlayGamesPlatform.Instance.ReportScore(currentBestScore, myLeaderboardID, "FirstDaily", (bool success) =>
        {
            if (success)
            {
                Debug.Log("Skor başarıyla liderlik tablosuna gönderildi: " + currentBestScore);
            }
            else
            {
                Debug.LogError("Skor liderlik tablosuna gönderilemedi.");
            }
        });
    }


    // Bu metodun doğru çalıştığı varsayılıyor. Değişiklik yok.
    void GetNextPage(LeaderboardScoreData data)
    {
        PlayGamesPlatform.Instance.LoadMoreScores(data.NextPageToken, 10,
            (results) =>
            {
                mStatus = "Liderlik tablosu verileri geçerli: " + data.Valid;
                mStatus += "\n yakl.:" + data.ApproximateCount + " içeriyor: " + data.Scores.Length;
                Debug.Log("Sonraki sayfa yüklendi: " + mStatus); // Konsola da yazdırın
            });
    }

    // Bu metodun doğru çalıştığı varsayılıyor. Değişiklik yok.
    internal void LoadUsersAndDisplay(ILeaderboard lb)
    {
        // Kullanıcı ID'lerini al
        List<string> userIds = new List<string>();

        foreach (IScore score in lb.scores)
        {
            userIds.Add(score.userID);
        }

        // Profilleri yükle ve görüntüle (veya bu durumda logla)
        Social.LoadUsers(userIds.ToArray(), (users) =>
        {
            string status = "Liderlik tablosu yükleniyor: " + lb.title + " toplam skor = " +
                            lb.scores.Length;
            foreach (IScore score in lb.scores)
            {
                IUserProfile user = FindUser(users, score.userID);
                status += "\n" + score.formattedValue + " tarafından: " +
                          (string)(
                              (user != null) ? user.userName : "**bilinmeyen_" + score.userID + "**");
            }

            Debug.Log(status);
        });
    }

    // Bu metodun doğru çalıştığı varsayılıyor. Değişiklik yok.
    private IUserProfile FindUser(IUserProfile[] users, string scoreUserID)
    {
        foreach (var user in users)
        {
            if (user.id == scoreUserID)
                return user;
        }

        return null;
    }

    /// <summary>
    /// Google Play Games liderlik tablosu UI'sını gösterir.
    /// </summary>
    public void ShowLeaderboardUI()
    {
        if (!Social.localUser.authenticated)
        {
            Debug.LogWarning("Oyuncu kimliği doğrulanmadı. Liderlik tablosu gösterilemiyor.");
            SignInAndCheckScore(); // Giriş yapmayı dene
            return;
        }
        
        Debug.Log("Liderlik tablosu UI'si gösteriliyor. ID: " + myLeaderboardID);
        PlayGamesPlatform.Instance.ShowLeaderboardUI(myLeaderboardID);
    }
}