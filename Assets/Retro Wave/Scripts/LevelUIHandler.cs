using UnityEngine;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(1000)]
public class LevelUIHandler : MonoBehaviour
{
    public void StartEasy()
    {
        PlayerPrefs.SetString("Difficulty", "Easy");
        SceneManager.LoadScene("EasyScene"); 
    }

    public void StartHard()
    {
        PlayerPrefs.SetString("Difficulty", "Hard");
        SceneManager.LoadScene("HardScene"); 
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("TitleScene"); 
    }
}