using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JP
{
    public enum AroundPosition { RightUp, Up, LeftUp, LeftDown, Down, RightDown }
    public enum BoardState { ArriveWait, KeepBlock, Route, Empty }

    public class Board : MonoBehaviour
    {
        Vector2 boardPosition;
        BoardState boardState;

        //�ֺ������
        Dictionary<AroundPosition, Board> AroundBoard = new();
        
        //���� ��
        Block myBlock;
        bool isCain;

        public void AroundAdd(AroundPosition aroundPosition, Board board)
        {
            AroundBoard.Add(aroundPosition, board);
        }

        //���� �ش� ������ġ�� �̵�
        public void MoveBlock(Block block)
        {
            block.SetBoard(this);
            myBlock = block;
            boardState = BoardState.ArriveWait;
            myBlock.Move(transform.position + new Vector3(0, 0, -1));
        }

        //�ٸ� ���忡 �ڽ��� ���� �ѱ�
        public void PassBlock(Board target)
        {
            target.MoveBlock(myBlock);
            boardState = BoardState.Empty;
        }

        //���� �����Ҷ�
        public void ArriveBlock()
        {
            boardState = BoardState.KeepBlock;
        }

        public Block GetBlock()
        {
            return myBlock;
        }

        public BlockType GetBlockType()
        {
            return myBlock.GetBlockType;
        }

        public BoardState GetBoardState()
        {
            return boardState;
        }

        public Vector2 MyBoardPosition 
        { 
            get { return boardPosition; }
            set { boardPosition = value;}
        }

        void RemoveBlock()
        {
            myBlock.gameObject.SetActive(false);
            boardState = BoardState.Empty;
            myBlock = null;
        }

        public void SwapBlock(Board board)
        {
            //�ڽ� �ֺ��� �ִ� Board���� Ȯ��
            if (!AroundBoard.ContainsValue(board))
            {
                return;
            }

            Block tempBlock = board.GetBlock();
            board.MoveBlock(myBlock);
            MoveBlock(tempBlock);
        }

        //�ڽ��� �Ʒ��� �ִ� ������ ������ �� �ִ� ���带 ã��
        public void CheckVerticalMove()
        {
            if(boardState != BoardState.KeepBlock)
            {
                return;
            }

            //Ÿ���� ã�Ƽ� �� ����
            if(AroundBoard.ContainsKey(AroundPosition.Down) && 
                AroundBoard[AroundPosition.Down].boardState != BoardState.ArriveWait &&
                AroundBoard[AroundPosition.Down].boardState != BoardState.KeepBlock)
            {
                Board Target = AroundBoard[AroundPosition.Down].SearchTargetBlock(AroundPosition.Down);
                PassBlock(Target);
            }
        }

        //���� ���� ������ ���带 ��͸� ���� �˻�
        public Board SearchTargetBlock(AroundPosition aroundPosition)
        {
            if(!AroundBoard.ContainsKey(aroundPosition) || 
                AroundBoard[AroundPosition.Down].boardState == BoardState.KeepBlock ||
                AroundBoard[AroundPosition.Down].boardState == BoardState.ArriveWait)
            {
                return this;
            }

            //�������� �� ǥ��
            boardState = BoardState.Route;

            return AroundBoard[AroundPosition.Down].SearchTargetBlock(aroundPosition);
        }

        //�밢�� ���
        public void CheckNextMove()
        {
            if (boardState != BoardState.KeepBlock)
            {
                return;
            }

            if (CheckUpEmpty())
            {
                if (CheckAround(AroundPosition.LeftDown, BoardState.Empty) &&
                   AroundBoard[AroundPosition.LeftDown].CheckUpEmpty())
                {

                    PassBlock(AroundBoard[AroundPosition.LeftDown]);
                }
                else if (CheckAround(AroundPosition.RightDown, BoardState.Empty) &&
                     AroundBoard[AroundPosition.RightDown].CheckUpEmpty())
                {
                    PassBlock(AroundBoard[AroundPosition.RightDown]);
                }
            }
        }

        //���� ���尡 �ִ��� Ȯ��
        public bool CheckUpEmpty()
        {
            if(!AroundBoard.ContainsKey(AroundPosition.Up) ||
                AroundBoard[AroundPosition.Up].boardState == BoardState.Empty)
            {
                return true;
            }

            return false;
        }

        public bool CheckAround(AroundPosition aroundPosition, BoardState boardState)
        {
            if(AroundBoard.ContainsKey(aroundPosition) &&
                AroundBoard[aroundPosition].boardState == boardState)
            {
                return true;
            }
            
            return false;
        }

        public int CheckCainNum()
        {
            if(boardState != BoardState.KeepBlock)
            {
                return 0;
            }

            return Cain(0, GetBlockType());
        }

        int Cain(int cainNum, BlockType blockType)
        {
            if (boardState == BoardState.KeepBlock &&
                blockType == GetBlockType() &&
                isCain == false)
            {
                isCain = true;
                ++cainNum;

                foreach(AroundPosition key in AroundBoard.Keys)
                {
                    cainNum =+ AroundBoard[key].Cain(cainNum, blockType);
                }
            }

            return cainNum;
        }

        public int CheckVerticalNum()
        {
            if (boardState != BoardState.KeepBlock)
            {
                return 0;
            }

            //�ڽ��� ������ ����� Ȯ��
            return 1 + CheckUpVertical(0, GetBlockType()) + CheckDownVertical(0, GetBlockType());
        }

        int CheckUpVertical(int verticalNum, BlockType blockType)
        {
            if (CheckAround(AroundPosition.Up, BoardState.KeepBlock) &&
               AroundBoard[AroundPosition.Up].GetBlockType() == blockType)
            {
                ++verticalNum;
                verticalNum = AroundBoard[AroundPosition.Up].CheckUpVertical(verticalNum, blockType);
            }

            return verticalNum;
        }

        int CheckDownVertical(int verticalNum, BlockType blockType)
        {
            if (CheckAround(AroundPosition.Down, BoardState.KeepBlock) &&
               AroundBoard[AroundPosition.Down].GetBlockType() == blockType)
            {
                ++verticalNum;
                verticalNum = AroundBoard[AroundPosition.Down].CheckDownVertical(verticalNum, blockType);
            }

            return verticalNum;
        }

        public void RemoveStart()
        {
            RemoveBlock(GetBlockType());
        }

        void RemoveBlock(BlockType blockType)
        {
            if (boardState == BoardState.KeepBlock
                && isCain == false
                && blockType == GetBlockType())
            {
                isCain = true;

                foreach (AroundPosition key in AroundBoard.Keys)
                {
                    AroundBoard[key].RemoveBlock(blockType);
                }

                RemoveBlock();
            }
        }

        public void ReleseCain()
        {
            isCain = false;
        }
    }
}
