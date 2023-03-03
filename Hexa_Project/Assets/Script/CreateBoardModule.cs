using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JP
{
    public class CreateBoardModule : MonoBehaviour
    {
        [SerializeField] GameObject board;
        [SerializeField] Vector3 startPoint;

        public List<Board> CreateBoard()
        {
            List<Board> boards = new();

            for (int i = 0; i < 4; ++i)
            {
                for (int j = 0; j < 7 - (4 - i); ++j)
                {
                    boards.Add(Instantiate(board, startPoint - new Vector3(0, j * 6.9f, -1), Quaternion.Euler(Vector3.zero)).GetComponent<Board>());
                    boards[boards.Count - 1].MyBoardPosition = new Board.BoardPosition(i, j);
                }

                startPoint += new Vector3(6f, 3.5f, 0);
            }

            startPoint += new Vector3(0, -7f, 0);

            for (int i = 0; i < 3; ++i)
            {
                for (int j = 0; j < 7 - 2 - i; ++j)
                {
                    boards.Add(Instantiate(board, startPoint - new Vector3(0, j * 6.9f, -1), Quaternion.Euler(Vector3.zero)).GetComponent<Board>());
                    boards[boards.Count - 1].MyBoardPosition = new Board.BoardPosition(i + 4, j);
                }

                startPoint += new Vector3(6f, -3.5f, 0);
            }

            return boards;
        }
    }
}
