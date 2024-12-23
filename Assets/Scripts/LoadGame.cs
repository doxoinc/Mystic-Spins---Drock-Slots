using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadGame : MonoBehaviour
{
    public GameObject loadingScreen;
    private string _selectedGame;
    [SerializeField] private Slider LoadingBar;
    [SerializeField] private TextMeshProUGUI LoadingText;

    public void loadGame(string selectedGame)
    {
        _selectedGame = selectedGame;
        loadingScreen.SetActive(true);
        StartCoroutine("LoadSelectedGame");
    }
    private IEnumerator LoadSelectedGame()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(_selectedGame);
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
