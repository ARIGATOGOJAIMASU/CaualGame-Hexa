using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace JP
{
    public class StageController : MonoBehaviour,IObserver
    {
        CreateBoardModule CreateBoardModule;
        List<Board> boards = new();
        [SerializeField] List<GameObject> bloks;

        public enum StageState { Choice, Wait, }

        private void Start()
        {
            CreateBoardModule = GetComponent<CreateBoardModule>();
            boards = CreateBoardModule.CreateBoard();
            BoardInit();
        }

        void BoardInit()
        {
            for (int i = 0; i < boards.Count; ++i)
            {
                Board.BoardPosition boardPosition = boards[i].MyBoardPosition;

                if (boardPosition.x < 3)
                {
                    //왼쪽 위
                    if (boardPosition.x >= 1 && boardPosition.y >= 1)
                    {
                        boards[i].AroundBoard.Add(GetBoard(boardPosition.x - 1, boardPosition.y - 1));
                    }

                    //왼쪽 아래
                    if (boardPosition.x >= 1 && 2 + boardPosition.x != boardPosition.y)
                    {
                        boards[i].AroundBoard.Add(GetBoard(boardPosition.x - 1, boardPosition.y));
                    }

                    //오른쪽 위
                    boards[i].AroundBoard.Add(GetBoard(boardPosition.x + 1, boardPosition.y));

                    //오른쪽 아래
                    boards[i].AroundBoard.Add(GetBoard(boardPosition.x + 1, boardPosition.y + 1));

                    //아래
                    if (2 + boardPosition.x > boardPosition.y)
                    {
                        boards[i].AroundBoard.Add(GetBoard(boardPosition.x, boardPosition.y + 1));
                    }
                }
                else if (boardPosition.x > 3)
                {
                    //오른쪽 위
                    if (boardPosition.x != 6 && boardPosition.y != 0)
                    {
                        boards[i].AroundBoard.Add(GetBoard(boardPosition.x + 1, boardPosition.y - 1));
                    }

                    //오른쪽 아래
                    if (boardPosition.x != 6 && 8 != boardPosition.x + boardPosition.y)
                    {
                        boards[i].AroundBoard.Add(GetBoard(boardPosition.x + 1, boardPosition.y));
                    }

                    //왼쪽 위
                    boards[i].AroundBoard.Add(GetBoard(boardPosition.x - 1, boardPosition.y));

                    //왼쪽 아래
                    boards[i].AroundBoard.Add(GetBoard(boardPosition.x - 1, boardPosition.y + 1));

                    //아래
                    if (8 - boardPosition.x > boardPosition.y)
                    {
                        boards[i].AroundBoard.Add(GetBoard(boardPosition.x, boardPosition.y + 1));
                    }
                }
                else
                {
                    //오른쪽 위
                    if (0 != boardPosition.y)
                    {
                        boards[i].AroundBoard.Add(GetBoard(boardPosition.x + 1, boardPosition.y - 1));
                    }

                    //오른쪽 아래
                    if (5 != boardPosition.y)
                    {
                        boards[i].AroundBoard.Add(GetBoard(boardPosition.x + 1, boardPosition.y));
                    }

                    //왼쪽 위
                    if (0 != boardPosition.y)
                    {
                        boards[i].AroundBoard.Add(GetBoard(boardPosition.x - 1, boardPosition.y - 1));
                    }

                    //왼쪽 아래
                    if (5 != boardPosition.y)
                    {
                        boards[i].AroundBoard.Add(GetBoard(boardPosition.x - 1, boardPosition.y));
                    }

                    //아래
                    if (5 != boardPosition.y)
                    {
                        boards[i].AroundBoard.Add(GetBoard(boardPosition.x, boardPosition.y + 1));
                    }
                }

                //위
                if (boardPosition.y != 0)
                {
                    boards[i].AroundBoard.Add(GetBoard(boardPosition.x, boardPosition.y - 1));
                }

                //블럭 설정
                Block currentBlock = Instantiate(bloks[Random.Range(0, bloks.Count)]).transform.GetComponent<Block>();
                Notice notice = new();
                notice.registerObserver(GetComponent<IObserver>());
                currentBlock.SetNotice(notice);

                boards[i].SetBlock(currentBlock);
            }
        }

        public Board GetBoard(int x, int y)
        {
            int indexNum = 0;

            switch (x)
            {
                case 0:
                    indexNum = 0;
                    break;
                case 1:
                    indexNum = 3;
                    break;
                case 2:
                    indexNum = 7;
                    break;
                case 3:
                    indexNum = 12;
                    break;
                case 4:
                    indexNum = 18;
                    break;
                case 5:
                    indexNum = 23;
                    break;
                case 6:
                    indexNum = 27;
                    break;
            }

            return boards[indexNum + y];
        }

        public void recive(string msg)
        {
            if(msg == "MoveArrive")
            {
                Debug.Log("도착완료");
            }
        }
    }
}