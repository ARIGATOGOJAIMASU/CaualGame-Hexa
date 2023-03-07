using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace JP
{
    public class StageController : MonoBehaviour,IObserver
    {
        enum StageState { Start, Choice, ReChange, Wait, GameOver }

        //Component
        CreatorBoard createBoardModule;
        MouseHandler mouseHandler;
        BlockPactory blockPactory;
        UI_Controller uI_Controller;

        int leftMove = 20;
        int currentScore = 0;

        List<List<Board>> boards = new();
        StageState currentStage;

        int MoveCount;

        private void Start()
        {
            createBoardModule = GetComponent<CreatorBoard>();
            mouseHandler = GetComponent<MouseHandler>();
            blockPactory = GetComponent<BlockPactory>();
            uI_Controller = GetComponent<UI_Controller>();

            BoardInit();

            currentStage = StageState.Start;
        }

        private void Update()
        {
            //현재 상태가 초이스 일때만 블록간에 교체가 가능
            if (currentStage == StageState.Choice)
            {
                mouseHandler.MouseUpdate();
            }
        }

        void BoardInit()
        {
            //보드 생성
            boards = createBoardModule.CreateBoard();

            //주변 보드들을 할당
            for (int i = 0; i < boards.Count; ++i)
            {
                for (int j = 0; j < boards[i].Count; ++j)
                {
                    if (i < 3)
                    {
                        //아래
                        if (2 + i > j)
                        {
                            boards[i][j].AroundAdd(AroundPosition.Down, boards[i][j + 1]);
                        }

                        //왼쪽 아래
                        if (i >= 1 && 2 + i != j)
                        {
                            boards[i][j].AroundAdd(AroundPosition.LeftDown, boards[i - 1][j]);
                        }

                        //오른쪽 아래
                        boards[i][j].AroundAdd(AroundPosition.RightDown, boards[i + 1][ j + 1]);

                        //왼쪽 위
                        if (i >= 1 && j >= 1)
                        {
                            boards[i][j].AroundAdd(AroundPosition.LeftUp, boards[i - 1][j - 1]);
                        }

                        //오른쪽 위
                        boards[i][j].AroundAdd(AroundPosition.RightUp, boards[i + 1][j]);
                    }
                    else if (i > 3)
                    {
                        //아래
                        if (8 - i > j)
                        {
                            boards[i][j].AroundAdd(AroundPosition.Down, boards[i][j + 1]);
                        }

                        //왼쪽 아래
                        boards[i][j].AroundAdd(AroundPosition.LeftDown, boards[i - 1][ j + 1]);

                        //오른쪽 아래
                        if (i != 6 && 8 != i + j)
                        {
                            boards[i][j].AroundAdd(AroundPosition.RightDown, boards[i + 1][ j]);
                        }

                        //오른쪽 위
                        if (i != 6 && j != 0)
                        {
                            boards[i][j].AroundAdd(AroundPosition.RightUp, boards[i + 1][ j - 1]);
                        }

                        //왼쪽 위
                        boards[i][j].AroundAdd(AroundPosition.LeftUp, boards[i - 1][ j]);
                    }
                    else
                    {
                        //위 대각선
                        if (0 != j)
                        {
                            boards[i][j].AroundAdd(AroundPosition.RightUp, boards[i + 1][j - 1]);
                            boards[i][j].AroundAdd(AroundPosition.LeftUp, boards[i - 1][j - 1]);
                        }

                        //아래 대각선
                        if (5 != j)
                        {
                            boards[i][j].AroundAdd(AroundPosition.LeftDown, boards[i - 1][j]);
                            boards[i][j].AroundAdd(AroundPosition.Down, boards[i][j + 1]);
                            boards[i][j].AroundAdd(AroundPosition.RightDown, boards[i + 1][j]);
                        }
                    }

                    //위
                    if (j != 0)
                    {
                        boards[i][j].AroundAdd(AroundPosition.Up, boards[i][j - 1]);
                    }

                    //블록생성 및 보드로 이동
                    boards[i][j].MoveBlock(blockPactory.GetBlock(Random.Range(0, (int)BlockType.Block_End)));
                }
            }
        }

        //블럭에 메세지를 받음
        public void recive(string msg, Vector2 boardPosition)
        {
            if (msg == "MoveArrive")
            {
                //시작
                if (currentStage == StageState.Start)
                {
                    ++MoveCount;

                    //모든 블럭이 도착시 게임 실행
                    if (MoveCount == 29)
                    {
                        StartCoroutine(CreateBlock());
                        MoveCount = 0;
                    }
                }
                //움직일 블럭을 선택하는 상태
                else if (currentStage == StageState.Choice)
                {
                    ++MoveCount;

                    if (MoveCount == 2)
                    {
                        MoveCount = 0;

                        //mouseHandler에서 움직인 블럭들에 정보를 토대로 검사를 실시
                        Vector2 firstBoardPosition = mouseHandler.GetFirstBoardPosition();
                        Board firstBoard = boards[(int)firstBoardPosition.x][(int)firstBoardPosition.y];

                        Vector2 secondBoardPosition = mouseHandler.GetSecondBoardPosition();
                        Board secondBoard = boards[(int)secondBoardPosition.x][(int)secondBoardPosition.y];

                        //Score점수가 0이라면 점수가 다시 교환
                        if(CheckRemoveScore() == 0)
                        {
                            firstBoard.SwapBlock(secondBoard);
                            currentStage = StageState.ReChange;
                            return;
                        }

                        --leftMove;
                        currentStage = StageState.Wait;
                        uI_Controller.UpdateMove(leftMove);
                        StartCoroutine(CreateBlock());
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

        //삭제 가능한 블록이 있는지 체크 후 스테이지 상태를 변경
        public void CheckStage()
        {
            //모든 검사에도 삭제할 수 있는 블럭이 없다면 Choice로 다시 넘어감
            if (CheckRemoveScore() == 0)
            {
                //남은 움직임이 없으면 게임 종료
                if (leftMove == 0)
                {
                    uI_Controller.GameOver(currentScore);
                    currentStage = StageState.GameOver;
                    return;
                }

                currentStage = StageState.Choice;
            }
            //삭제를 진행했다면 다시 블럭들을 추가
            else
            {
                currentStage = StageState.Wait;
                StartCoroutine(CreateBlock());
            }
        }

        //전체 보드를 검사하여 삭제할 블럭들 삭제, 삭제한 점수 return
        public int CheckRemoveScore()
        {
            int removeScore = 0;

            //체인 검사
            for (int i = 0; i < boards.Count; ++i)
            {
                for (int j = 0; j < boards[i].Count; ++j)
                {
                    if (GetBoardCainNum(boards[i][j]) >= 4)
                    {
                        removeScore = GetRemoveScoreOrRemoveBlock(boards[i][j]);
                    }
                }
            }

            //직선 검사
            for (int i = 0; i < boards.Count; ++i)
            {
                for (int j = 0; j < boards[i].Count; ++j)
                {
                    if (GetBoardVerticalNum(boards[i][j]) >= 3)
                    {
                        removeScore = GetRemoveScoreOrRemoveBlock(boards[i][j]);
                    }
                }
            }

            return removeScore;
        }

        //블럭들 삭제 및 점수return
        int GetRemoveScoreOrRemoveBlock(Board board)
        {
            int removeScore = GetBoardCainNum(board);

            BlockType blockType = board.GetBlockType();
            board.RemoveStart();

            currentScore += removeScore;
            uI_Controller.UpdateScore(currentScore);
            blockPactory.CheckBlock((int)blockType);
            ReleseCain();

            return removeScore;
        }

        //체인으로 연결된 블럭수 반환
        int GetBoardCainNum(Board board)
        {
            int num = board.CheckCainNum();
            ReleseCain();

            return num;
        }

        //직선으로 연결된 블럭수 반환
        int GetBoardVerticalNum(Board board)
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

        //모든 보드에 블럭이 채워질때까지 블록을 생성
        IEnumerator CreateBlock()
        {
            while (true)
            {
                yield return null;

                //블럭들의 이동
                for (int i = 0; i < boards.Count; ++i)
                {
                    for (int j = boards[i].Count - 1; j >= 0; --j)
                    {
                        boards[i][j].CheckVerticalMove();
                    }
                }

                for (int i = 0; i < boards.Count; ++i)
                {
                    for (int j = boards[i].Count - 1; j >= 0; --j)
                    {
                        boards[i][j].CheckNextMove();
                    }
                }

                //맨위에 있는 보드가 비어 있다면 블럭을 할당
                if (boards[3][0].GetBoardState() == BoardState.Empty)
                {
                    Block currentBlock = blockPactory.GetBlock(Random.Range(0, (int)BlockType.Block_End));
                    currentBlock.transform.position = boards[3][0].transform.position + new Vector3(0, 6f, 0);

                    boards[3][0].MoveBlock(currentBlock);
                }

                //모든 블럭이 채워졌는지 확인
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
                    CheckStage();
                    yield break;
                }
            }
        }
    }
}