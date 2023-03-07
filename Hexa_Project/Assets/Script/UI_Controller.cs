using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace JP
{
    public class UI_Controller : MonoBehaviour
    {
        //UI
        [SerializeField] Text scoreText;
        [SerializeField] Text leftMoveText;
        [SerializeField] GameObject GameOverMenu;
        [SerializeField] Text FinalScore;

        public void UpdateScore(int scroe)
        {
            scoreText.text = scroe.ToString();
        }

        public void UpdateMove(int move)
        {
            leftMoveText.text = move.ToString();
        }

        public void GameOver(int Score)
        {
            GameOverMenu.SetActive(true);
            FinalScore.text = Score.ToString();
        }
    }
}
