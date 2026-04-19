using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour
{
    public static GameSceneManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadGameScene()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void LoadGameOverScene(bool isVictory)
    {
        // 씬 전환 전 데이터 정리?
        // CommandInvoker.Instance.ClearAll(); 
        SceneManager.LoadScene(isVictory ? "EndingScene" : "GameOverScene");
    }
}
