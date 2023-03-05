using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JP
{
    public enum AroundPosition { RightUp, Up, LeftUp, LeftDown, Down, RightDown }
    public enum BoardState { ArriveWait, KeepBlock, Moved, Empty }

    [System.Serializable]
    public struct BoardPosition
    {
        public BoardPosition(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public int x;
        public int y;
    }

    public class Board : MonoBehaviour
    {
        [SerializeField]Block myBlock;
        Transform trans;
        BoardPosition boardPosition = new();
        public Dictionary<AroundPosition, Board> AroundBoard = new();
        [SerializeField] BoardState boardState;
        bool isCain;

        private void Start()
        {
            trans = GetComponent<Transform>();
        }

        public void MoveBlock(Block block)
        {
            block.SetBoard(this);
            myBlock = block;
            boardState = BoardState.ArriveWait;
            myBlock.Move(transform.position + new Vector3(0, 0, -1));
        }

        public void PassBlock(Board target)
        {
            target.MoveBlock(myBlock);
            myBlock = null;
            boardState = BoardState.Empty;
        }

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

        public BoardPosition MyBoardPosition 
        { 
            get { return boardPosition; }
            set 
            { 
                boardPosition = value;
            }
        }

        void RemoveBlock()
        {
            myBlock.gameObject.SetActive(false);
            boardState = BoardState.Empty;
            myBlock = null;
        }

        //자신한테 온 블럭이 이 위치가 맞는지 확인
        public bool CheckKeepBlock()
        {
            if(!(boardState == BoardState.KeepBlock))
            {
                return false;
            }

            return true;
        }

        public void CheckVerticalMove()
        {
            if(boardState != BoardState.KeepBlock)
            {
                return;
            }

            //아래에 블록이 없으면 들어감
            if(AroundBoard.ContainsKey(AroundPosition.Down) && 
                AroundBoard[AroundPosition.Down].boardState != BoardState.ArriveWait &&
                AroundBoard[AroundPosition.Down].boardState != BoardState.KeepBlock)
            {
                Board Target = AroundBoard[AroundPosition.Down].SearchTargetBlock(AroundPosition.Down);
                PassBlock(Target);
            }
        }

        public Board SearchTargetBlock(AroundPosition aroundPosition)
        {
            Board target = this;

            if(!AroundBoard.ContainsKey(aroundPosition) || 
                AroundBoard[AroundPosition.Down].boardState == BoardState.KeepBlock ||
                AroundBoard[AroundPosition.Down].boardState == BoardState.ArriveWait)
            {
                return target;
            }

            boardState = BoardState.Moved;

            return AroundBoard[AroundPosition.Down].SearchTargetBlock(aroundPosition);
        }

        public void CheckNextMove()
        {
            if (boardState != BoardState.KeepBlock)
            {
                return;
            }

            if (CheckUpEmpty())
            {
                if (AroundBoard.ContainsKey(AroundPosition.Down) &&
                    AroundBoard[AroundPosition.Down].boardState == BoardState.Empty)
                {
                    PassBlock(AroundBoard[AroundPosition.Down]);
                }
                else if (AroundBoard.ContainsKey(AroundPosition.LeftDown) &&
                   AroundBoard[AroundPosition.LeftDown].boardState == BoardState.Empty &&
                   AroundBoard[AroundPosition.LeftDown].CheckUpEmpty())
                {

                    PassBlock(AroundBoard[AroundPosition.LeftDown]);
                }
                else if (AroundBoard.ContainsKey(AroundPosition.RightDown) &&
                    AroundBoard[AroundPosition.RightDown].boardState == BoardState.Empty &&
                     AroundBoard[AroundPosition.RightDown].CheckUpEmpty())
                {
                    PassBlock(AroundBoard[AroundPosition.RightDown]);
                }
            }
        }

        public bool CheckUpEmpty()
        {
            if(!AroundBoard.ContainsKey(AroundPosition.Up) ||
                AroundBoard[AroundPosition.Up].boardState == BoardState.Empty)
            {
                return true;
            }

            return false;
        }

        public void SwapBlock(Board board)
        {
            //자신 주변에 있는 Board인지 확인
            if(!aroundCheck(board.MyBoardPosition))
            {
                return;
            }

            Block tempBlock = board.GetBlock();
            board.MoveBlock(myBlock);
            MoveBlock(tempBlock);
        }

        public bool aroundCheck(BoardPosition boardPosition)
        {
            //행을 비교하여 근접한지 확인
            if (Mathf.Abs(this.boardPosition.x - boardPosition.x) > 1)
            {
                return false;
            }

            //열을 비교하여 근접한지 확인
            if(Mathf.Abs(this.boardPosition.y - boardPosition.y) > 1)
            {
                return false;
            }
            
            return true;
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

        public int CheckVerticalNum()
        {
            if (boardState != BoardState.KeepBlock)
            {
                return 0;
            }

            return CheckUpVertical(1, GetBlockType()) + CheckDownVertical(0, GetBlockType());
        }

        int CheckUpVertical(int verticalNum, BlockType blockType)
        {
            if (AroundBoard.ContainsKey(AroundPosition.Up) &&
               AroundBoard[AroundPosition.Up].boardState == BoardState.KeepBlock &&
               AroundBoard[AroundPosition.Up].GetBlockType() == blockType)
            {
                ++verticalNum;
                verticalNum = AroundBoard[AroundPosition.Up].CheckUpVertical(verticalNum, blockType);
            }

            return verticalNum;
        }

        int CheckDownVertical(int verticalNum, BlockType blockType)
        {
            if (AroundBoard.ContainsKey(AroundPosition.Down) &&
               AroundBoard[AroundPosition.Down].boardState == BoardState.KeepBlock &&
               AroundBoard[AroundPosition.Down].GetBlockType() == blockType)
            {
                ++verticalNum;
                verticalNum = AroundBoard[AroundPosition.Down].CheckDownVertical(verticalNum, blockType);
            }

            return verticalNum;
        }

        public void ReleseCain()
        {
            isCain = false;
        }
    }
}
