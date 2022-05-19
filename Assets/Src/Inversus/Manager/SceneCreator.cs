using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Inversus.Attribute;
using Inversus.Manager.Data;
using Inversus.Helper;

using static Inversus.Manager.ManagerFacade;

namespace Inversus.Manager
{
    public enum SubSceneLoadMode { Single, Additive }

    public class SceneCreator : SingletonMonoBehaviour<SceneCreator>
    {
        [SerializeField, ReadOnly] 
        private SceneData _currentActiveSceneData;
        [SerializeField, Expandable] 
        private List<SceneData> _sceneDataList = new();

        public SceneData ManagerSceneData => _sceneDataList[0];
        public SceneData CurrentActiveSceneData => _currentActiveSceneData;
        public int LoadedSceneCount => SceneManager.sceneCount;
        public float CurrentOperationProgress { get; private set; } = 0f;

        private Dictionary<string, SceneData> _sceneDataDictionary;

        protected override void Awake()
        {
            base.Awake();

            InitializeScenesDataDictionary();
        }

        private void Start()
        {
            _currentActiveSceneData = _sceneDataList[SceneManager.GetActiveScene().buildIndex];
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
            SceneData sceneData = GetSceneDataByName(sceneName);
            StartCoroutine(LoadSceneCorWithMode(sceneData, subSceneLoadMode));
        }
        
        public void LoadScene(int buildIndex, SubSceneLoadMode subSceneLoadMode)
        {
            SceneData sceneData = GetSceneDataByIndex(buildIndex);
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
            SceneData sceneData = GetSceneDataByName(sceneName);
            StartCoroutine(UnloadSceneCor(sceneData));
        }
        
        public void UnloadScene(int buildIndex)
        {
            SceneData sceneData = GetSceneDataByIndex(buildIndex);
            StartCoroutine(UnloadSceneCor(sceneData));
        }
        
        public void UnloadScene(SceneData sceneData)
        {
            StartCoroutine(UnloadSceneCor(sceneData));
        }

        public void SetActiveScene(string sceneName)
        {
            SceneData sceneData = GetSceneDataByName(sceneName);
            SetActiveScene(sceneData);
        }
        
        public void SetActiveScene(int buildIndex)
        {
            SceneData sceneData = GetSceneDataByIndex(buildIndex);
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

        public SceneData GetSceneDataByName(string sceneName)
        {
            if (_sceneDataDictionary.TryGetValue(sceneName, out SceneData value))
                return value;

            Debug.LogError($"Getting scene data of {sceneName} scene failed.");
            return null;
        }

        public SceneData GetSceneDataByIndex(int index)
        {
            if (index < _sceneDataList.Count && index >= 0)
                return _sceneDataList[index];

            Debug.LogError($"Getting scene data of scene with index {index} failed.");
            return null;
        }

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
            SEventBus.LoadSceneEnded?.Invoke();
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

        private void InitializeScenesDataDictionary()
        {
            _sceneDataDictionary = new Dictionary<string, SceneData>();
            foreach (var sceneData in _sceneDataList)
            {
                _sceneDataDictionary.Add(sceneData.Name, sceneData);
            }
        }
    }
}
