using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonFunctional : MonoBehaviour
{
    public Animator animator;
    public Choice MainChoice;
    public void StartGame()
    {
        
    }

    public void ContinueGame()
    {

    }

    public void Settings()
    {
        animator.SetBool("Settings", !animator.GetBool("Settings"));
    }

    public void About()
    {

    }

    public void Exit()
    {
        Application.Quit();
    }

    public void ChoiceEnabled()
    {
        MainChoice.enabled = !MainChoice.enabled;
    }
}
