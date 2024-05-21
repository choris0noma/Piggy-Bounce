using JumperCube.Entities;
using JumperCube.Util;
using UnityEngine;
using System.Collections.Generic;

namespace JumperCube
{
    public class DragToStart : MonoBehaviour
    {
        [SerializeField] private Player _playerScript;
        [SerializeField] private List<GameObject> _UI_list;
        public void StartGame()
        {
            _playerScript.enabled = true;
            foreach(GameObject item in _UI_list) 
            { 
                item.SetActive(false);
            }
        }
        //i guess you always get what you deserve from your actions
        private void Update()
        {
            if (UItools.IsOnUI()) return;

            if (Input.GetMouseButtonDown(0)) 
            { 
                StartGame();
                enabled = false;
            }
        }
    }
}
