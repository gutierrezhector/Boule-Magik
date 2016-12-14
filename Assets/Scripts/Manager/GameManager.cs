﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;


public class GameManager : MonoBehaviour
{

    [SerializeField]
    GameObject[] m_platformsGO; // tab for all platforms

    [Range(0, 100)]
    [SerializeField]
    int m_chanceBonus;

    [SerializeField]
    float m_spaceBetweenPlatform; // space between platform

    [SerializeField]
    float m_startSpeedScroll; // the default speed of the level translate

    [SerializeField]
    float m_updateSpeedScroll; // the speed added at each tick

    [SerializeField]
    GameObject m_firstBackground; // first background

    [SerializeField]
    GameObject m_secondBackground; // second background

    [SerializeField]
    GameObject m_firstFloor; // first floor

    [SerializeField]
    GameObject m_secondFloor; // second floor

    [SerializeField]
    GameObject m_player; // GO of the player

    [SerializeField]
    Transform m_spawnPlatform; // the position of where platform are instanciated

    [SerializeField]
    GameObject m_deathParticleSystem; // the GO with the particle system played when player is dead

    [SerializeField]
    Text m_uiPoint; // the Text from the GUI to show points

    [SerializeField]
    AudioSource m_musicManager; // audio source for music

    [SerializeField]
    AudioSource m_soundManager; // audio source for sound

    [SerializeField]
    AudioClip m_musicDefault; // the Text from the GUI to show points

    [SerializeField]
    AudioClip m_loseSound; // the Text from the GUI to show points

    [SerializeField]
    Sprite m_playSprite; // the Text from the GUI to show points

    [SerializeField]
    Sprite m_pauseSprite; // the Text from the GUI to show points

    [SerializeField]
    GameObject m_retryButton; // the retry button when the player lose

    [SerializeField]
    GameObject m_pauseButton; // the pause button

    [SerializeField]
    GameObject m_menuButton; // the menu button to get back to the menu scene when the player lose

    public enum e_bonusType { SPEED, POINT }

    OptionManager m_optionManager;

    List<GameObject> m_platformTab; // list of platforms instanciated
    GameObject currentPlatform; // the current platform handled
    int m_points; // current points of the player
    float m_speedScroll; // the current speed of the level translate
    float m_timeBetweenTwoInstancePlatform; // variable to save the time passed to know when to make another instance of a new platform
    float m_sizePlatformSave; // variable to save the size of the last platform instanciated
    int m_countTab; // count set randomly to instance a random platform
    bool m_isPaused; // is game paused ?
    bool m_isLose; // is the game lost ?
    float timeWhenLose; // the moment the player lost

    // UNITY METHODES

    void Start()
    {
        m_platformTab = new List<GameObject>();
        m_optionManager = GameObject.Find("OptionManager").GetComponent<OptionManager>();
        m_musicManager.clip = m_musicDefault;
        if (PlayerPrefs.GetInt("MusicToggle") == 1)
            m_musicManager.Play();
        m_startSpeedScroll = m_optionManager.SpeedStart;
        m_updateSpeedScroll = m_optionManager.Speedupdate;
        initGame();
    }

    void Update()
    {
        manageInput();
        if (!Pause)
        {
            if (m_sizePlatformSave <= (m_timeBetweenTwoInstancePlatform - m_spaceBetweenPlatform))
            {
                m_timeBetweenTwoInstancePlatform = 0;
                m_countTab = Random.Range(0, m_platformsGO.Length);
                currentPlatform = Instantiate(m_platformsGO[m_countTab], m_spawnPlatform.position, Quaternion.identity) as GameObject;
                currentPlatform.GetComponent<Platform>().initPlatform(m_speedScroll, m_chanceBonus);
                m_sizePlatformSave = currentPlatform.GetComponent<Platform>().WidthSize;
                m_platformTab.Add(currentPlatform);
                updateSpeedScroll(m_updateSpeedScroll);
                Debug.Log("Instance platform : " + currentPlatform.name + ", speed scroll : " + m_speedScroll);
            }
            m_timeBetweenTwoInstancePlatform += Time.deltaTime * m_speedScroll;
        }
    }

    // PRIVATE METHODES

