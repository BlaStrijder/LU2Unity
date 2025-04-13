using UnityEngine;
using UnityEngine.SceneManagement;

public class Navigator : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void LoadMainScene()
    {
        SceneManager.LoadScene(0);
    }
    public void LoadRegisterScene()
    {
        SceneManager.LoadScene(1);
    }
    public void LoadLoginScene()
    {
        SceneManager.LoadScene(2);
    }
    public void LoadWorldMenuScene()
    {
        SceneManager.LoadScene(3);
    }
}
