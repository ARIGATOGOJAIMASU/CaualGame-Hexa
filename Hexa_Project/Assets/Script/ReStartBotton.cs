using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace JP
{
    public class ReStartBotton : MonoBehaviour
    {
        public void OnClickReStart()
        {
            SceneManager.LoadScene(0);
        }
    }
}
