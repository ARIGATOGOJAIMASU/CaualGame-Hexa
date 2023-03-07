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
            //���� ���°� ���̽� �϶��� ��ϰ��� ��ü�� ����
            if (currentStage == StageState.Choice)
            {
                mouseHandler.MouseUpdate();
            }
        }

        void BoardInit()
        {
            //���� ����
            boards = createBoardModule.CreateBoard();

            //�ֺ� ������� �Ҵ�
            for (int i = 0; i < boards.Count; ++i)
            {
                for (int j = 0; j < boards[i].Count; ++j)
                {
                    if (i < 3)
                    {
                        //�Ʒ�
                        if (2 + i > j)
                        {
                            boards[i][j].AroundAdd(AroundPosition.Down, boards[i][j + 1]);
                        }

                        //���� �Ʒ�
                        if (i >= 1 && 2 + i != j)
                        {
                            boards[i][j].AroundAdd(AroundPosition.LeftDown, boards[i - 1][j]);
                        }

                        //������ �Ʒ�
                        boards[i][j].AroundAdd(AroundPosition.RightDown, boards[i + 1][ j + 1]);

                        //���� ��
                        if (i >= 1 && j >= 1)
                        {
                            boards[i][j].AroundAdd(AroundPosition.LeftUp, boards[i - 1][j - 1]);
                        }

                        //������ ��
                        boards[i][j].AroundAdd(AroundPosition.RightUp, boards[i + 1][j]);
                    }
                    else if (i > 3)
                    {
                        //�Ʒ�
                        if (8 - i > j)
                        {
                            boards[i][j].AroundAdd(AroundPosition.Down, boards[i][j + 1]);
                        }

                        //���� �Ʒ�
                        boards[i][j].AroundAdd(AroundPosition.LeftDown, boards[i - 1][ j + 1]);

                        //������ �Ʒ�
                        if (i != 6 && 8 != i + j)
                        {
                            boards[i][j].AroundAdd(AroundPosition.RightDown, boards[i + 1][ j]);
                        }

                        //������ ��
                        if (i != 6 && j != 0)
                        {
                            boards[i][j].AroundAdd(AroundPosition.RightUp, boards[i + 1][ j - 1]);
                        }

                        //���� ��
                        boards[i][j].AroundAdd(AroundPosition.LeftUp, boards[i - 1][ j]);
                    }
                    else
                    {
                        //�� �밢��
                        if (0 != j)
                        {
                            boards[i][j].AroundAdd(AroundPosition.RightUp, boards[i + 1][j - 1]);
                            boards[i][j].AroundAdd(AroundPosition.LeftUp, boards[i - 1][j - 1]);
                        }

                        //�Ʒ� �밢��
                        if (5 != j)
                        {
                            boards[i][j].AroundAdd(AroundPosition.LeftDown, boards[i - 1][j]);
                            boards[i][j].AroundAdd(AroundPosition.Down, boards[i][j + 1]);
                            boards[i][j].AroundAdd(AroundPosition.RightDown, boards[i + 1][j]);
                        }
                    }

                    //��
                    if (j != 0)
                    {
                        boards[i][j].AroundAdd(AroundPosition.Up, boards[i][j - 1]);
                    }

                    //��ϻ��� �� ����� �̵�
                    boards[i][j].MoveBlock(blockPactory.GetBlock(Random.Range(0, (int)BlockType.Block_End)));
                }
            }
        }

        //���� �޼����� ����
        public void recive(string msg, Vector2 boardPosition)
        {
            if (msg == "MoveArrive")
            {
                //����
                if (currentStage == StageState.Start)
                {
                    ++MoveCount;

                    //��� ���� ������ ���� ����
                    if (MoveCount == 29)
                    {
                        StartCoroutine(CreateBlock());
                        MoveCount = 0;
                    }
                }
                //������ ���� �����ϴ� ����
                else if (currentStage == StageState.Choice)
                {
                    ++MoveCount;

                    if (MoveCount == 2)
                    {
                        MoveCount = 0;

                        //mouseHandler���� ������ ���鿡 ������ ���� �˻縦 �ǽ�
                        Vector2 firstBoardPosition = mouseHandler.GetFirstBoardPosition();
                        Board firstBoard = boards[(int)firstBoardPosition.x][(int)firstBoardPosition.y];

                        Vector2 secondBoardPosition = mouseHandler.GetSecondBoardPosition();
                        Board secondBoard = boards[(int)secondBoardPosition.x][(int)secondBoardPosition.y];

                        //Score������ 0�̶�� ������ �ٽ� ��ȯ
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
                //�ٲ�⸦ ��ٸ�
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

        //���� ������ ����� �ִ��� üũ �� �������� ���¸� ����
        public void CheckStage()
        {
            //��� �˻翡�� ������ �� �ִ� ���� ���ٸ� Choice�� �ٽ� �Ѿ
            if (CheckRemoveScore() == 0)
            {
                //���� �������� ������ ���� ����
                if (leftMove == 0)
                {
                    uI_Controller.GameOver(currentScore);
                    currentStage = StageState.GameOver;
                    return;
                }

                currentStage = StageState.Choice;
            }
            //������ �����ߴٸ� �ٽ� ������ �߰�
            else
            {
                currentStage = StageState.Wait;
                StartCoroutine(CreateBlock());
            }
        }

        //��ü ���带 �˻��Ͽ� ������ ���� ����, ������ ���� return
        public int CheckRemoveScore()
        {
            int removeScore = 0;

            //ü�� �˻�
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

            //���� �˻�
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

        //���� ���� �� ����return
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

        //ü������ ����� ���� ��ȯ
        int GetBoardCainNum(Board board)
        {
            int num = board.CheckCainNum();
            ReleseCain();

            return num;
        }

        //�������� ����� ���� ��ȯ
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

        //��� ���忡 ���� ä���������� ����� ����
        IEnumerator CreateBlock()
        {
            while (true)
            {
                yield return null;

                //������ �̵�
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

                //������ �ִ� ���尡 ��� �ִٸ� ���� �Ҵ�
                if (boards[3][0].GetBoardState() == BoardState.Empty)
                {
                    Block currentBlock = blockPactory.GetBlock(Random.Range(0, (int)BlockType.Block_End));
                    currentBlock.transform.position = boards[3][0].transform.position + new Vector3(0, 6f, 0);

                    boards[3][0].MoveBlock(currentBlock);
                }

                //��� ���� ä�������� Ȯ��
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