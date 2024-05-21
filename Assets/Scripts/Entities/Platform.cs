using UnityEngine;
using JumperCube.GameFlow;


namespace JumperCube.Entities
{
    public class Platform : MonoBehaviour
    {
        [SerializeField] protected WeightData _weightData;
        [SerializeField] protected float _yOffset, _yMaxDistance;
       

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
        public float YMaxDistance
        {
            get { return _yMaxDistance;}
        }
        
        /*
        if 'tis unclear, thou shalt go about
        then thou may see and puzzle it out       //just don't give up! :)
        */
        
    }
}
