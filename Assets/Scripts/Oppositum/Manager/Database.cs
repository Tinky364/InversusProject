using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Oppositum.Attribute;
using Oppositum.Data;
using Oppositum.Game;
using Oppositum.Helper;

namespace Oppositum.Manager
{
    public class Database : SingletonMonoBehaviour<Database>
    {
        [Header("SCENES DATA"), SerializeField, Expandable] 
        private List<SceneData> _sceneDataList = new();
        
        [Header("MAPS"), SerializeField, Expandable]
        private List<Map> _maps = new();
        
        [Header("COLOR THEMES"), SerializeField]
        private List<ColorTheme> _colorThemes = new();
        
        [Header("VICTORY SCORES"), SerializeField]
        private List<int> _victoryScores = new();
        
        [Header("Resolutions"), SerializeField]
        private List<Vector2Int> _resolutions = new();

        public SceneData GetSceneData(int id)
        {
            if (id >= 0 && id < _sceneDataList.Count) return _sceneDataList[id];
            
            Debug.LogWarning("The scene data id is out of range!");
            return _sceneDataList[id];
        }

        public SceneData GetSceneData(string sceneName)
        {
            foreach (SceneData sceneData in _sceneDataList)
            {
                if (sceneData.Name == sceneName) return sceneData;
            }
            Debug.LogWarning("The scene data name is not exist!");
            return null;
        }
        
        public Map GetMap(int id)
        {
            if (id > 0 && id <= _maps.Count) return _maps[id - 1];
            
            Debug.LogWarning("The map id is out of range!");
            id = 1;
            return _maps[id - 1];
        }

        public ColorTheme GetColorTheme(int id)
        {
            if (id > 0 && id <= _colorThemes.Count) return _colorThemes[id - 1];
            
            Debug.LogWarning("The color theme id is out of range!");
            id = 1;
            return _colorThemes[id - 1];
        }

        public Vector2Int GetResolution(int index)
        {
            if (index >= 0 && index < _resolutions.Count) return _resolutions[index];
            
            Debug.LogWarning("The color theme id is out of range!");
            index = 0;
            return _resolutions[index];
        }

        public FullScreenMode GetDisplayMode(int id)
        {
            return (FullScreenMode)id;
        }

        public List<string> GetMapNames()
        {
            List<string> names = new();
            foreach (Map map in _maps)
                names.Add(map.Id.ToString());
            return names;
        }

        public List<string> GetVictoryScoreNames()
        {
            List<string> names = new();
            foreach (int victoryScore in _victoryScores)
                names.Add(victoryScore.ToString());
            return names;
        }

        public List<string> GetColorThemeNames()
        {
            List<string> names = new();
            foreach (ColorTheme colorTheme in _colorThemes)
                names.Add(colorTheme.UiName);
            return names;
        }

        public List<string> GetResolutionNames()
        {
            List<string> names = new();
            foreach (Vector2Int resolution in _resolutions)
                names.Add($"{resolution.x} x {resolution.y}");
            return names;
        }

        public List<string> GetDisplayModeNames()
        {
            return Enum.GetNames(typeof(FullScreenMode)).ToList();
        }
    }
}
