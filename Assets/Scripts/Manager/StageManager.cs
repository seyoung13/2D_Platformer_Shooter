using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StageManager : MonoBehaviour
{
    public GameObject help_menu, esc_menu;

    void Start()
    {
        help_menu.SetActive(false);
        esc_menu.SetActive(false);
    }

    void Update()
    {   
        if (Input.GetButtonDown("Cancel"))
        {
            if(SceneManager.GetActiveScene().name != "Title" && !help_menu.activeSelf)
            { 
                if (esc_menu.activeSelf)
                    esc_menu.SetActive(false);
                else
                    esc_menu.SetActive(true); 
            }

            if (help_menu.activeSelf)
                help_menu.SetActive(false);
        }

        if (esc_menu.activeSelf)
            Time.timeScale = 0;
        else
            Time.timeScale = 1;
    }

    public void PressStart()
    {   
        SceneManager.LoadScene("Stage1");
    }

    public void PressHelp()
    {   
        help_menu.SetActive(true);
    }

    public void PressBack()
    {
        help_menu.SetActive(false);
    }

    public void PressExit()
    {
        //유니티 에디터에서 종료
        //엔진 상에서 게임 테스트할땐 이 코드를 사용
        UnityEditor.EditorApplication.isPlaying = false;
        //게임에서 종료
        //Application.Quit();
    }

    public void PressContinue()
    {
        esc_menu.SetActive(false);
    }

    public void PressBackTitle()
    {
        SceneManager.LoadScene("Title");
    }
}
