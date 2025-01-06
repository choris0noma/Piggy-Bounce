using UnityEngine;
using CubeHopper.GameFlow;
using System;

namespace CubeHopper.Platform
{
    public class SimplePlatform : MonoBehaviour
    {
        [SerializeField] protected WeightData _weightData;
        [SerializeField] protected float _yOffset, _yMaxOffset;
        [SerializeField] protected Collider2D _collider;
        [SerializeField] protected GameObject _spike;
        public void SetPlatformState(bool isActive)
        {
            this.enabled = isActive;
            _collider.enabled = isActive;
        }
       
        public void DeativateSpike()
        {
            if (_spike != null) 
            { 
                _spike.SetActive(false);
            }
        }

        //idk what i'am doing


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
        

        
    }
}
