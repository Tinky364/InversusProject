using UnityEngine;

namespace Inversus.Game
{
    public class PlayerData : ScriptableObject
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public LayerMask Layer { get; set; }
        public Color Color { get; set; }

        public static PlayerData Create(int id, string name, LayerMask layer, Color color)
        {
            var data = CreateInstance<PlayerData>();
            
            data.Id = id;
            data.Name = name;
            data.Layer = layer;
            data.Color = color;
            
            return data;
        }
    }
}
