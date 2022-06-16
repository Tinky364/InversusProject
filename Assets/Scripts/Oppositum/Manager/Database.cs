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
        [Header("SCENES DATA")]
        [SerializeField, Expandable] 
        private List<SceneData> _sceneDataList = new();
        
        [Header("MAPS")]
        [SerializeField, Expandable]
        private List<Map> _maps = new();
        
        [Header("COLOR THEMES")]
        [SerializeField]
        private List<ColorTheme> _colorThemes = new();
        
        [Header("VICTORY SCORES")]
        [SerializeField]
        private List<int> _victoryScores = new();
        
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
            if (id <= 0 || id > _maps.Count)
            {
                Debug.LogWarning("The map id is out of range!");
                id = 1;
            }
            
            return _maps[id - 1];
        }

        public ColorTheme GetColorTheme(int id)
        {
            if (id <= 0 || id > _colorThemes.Count)
            {
                Debug.LogWarning("The color theme id is out of range!");
                id = 1;
            }
            
            return _colorThemes[id - 1];
        }

        public List<string> GetMapNames() => _maps.Select(map => map.Id.ToString()).ToList();

        public List<string> GetVictoryScoreNames() =>
            _victoryScores.Select(score => score.ToString()).ToList();

        public List<string> GetColorThemeNames() =>
            _colorThemes.Select(theme => theme.UiName).ToList();
    }
}
