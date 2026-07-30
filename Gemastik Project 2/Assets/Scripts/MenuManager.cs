using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private AudioSource m_buttonsSound;

    private float m_soundDelay = 0.2f;

    // Start is called before the first frame update
    void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    public void PressPlayButton()
    {
        m_buttonsSound.PlayOneShot(m_buttonsSound.clip, 1f);
        StartCoroutine(PlayButtonSound());
    }
    private IEnumerator PlayButtonSound()
    {
        yield return new WaitForSeconds(m_soundDelay);
        SceneManager.LoadScene(1);
    }
    public void PressExitButton()
    {
        m_buttonsSound.PlayOneShot(m_buttonsSound.clip, 1f);
        StartCoroutine(ExitButtonSound());
    }
    private IEnumerator ExitButtonSound()
    {
        yield return new WaitForSeconds(m_soundDelay);
        //Application.Quit();
        EditorApplication.ExitPlaymode();
    }
    public void DeleteAllKeyButton()
    {
        m_buttonsSound.PlayOneShot(m_buttonsSound.clip, 1f);
        PlayerPrefs.DeleteAll();
    }
}