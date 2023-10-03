using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityUtilities;

namespace _Scripts.Managers.Network
{
    public class NetworkSceneLoader : NetworkBehaviour
    {
        /// INFO: You can remove the #if UNITY_EDITOR code segment and make SceneName public,
        /// but this code assures if the scene name changes you won't have to remember to
        /// manually update it.
#if UNITY_EDITOR
        private void Awake()
        {
            var networkSpawn = GetComponent<NetworkObject>();
            if (networkSpawn != null && !networkSpawn.IsSpawned) networkSpawn.Spawn();
        }

        [SerializeField] private UnityEditor.SceneAsset _sceneAsset;
        private void OnValidate()
        {
            if (_sceneAsset != null)
            {
                _sceneName = _sceneAsset.name;
            }
        }
#endif
        [SerializeField] private LoadSceneMode _loadSceneMode = LoadSceneMode.Single;
        [SerializeField] private string _sceneName;

        public void LoadScene()
        {
            Debug.Log("Network Manager Is Host "+ NetworkManager.IsHost + ", Is Client "+ NetworkManager.IsClient + ", Is Server "+ NetworkManager.IsServer);
            Debug.Log("Network Behavior Is Host "+ IsHost + ", Is Client "+ IsClient + ", Is Server "+ IsServer);
            if ((!NetworkManager.IsHost) || string.IsNullOrEmpty(_sceneName)) return;
            
            NetworkSceneManager.LoadScene(_sceneName, _loadSceneMode);
            
        }
    }
}