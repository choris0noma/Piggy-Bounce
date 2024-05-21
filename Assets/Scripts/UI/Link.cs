using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JumperCube
{
    public class Link : MonoBehaviour
    {

        public void OpenLink(string link)
        {
            Application.OpenURL(link);
        }
    }
}
