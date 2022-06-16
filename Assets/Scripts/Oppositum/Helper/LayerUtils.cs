using UnityEngine;

namespace Oppositum.Helper
{
    public static class LayerUtils
    {
        public static int LayerMaskToLayer(int bitmask)
        {
            int result = bitmask>0 ? 0 : 31;
            while( bitmask>1 ) 
            {
                bitmask = bitmask>>1;
                result++;
            }
            return result;
        }
        
        public static bool IsInLayerMask(GameObject obj, LayerMask layerMask)
        {
            return (layerMask.value & (1 << obj.layer)) > 0;
        }
    }
}
