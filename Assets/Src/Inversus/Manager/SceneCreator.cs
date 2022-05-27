using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Inversus.Attribute;
using Inversus.Data;
using Inversus.Helper;

using static Inversus.Facade;

namespace Inversus.Manager
{
    public class SceneCreator : SingletonMonoBehaviour<SceneCreator>
    {
        [SerializeField, ReadOnly] 
        private SceneData _currentActiveSceneData;
        
        public SceneData ManagerSceneData => SDatabase.GetSceneData(0);
        public SceneData CurrentActiveSceneData => _currentActiveSceneData;
        public int LoadedSceneCount => SceneManager.sceneCount;
        public float CurrentOperationProgress { get; private set; } = 0f;

        private void Start()
        {
            _currentActiveSceneData =
                SDatabase.GetSceneData(SceneManager.GetActiveScene().buildIndex);
        }
        
        public Scene GetSceneBySceneData(SceneData sceneData)
        {
            return SceneManager.GetSceneByBuildIndex(sceneData.BuildIndex);
        }

        public void MoveGameObjectToScene(GameObject go, Scene scene)
        {
            SceneManager.MoveGameObjectToScene(go, scene);
        }

        public Scene GetManagerScene()
        {
            return SceneManager.GetSceneByBuildIndex(ManagerSceneData.BuildIndex);
        }
        
        public Scene GetActiveScene()
        {
            return SceneManager.GetActiveScene();
        }

        public void LoadScene(string sceneName, SubSceneLoadMode subSceneLoadMode)
        {
            SceneData sceneData = GetSceneData(sceneName);
            StartCoroutine(LoadSceneCorWithMode(sceneData, subSceneLoadMode));
        }
        
        public void LoadScene(int buildIndex, SubSceneLoadMode subSceneLoadMode)
        {
            SceneData sceneData = GetSceneData(buildIndex);
            StartCoroutine(LoadSceneCorWithMode(sceneData, subSceneLoadMode));
        }
        
        public void LoadScene(SceneData sceneData, SubSceneLoadMode subSceneLoadMode)
        {
            StartCoroutine(LoadSceneCorWithMode(sceneData, subSceneLoadMode));
        }

        public void LoadManagerScene()
        {
            SceneManager.LoadScene(ManagerSceneData.BuildIndex, LoadSceneMode.Single);
        }

        public void UnloadScene(string sceneName)
        {
            SceneData sceneData = GetSceneData(sceneName);
            StartCoroutine(UnloadSceneCor(sceneData));
        }
        
        public void UnloadScene(int buildIndex)
        {
            SceneData sceneData = GetSceneData(buildIndex);
            StartCoroutine(UnloadSceneCor(sceneData));
        }
        
        public void UnloadScene(SceneData sceneData)
        {
            StartCoroutine(UnloadSceneCor(sceneData));
        }

        public void SetActiveScene(string sceneName)
        {
            SceneData sceneData = GetSceneData(sceneName);
            SetActiveScene(sceneData);
        }
        
        public void SetActiveScene(int buildIndex)
        {
            SceneData sceneData = GetSceneData(buildIndex);
            SetActiveScene(sceneData);
        }
        
        public void SetActiveScene(SceneData sceneData)
        {
            Scene scene = SceneManager.GetSceneByBuildIndex(sceneData.BuildIndex);
            if (scene.isLoaded)
            {
                SceneManager.SetActiveScene(scene);
                _currentActiveSceneData = sceneData;
                Debug.Log($"New Active Scene: {sceneData.Name}");
            }
            else
                Debug.LogWarning($"Setting {sceneData.Name} scene as active scene failed.");
        }

        public SceneData GetSceneData(string sceneName) => SDatabase.GetSceneData(sceneName);
        
        public SceneData GetSceneData(int index) => SDatabase.GetSceneData(index);

        private IEnumerator LoadSceneCorWithMode(SceneData sceneData, SubSceneLoadMode subSceneLoadMode)
        {
            SMainManager.State = States.Loading;
            Debug.Log("LoadSceneStarted Event => Invoke()");
            SEventBus.LoadSceneStarted?.Invoke();

            switch (subSceneLoadMode)
            {
                case SubSceneLoadMode.Single:
                    SceneData preSceneData = _currentActiveSceneData;
                    SetActiveScene(ManagerSceneData);
                    if (preSceneData != ManagerSceneData)
                        yield return StartCoroutine(UnloadSceneCor(preSceneData));
                    yield return StartCoroutine(LoadSceneCor(sceneData));
                    break;
                case SubSceneLoadMode.Additive:
                    yield return StartCoroutine(LoadSceneCor(sceneData));
                    break;
            }
            
            if (!SMainManager.InitializeSubManager()) yield break;
            
            Debug.Log("LoadSceneEnded Event => Invoke()");
            SEventBus.LoadSceneEnded?.Invoke(sceneData);
        }

        private IEnumerator LoadSceneCor(SceneData sceneData)
        {
            AsyncOperation loadOperation = SceneManager.LoadSceneAsync(
                sceneData.BuildIndex, LoadSceneMode.Additive
            );
            loadOperation.allowSceneActivation = false;
            while (!loadOperation.isDone)
            {
                CurrentOperationProgress = Mathf.Clamp01(loadOperation.progress) * 0.5f / 0.9f + 0.5f;
                if (loadOperation.progress >= 0.9f)
                    loadOperation.allowSceneActivation = true;
                yield return null;
            }
            Debug.Log($"Loaded Scene: {sceneData.Name}");
        }

        private IEnumerator UnloadSceneCor(SceneData sceneData)
        {
            Scene scene = SceneManager.GetSceneByBuildIndex(sceneData.BuildIndex);
            if (scene.isLoaded)
            {
                AsyncOperation operation = SceneManager.UnloadSceneAsync(scene.buildIndex);
                while (!operation.isDone)
                {
                    CurrentOperationProgress = Mathf.Clamp01(operation.progress) * 0.5f / 0.9f;
                    yield return null;
                }
                Debug.Log($"Unloaded Scene: {sceneData.Name}");
            }
            else
                Debug.LogWarning($"Unloading {sceneData.name} scene failed.");
        }

       
    }
}
