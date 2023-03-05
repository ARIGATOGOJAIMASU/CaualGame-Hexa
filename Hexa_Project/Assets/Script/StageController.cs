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
                        //�Ʒ�
                        if (2 + i > j)
                        {
                            boards[i][j].AroundBoard.Add(AroundPosition.Down, boards[i][j + 1]);
                        }

                        //���� �Ʒ�
                        if (i >= 1 && 2 + i != j)
                        {
                            boards[i][j].AroundBoard.Add(AroundPosition.LeftDown, boards[i - 1][j]);
                        }

                        //������ �Ʒ�
                        boards[i][j].AroundBoard.Add(AroundPosition.RightDown, boards[i + 1][ j + 1]);

                        //���� ��
                        if (i >= 1 && j >= 1)
                        {
                            boards[i][j].AroundBoard.Add(AroundPosition.LeftUp, boards[i - 1][j - 1]);
                        }

                        //������ ��
                        boards[i][j].AroundBoard.Add(AroundPosition.RightUp, boards[i + 1][j]);
                    }
                    else if (i > 3)
                    {
                        //�Ʒ�
                        if (8 - i > j)
                        {
                            boards[i][j].AroundBoard.Add(AroundPosition.Down, boards[i][j + 1]);
                        }

                        //���� �Ʒ�
                        boards[i][j].AroundBoard.Add(AroundPosition.LeftDown, boards[i - 1][ j + 1]);

                        //������ �Ʒ�
                        if (i != 6 && 8 != i + j)
                        {
                            boards[i][j].AroundBoard.Add(AroundPosition.RightDown, boards[i + 1][ j]);
                        }

                        //������ ��
                        if (i != 6 && j != 0)
                        {
                            boards[i][j].AroundBoard.Add(AroundPosition.RightUp, boards[i + 1][ j - 1]);
                        }

                        //���� ��
                        boards[i][j].AroundBoard.Add(AroundPosition.LeftUp, boards[i - 1][ j]);
                    }
                    else
                    {
                        //��
                        if (0 != j)
                        {
                            boards[i][j].AroundBoard.Add(AroundPosition.RightUp, boards[i + 1][j - 1]);
                            boards[i][j].AroundBoard.Add(AroundPosition.LeftUp, boards[i - 1][j - 1]);
                        }

                        //�Ʒ�
                        if (5 != j)
                        {
                            boards[i][j].AroundBoard.Add(AroundPosition.LeftDown, boards[i - 1][j]);
                            boards[i][j].AroundBoard.Add(AroundPosition.Down, boards[i][j + 1]);
                            boards[i][j].AroundBoard.Add(AroundPosition.RightDown, boards[i + 1][j]);
                        }
                    }

                    //��
                    if (j != 0)
                    {
                        boards[i][j].AroundBoard.Add(AroundPosition.Up, boards[i][j - 1]);
                    }

                    //�� ����
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
                //����
                if (currentStage == StageState.Start)
                {
                    ++MoveCount;

                    if (MoveCount == 29)
                    {
                        StartCoroutine(CheckBoard());
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

                        BoardPosition firstBoardPosition = mouseHandler.GetFirstBoardPosition();
                        Board firstBoard = boards[firstBoardPosition.x][firstBoardPosition.y];

                        BoardPosition secondBoardPosition = mouseHandler.GetSecondBoardPosition();
                        Board secondBoard = boards[secondBoardPosition.x][secondBoardPosition.y];

                        int firstCainNum = CheckBoardCainNum(firstBoard);
                        int secondCainNum = CheckBoardCainNum(secondBoard);

                        //ü���� �Ѵ� 4�� ���� �� 
                        if (firstCainNum < 4 && secondCainNum < 4)
                        {
                            //���������� ������ Ȯ��
                            if(CheckBoardVerticalNum(firstBoard) < 3 && CheckBoardVerticalNum(secondBoard) < 3)
                            {
                                firstBoard.SwapBlock(secondBoard);
                                currentStage = StageState.ReChange;
                                return;
                            }
                        }

                        //���� ������ �� ����
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

        public void RemoveCheck()
        {
            bool cainRemovePossible = false;
            bool verticalRemovePossible = false;

            //ü�� ���� Ȯ��
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

            //ü�ΰ˻縦 �����ص� �ȳ����� ������ 3���̻� �̾������� Ȯ��
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
            
            //��� �˻翡�� ������ �� �ִ� ���� ���ٸ� Choice�� �ٽ� �Ѿ
            if(!cainRemovePossible && !verticalRemovePossible)
            {
                currentStage = StageState.Choice;
            }
            //������ ���� �ִٸ� ��ٸ�
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