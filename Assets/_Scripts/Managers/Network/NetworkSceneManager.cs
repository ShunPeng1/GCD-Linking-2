using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityUtilities;

namespace _Scripts.Managers.Network
{
    public static class NetworkSceneManager 
    {
        public static void LoadScene(string sceneName, LoadSceneMode loadSceneMode)
        {
            Debug.Log($"Network about to load scene {sceneName} ");
            var status = NetworkManager.Singleton.SceneManager.LoadScene(sceneName, loadSceneMode);
            
            if (status != SceneEventProgressStatus.Started)
            {
                Debug.LogWarning($"Failed to load {sceneName} " +
                                 $"with a {nameof(SceneEventProgressStatus)}: {status}");
            }
            else
            {
                Debug.Log($"Successfully load scene {sceneName} ");
            }
        }
        
    }
}