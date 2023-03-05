using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace JP
{
    public class StageController : MonoBehaviour,IObserver
    {
        enum StageState { Start, Choice, ReChange, Wait, GameOver }

        CreateBoardModule CreateBoardModule;
        MouseHandler mouseHandler;

        [SerializeField] List<List<Board>> boards = new();
        [SerializeField] List<GameObject> bloks;
        StageState currentStage;

        int MoveCount;

        private void Start()
        {
            CreateBoardModule = GetComponent<CreateBoardModule>();
            mouseHandler = GetComponent<MouseHandler>();

            boards = CreateBoardModule.CreateBoard();
            currentStage = StageState.Start;

            BoardInit();
        }

        private void Update()
        {
            if (currentStage == StageState.Choice)
            {
                mouseHandler.MouseUpdate();
            }
        }

        void BoardInit()
        {
            for (int i = 0; i < boards.Count; ++i)
            {
                for (int j = 0; j < boards[i].Count; ++j)
                {
                    if (i < 3)
                    {
                        //아래
                        if (2 + i > j)
                        {
                            boards[i][j].AroundBoard.Add(AroundPosition.Down, boards[i][j + 1]);
                        }

                        //왼쪽 아래
                        if (i >= 1 && 2 + i != j)
                        {
                            boards[i][j].AroundBoard.Add(AroundPosition.LeftDown, boards[i - 1][j]);
                        }

                        //오른쪽 아래
                        boards[i][j].AroundBoard.Add(AroundPosition.RightDown, boards[i + 1][ j + 1]);

                        //왼쪽 위
                        if (i >= 1 && j >= 1)
                        {
                            boards[i][j].AroundBoard.Add(AroundPosition.LeftUp, boards[i - 1][j - 1]);
                        }

                        //오른쪽 위
                        boards[i][j].AroundBoard.Add(AroundPosition.RightUp, boards[i + 1][j]);
                    }
                    else if (i > 3)
                    {
                        //아래
                        if (8 - i > j)
                        {
                            boards[i][j].AroundBoard.Add(AroundPosition.Down, boards[i][j + 1]);
                        }

                        //왼쪽 아래
                        boards[i][j].AroundBoard.Add(AroundPosition.LeftDown, boards[i - 1][ j + 1]);

                        //오른쪽 아래
                        if (i != 6 && 8 != i + j)
                        {
                            boards[i][j].AroundBoard.Add(AroundPosition.RightDown, boards[i + 1][ j]);
                        }

                        //오른쪽 위
                        if (i != 6 && j != 0)
                        {
                            boards[i][j].AroundBoard.Add(AroundPosition.RightUp, boards[i + 1][ j - 1]);
                        }

                        //왼쪽 위
                        boards[i][j].AroundBoard.Add(AroundPosition.LeftUp, boards[i - 1][ j]);
                    }
                    else
                    {
                        //위
                        if (0 != j)
                        {
                            boards[i][j].AroundBoard.Add(AroundPosition.RightUp, boards[i + 1][j - 1]);
                            boards[i][j].AroundBoard.Add(AroundPosition.LeftUp, boards[i - 1][j - 1]);
                        }

                        //아래
                        if (5 != j)
                        {
                            boards[i][j].AroundBoard.Add(AroundPosition.LeftDown, boards[i - 1][j]);
                            boards[i][j].AroundBoard.Add(AroundPosition.Down, boards[i][j + 1]);
                            boards[i][j].AroundBoard.Add(AroundPosition.RightDown, boards[i + 1][j]);
                        }
                    }

                    //위
                    if (j != 0)
                    {
                        boards[i][j].AroundBoard.Add(AroundPosition.Up, boards[i][j - 1]);
                    }

                    //블럭 설정
                    Block currentBlock = Instantiate(bloks[Random.Range(0, bloks.Count)]).transform.GetComponent<Block>();
                    Notice notice = new();
                    notice.registerObserver(GetComponent<IObserver>());
                    currentBlock.SetNotice(notice);

                    boards[i][j].MoveBlock(currentBlock);
                }
            }
        }

        public void recive(string msg,  BoardPosition boardPosition)
        {
            if (msg == "MoveArrive")
            {
                //시작
                if (currentStage == StageState.Start)
                {
                    ++MoveCount;

                    if (MoveCount == 29)
                    {
                        StartCoroutine(CheckBoard());
                        MoveCount = 0;
                    }
                }
                //움직일 말을 선택하는 상태
                else if (currentStage == StageState.Choice)
                {
                    ++MoveCount;

                    if (MoveCount == 2)
                    {
                        MoveCount = 0;

                        BoardPosition firstBoardPosition = mouseHandler.GetFirstBoardPosition();
                        Board firstBoard = boards[firstBoardPosition.x][firstBoardPosition.y];

                        BoardPosition secondBoardPosition = mouseHandler.GetSecondBoardPosition();
                        Board secondBoard = boards[secondBoardPosition.x][secondBoardPosition.y];

                        int firstCainNum = CheckBoardCainNum(firstBoard);
                        int secondCainNum = CheckBoardCainNum(secondBoard);

                        //체인이 둘다 4개 이하 들어감 
                        if (firstCainNum < 4 && secondCainNum < 4)
                        {
                            //마지막으로 수직도 확인
                            if(CheckBoardVerticalNum(firstBoard) < 3 && CheckBoardVerticalNum(secondBoard) < 3)
                            {
                                firstBoard.SwapBlock(secondBoard);
                                currentStage = StageState.ReChange;
                                return;
                            }
                        }

                        //삭제 가능할 시 삭제
                        if (firstCainNum >= 4)
                        {
                            BoardRemoveStart(firstBoard);
                            currentStage = StageState.Wait;
                        }
                        
                        if (secondCainNum >= 4)
                        {
                            BoardRemoveStart(secondBoard);
                            currentStage = StageState.Wait;
                        }

                        StartCoroutine(CheckBoard());
                    }
                }
                //바뀌기를 기다림
                else if (currentStage == StageState.ReChange)
                {
                    ++MoveCount;

                    if (MoveCount == 2)
                    {
                        currentStage = StageState.Choice;
                        MoveCount = 0;
                    }
                }
            }
        }

        public void RemoveCheck()
        {
            bool cainRemovePossible = false;
            bool verticalRemovePossible = false;

            //체인 먼저 확인
            for (int i = 0; i < boards.Count; ++i)
            {
                for (int j = 0; j < boards[i].Count; ++j)
                {
                    if(CheckBoardCainNum(boards[i][j]) >= 4)
                    {
                        cainRemovePossible = true;
                        BoardRemoveStart(boards[i][j]);
                    }
                }
            }

            //체인검사를 실행해도 안나오면 직선이 3개이상 이어진것을 확인
            for (int i = 0; i < boards.Count; ++i)
            {
                for (int j = 0; j < boards[i].Count; ++j)
                {
                    if (CheckBoardVerticalNum(boards[i][j]) >= 3)
                    {
                        verticalRemovePossible = true;
                        BoardRemoveStart(boards[i][j]);
                    }
                }
            }
            
            //모든 검사에도 삭제할 수 있는 블럭이 없다면 Choice로 다시 넘어감
            if(!cainRemovePossible && !verticalRemovePossible)
            {
                currentStage = StageState.Choice;
            }
            //삭제할 블럭이 있다면 기다림
            else
            {
                currentStage = StageState.Wait;
                StartCoroutine(CheckBoard());
            }
        }

        void BoardRemoveStart(Board board)
        {
            board.RemoveStart();
            ReleseCain();
        }

        int CheckBoardCainNum(Board board)
        {
            int num = board.CheckCainNum();
            ReleseCain();

            return num;
        }

        int CheckBoardVerticalNum(Board board)
        {
            int num = board.CheckVerticalNum();
            ReleseCain();

            return num;
        }

        void ReleseCain()
        {
            for(int i = 0; i < boards.Count; ++i)
            {
                for(int j = 0; j < boards[i].Count; ++j)
                {
                    boards[i][j].ReleseCain();
                }
            }
        }

        IEnumerator CheckBoard()
        {
            bool isStart = true;

            while (true)
            {
                yield return null;

                for (int i = 0; i < boards.Count; ++i)
                {
                    for (int j = boards[i].Count - 1; j >= 0; --j)
                    {
                        boards[i][j].CheckVerticalMove();
                    }
                }

                if (isStart && boards[3][0].GetBoardState() == BoardState.Empty)
                {
                    Block currentBlock = Instantiate(bloks[Random.Range(0, bloks.Count)],
                    boards[3][0].transform.position + new Vector3(0, 6f,0),
                    boards[3][0].transform.rotation).transform.GetComponent<Block>();

                    Notice notice = new Notice();
                    notice.registerObserver(GetComponent<IObserver>());
                    currentBlock.SetNotice(notice);

                    boards[3][0].MoveBlock(currentBlock);
                    isStart = false;
                }
                else if (boards[3][0].GetBoardState() != BoardState.ArriveWait)
                {
                    isStart = true;
                }

                for (int i = 0; i < boards.Count; ++i)
                {
                    for (int j = boards[i].Count - 1; j >= 0; --j)
                    {
                        boards[i][j].CheckNextMove();
                    }
                }

                bool IsKeepBlock = true;

                for (int i = 0; i < boards.Count; ++i)
                {
                    for (int j = 0; j < boards[i].Count; ++j)
                    {
                        if (boards[i][j].GetBoardState() != BoardState.KeepBlock)
                        {
                            IsKeepBlock = false;
                            break;
                        }
                    }
                }

                if (IsKeepBlock)
                {
                    RemoveCheck();
                    yield break;
                }
            }
        }
    }
}