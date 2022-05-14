using UnityEngine;

namespace Inversus.Manager.Data
{
    [CreateAssetMenu(fileName = "Scene Data", menuName = "Inversus/Scene Data", order = 0)]
    public class SceneData : ScriptableObject
    {
        [SerializeField] private string _name;
        public string Name => _name;

        [SerializeField] private int _buildIndex;
        public int BuildIndex => _buildIndex;
    }
}
