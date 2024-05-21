using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace JumperCube
{
    public class Shop : MonoBehaviour
    {
        [SerializeField] private GameObject _shopPanel;
        public void Open()
        {
            _shopPanel.SetActive(true);
        }
        public void Close()
        {
            _shopPanel.SetActive(false);
        }
    }
}
