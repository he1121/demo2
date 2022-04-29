using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public GameObject pauseMenu;
    public AudioMixer audioMixer;
    public Slider volumeSlider;
    public AudioClip 菜单背景音乐;

    AudioSource 菜单背景音源;

    public GameObject loadingPanel;
    public Slider loadSlider;
    public Text loadPercentText;

    private void Start()
    {
        Time.timeScale = 1f;
        volumeSlider.value = 0;

        菜单背景音源 = gameObject.AddComponent<AudioSource>();
        PlayMenuAudio();


    }

    public void PlayMenuAudio()
    {
        菜单背景音源.clip = 菜单背景音乐;
        菜单背景音源.loop = true;
        菜单背景音源.Play();

    }

    private void Update()
    {
        KeyToPause();
    }
    public void PlayGame()
    {
        SceneManager.LoadScene("FirstMap");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    void KeyToPause()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseGame();
        }
    }


    public void PauseGame()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
    }

    public void BackToGame()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;

    }

    public  void BackToMenu()
    {
        AudioManager.PauseBackgroundAudio();
        SceneManager.LoadScene("Menu");
    }

    public void SetVolume(float Volume)
    {
        audioMixer.SetFloat("bgMusicVolume", Volume);
    }

    public void LoadBar(int sceneIndex)
    {
        StartCoroutine(AsyncLoadBar(sceneIndex));
    }

    IEnumerator AsyncLoadBar(int sceneIndex)
    {
        //异步载入场景
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
        loadingPanel.SetActive(true);

        while (!operation.isDone)
        {
            float percent = operation.progress / 0.9f;
            loadSlider.value = percent;
            loadPercentText.text = Mathf.FloorToInt(percent * 100f) + "%";
            yield return null;
        }

    }
}
