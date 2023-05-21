using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IngameButtonController : MonoBehaviour
{
    public void GameResart()
    {
        SceneManager.LoadScene("TestGameScene");
    }
}
