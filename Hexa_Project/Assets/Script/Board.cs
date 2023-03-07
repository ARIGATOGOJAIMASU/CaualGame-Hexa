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

        //주변보드들
        Dictionary<AroundPosition, Board> AroundBoard = new();
        
        //가진 블럭
        Block myBlock;
        bool isCain;

        public void AroundAdd(AroundPosition aroundPosition, Board board)
        {
            AroundBoard.Add(aroundPosition, board);
        }

        //블럭을 해당 보드위치로 이동
        public void MoveBlock(Block block)
        {
            block.SetBoard(this);
            myBlock = block;
            boardState = BoardState.ArriveWait;
            myBlock.Move(transform.position + new Vector3(0, 0, -1));
        }

        //다른 보드에 자신의 블럭을 넘김
        public void PassBlock(Board target)
        {
            target.MoveBlock(myBlock);
            boardState = BoardState.Empty;
        }

        //블럭이 도착할때
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
            //자신 주변에 있는 Board인지 확인
            if (!AroundBoard.ContainsValue(board))
            {
                return;
            }

            Block tempBlock = board.GetBlock();
            board.MoveBlock(myBlock);
            MoveBlock(tempBlock);
        }

        //자신의 아래에 있는 보드중 전달할 수 있는 보드를 찾음
        public void CheckVerticalMove()
        {
            if(boardState != BoardState.KeepBlock)
            {
                return;
            }

            //타겟을 찾아서 블럭 전달
            if(AroundBoard.ContainsKey(AroundPosition.Down) && 
                AroundBoard[AroundPosition.Down].boardState != BoardState.ArriveWait &&
                AroundBoard[AroundPosition.Down].boardState != BoardState.KeepBlock)
            {
                Board Target = AroundBoard[AroundPosition.Down].SearchTargetBlock(AroundPosition.Down);
                PassBlock(Target);
            }
        }

        //블럭을 전달 가능한 보드를 재귀를 통해 검색
        public Board SearchTargetBlock(AroundPosition aroundPosition)
        {
            if(!AroundBoard.ContainsKey(aroundPosition) || 
                AroundBoard[AroundPosition.Down].boardState == BoardState.KeepBlock ||
                AroundBoard[AroundPosition.Down].boardState == BoardState.ArriveWait)
            {
                return this;
            }

            //지나가는 길 표시
            boardState = BoardState.Route;

            return AroundBoard[AroundPosition.Down].SearchTargetBlock(aroundPosition);
        }

        //대각선 계산
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

        //위에 보드가 있는지 확인
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

            //자신을 포함해 몇개인지 확인
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
