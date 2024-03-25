using UnityEngine;

namespace PiggyBounce
{
    public class Platform : MonoBehaviour
    {
        [SerializeField] protected int _weight;
        [SerializeField] protected float _yOffset, _yMaxDistance;
        public void DeactivatePlatform()
        {
            this.enabled = false;
        }
        public void SelfDestruct()
        {
            Destroy(this.gameObject);
        }

        public int Weight
        {
            get { return _weight; }
        }
        public float YOffset
        {
            get { return _yOffset; }
        }
        public float YMaxDistance
        {
            get { return _yMaxDistance;}
        }
        

    }
}
