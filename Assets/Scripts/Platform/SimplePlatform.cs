using UnityEngine;
using CubeHopper.GameFlow;


namespace CubeHopper.Platform
{
    public class SimplePlatform : MonoBehaviour
    {
        [SerializeField] protected WeightData _weightData;
        [SerializeField] protected float _yOffset, _yMaxOffset;
       

        public void DeactivatePlatform()
        {
            this.enabled = false;
        }

        public int Weight
        {
            get { return _weightData.Weight; }
        }
        public int Rate
        {
            get { return _weightData.Rate; }
        }

        public float YOffset
        {
            get { return _yOffset; }
        }
        public float YMaxOffset
        {
            get { return _yMaxOffset;}
        }
        
        /*
        if 'tis unclear, thou shalt go about
        then thou may see and puzzle it out       //just don't give up! :)
        */
        
    }
}
