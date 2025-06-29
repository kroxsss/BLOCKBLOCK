using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButton : MonoBehaviour
{
   
    public void  LoadScene(string name)
    {
        SceneManager.LoadScene(name);
        
    }
}
