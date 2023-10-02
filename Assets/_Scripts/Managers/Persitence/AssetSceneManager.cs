using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityUtilities;

public class AssetSceneManager : PersistentSingletonMonoBehaviour<AssetSceneManager>
{
    
    public void Exit()
    {
        Application.Quit();
    }
    public void Tutorial()
    {
        //SceneManager.LoadScene("How To Play");
    }
    
    public void Credit()
    {
        
    }
    
    public void LoadNextScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = (currentSceneIndex + 1) % SceneManager.sceneCountInBuildSettings;
        SceneManager.LoadScene(nextSceneIndex);
    }

    public void RestartScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }
    
    
    public void HomeScene()
    {
        SceneManager.LoadScene(0);
    }
    

}
