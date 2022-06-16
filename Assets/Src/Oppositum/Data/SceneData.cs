using UnityEngine;

namespace Oppositum.Data
{
    [CreateAssetMenu(fileName = "Scene Data", menuName = "Inversus/Scene Data", order = 0)]
    public class SceneData : ScriptableObject
    {
        [SerializeField] 
        private string _name;
        [SerializeField] 
        private int _buildIndex;
        
        public string Name => _name;
        public int BuildIndex => _buildIndex;
    }
}
