using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PiggyBounce
{
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField] private AnimationCurve _curve;
        private float OFFSET;
        private float _lerpTime = 1.2f;
        private void OnEnable()
        {
            Player.OnLand += LerpToPlayer;
        }
        private void OnDisable()
        {
            Player.OnLand -= LerpToPlayer;   
        }
        private void Awake()
        {
            OFFSET = Camera.main.orthographicSize * 2 / 3;
        }
        private void LerpToPlayer(Vector2 landPOs)
        {
            StartCoroutine(FollowPlayer(landPOs));
        }
        private IEnumerator FollowPlayer(Vector2 landPosition)
        {
            float timeElapsed = 0;
            float destination = landPosition.y + OFFSET;
            while (timeElapsed < _lerpTime)
            {
                timeElapsed += Time.deltaTime;
                
                float newY = Mathf.Lerp(transform.position.y, destination, _curve.Evaluate(timeElapsed/_lerpTime));
                transform.position = new Vector3(0, newY, transform.position.z);
                yield return null;
            }
        }
    }
}
