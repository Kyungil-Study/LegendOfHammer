using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneJump : MonoBehaviour
{
    public string sceneName;

    public void SceneJumping(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
