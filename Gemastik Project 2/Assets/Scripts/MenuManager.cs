using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using TMPro;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private AudioSource m_buttonsSound;
    [SerializeField] private TextMeshProUGUI m_congratulationText;

    private float m_soundDelay = 0.2f;

    // Start is called before the first frame update
    void Start()
    {
        ActivateCongratulationText();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    private void ActivateCongratulationText()
    {
        if (PlayerPrefs.GetInt("Level5Status") == 1)
        {
            m_congratulationText.gameObject.SetActive(true);
        }
        else
        {
            m_congratulationText.gameObject.SetActive(false);
        }
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
        m_congratulationText.gameObject.SetActive(false);
    }
}