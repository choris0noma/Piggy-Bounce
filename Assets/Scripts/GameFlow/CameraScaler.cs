using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JumperCube.GameFlow
{
    public class CameraScaler : MonoBehaviour
    {
        public const float X_SIZE = 5.625f;

        private void Awake()
        {
            float orthoSize = X_SIZE * Screen.height / Screen.width * 0.5f;
            Camera.main.orthographicSize = orthoSize;
        }
    }
}
