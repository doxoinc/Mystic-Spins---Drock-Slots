using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScene : MonoBehaviour
{
    [SerializeField] private string NextScene;
    [SerializeField] private Slider LoadingBar;
    [SerializeField] private TextMeshProUGUI LoadingText;
    private string _selectedGame;
    private void Start()
    {
        StartCoroutine("LoadScene");
    }

    IEnumerator LoadScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(NextScene);
        while (!asyncLoad.isDone)
        {
            LoadingBar.value = asyncLoad.progress;
            LoadingText.text = asyncLoad.progress * 100 + "";
            float progress = asyncLoad.progress;
            LoadingBar.value = progress;
            yield return null;
        }
    }
    
    


}