    void initGame()
    {
        m_countTab = 0; // tab platform count;
        m_isPaused = false; // game paused ?
        m_isLose = false; // lose state
        m_points = 0; // point number
        currentPlatform = null; // instance variable for current platform
        updateSpeedScroll(m_startSpeedScroll); // set default speed scroll
        updatePoint(m_points); // set default point
        m_firstFloor.GetComponent<ScrollingEntity>().initEntity(m_startSpeedScroll); // set default speed for level transtaltion
        m_secondFloor.GetComponent<ScrollingEntity>().initEntity(m_startSpeedScroll); // set default speed for level transtaltion
        m_firstBackground.GetComponent<ScrollingEntity>().initEntity(m_startSpeedScroll); // set default speed for level transtaltion
        m_secondBackground.GetComponent<ScrollingEntity>().initEntity(m_startSpeedScroll); // set default speed for level transtaltion
        m_speedScroll = m_startSpeedScroll; // set default speed for level transtaltion
        m_timeBetweenTwoInstancePlatform = 0; // set the default value for the time passed between two instance of platform
        m_sizePlatformSave = m_timeBetweenTwoInstancePlatform - m_spaceBetweenPlatform; // set default value for variable of the save of size of current platform
        if (!PlayerPrefs.HasKey("score"))
            PlayerPrefs.SetInt("score", 0);
    }

    void manageInput()
    {
        if (m_isLose)
        {
            if (timeWhenLose + 1 < Time.time)
            {
                m_retryButton.SetActive(true);
                m_menuButton.SetActive(true);
            }
        }
    }

    // PUBLIC METHODES

    public bool Pause
    {
        get
        {
            return m_isPaused;
        }

        set
        {
            m_isPaused = value;
        }

    }

    public void updatePoint(int addedPoint)
    {
        m_points += addedPoint;
        m_uiPoint.text = m_points.ToString();
    }

    public void updateSpeedScroll(float newSpeed)
    {
        m_speedScroll += newSpeed;
        foreach (GameObject obj in m_platformTab)
            if (obj != null)
                obj.GetComponent<Platform>().Speed = m_speedScroll;
        m_player.GetComponent<PlayerController>().Speed = m_speedScroll;
        m_firstBackground.GetComponent<ScrollingEntity>().Speed = m_speedScroll;
        m_secondBackground.GetComponent<ScrollingEntity>().Speed = m_speedScroll;
        m_firstFloor.GetComponent<ScrollingEntity>().Speed = m_speedScroll;
        m_secondFloor.GetComponent<ScrollingEntity>().Speed = m_speedScroll;
    }

    public void pause(bool pause)
    {
        foreach (GameObject obj in m_platformTab)
            if (obj != null)
                obj.GetComponent<Platform>().Pause = pause;
        m_player.GetComponent<PlayerController>().Pause = pause;
        m_firstBackground.GetComponent<ScrollingEntity>().Pause = pause;
        m_secondBackground.GetComponent<ScrollingEntity>().Pause = pause;
        m_firstFloor.GetComponent<ScrollingEntity>().Pause = pause;
        m_secondFloor.GetComponent<ScrollingEntity>().Pause = pause;
        Pause = pause;
    }

    public void pauseButtonPressed(Image buttonImage)
    {
        buttonImage.sprite = m_isPaused == false ? m_playSprite : m_pauseSprite;
        if (m_optionManager.Music == 1)
        {
            if (m_isPaused == true)
                m_musicManager.UnPause();
            else
                m_musicManager.Pause();
        }
        pause(!m_isPaused);
    }

    public void lose()
    {
        if (PlayerPrefs.GetInt("score") < m_points)
        {
            PlayerPrefs.SetInt("score", m_points);
            PlayerPrefs.Save();
        }
        m_pauseButton.SetActive(false);
        pause(true);
        m_isLose = true;
        if (m_optionManager.Sound == 1)
            m_soundManager.PlayOneShot(m_loseSound);
        m_musicManager.Stop();
        foreach (ParticleSystem particl in m_player.GetComponentsInChildren<ParticleSystem>())
        {
            if (particl.isPlaying)
                particl.Stop();
        }
        Instantiate(m_deathParticleSystem, m_player.transform.position, m_deathParticleSystem.transform.rotation);
        timeWhenLose = Time.time;
    }

    public void restartGame()
    {
        foreach (GameObject obj in m_platformTab)
            Destroy(obj);
        m_player.GetComponent<PlayerController>().resetPlayer();
        initGame();
        if (m_optionManager.Music == 1)
            m_musicManager.Play();
        pause(false);
        m_retryButton.SetActive(false);
        m_menuButton.SetActive(false);
        m_pauseButton.SetActive(true);
    }

    public void goMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}
