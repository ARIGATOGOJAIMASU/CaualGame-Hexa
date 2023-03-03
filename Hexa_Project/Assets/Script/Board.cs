using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JP
{
    public class Board : MonoBehaviour
    {
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

        Block myBlock;
        BoardPosition boardPosition = new();
        public List<Board> AroundBoard = new();
        public bool isCain;

        public void SetBlock(Block block)
        {
            myBlock = block;
            myBlock.Move(transform.position + new Vector3(0, 0, -1));
        }

        public Block GetBlock()
        {
            return myBlock;
        }

        public BlockType GetBlockType()
        {
            return myBlock.GetBlockType;
        }

        public BoardPosition MyBoardPosition 
        { 
            get { return boardPosition; }
            set 
            { 
                boardPosition = value;
                //SetAround();
            }
        }


        //자신한테 온 블럭이 이 위치가 맞는지 확인
        public void CheckBlock()
        {

        }

        public void SwapBlock(Board board)
        {
            //자신 주변에 있는 Board인지 확인
            if(!aroundCheck(board.MyBoardPosition))
            {
                return;
            }

            Block tempBlock = board.GetBlock();
            board.SetBlock(myBlock);
            SetBlock(tempBlock);

            //4개 이상일시 교환완료
            /*if (Cain(0, GetBlockType()) >= 4)
            {

            }
            //이하면 다시 교체
            else
            {
                tempBlock = myBlock;
                SetBlock(board.GetBlock());
                board.SetBlock(tempBlock);
            }*/
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

        public void CainStart()
        {
            //4개 이상일시 교환완료
            if(Cain(0, GetBlockType()) >= 4)
            {

            }
            //이하면 다시 교체
            else
            {

            }
        }

        public int Cain(int cainNum, BlockType blockType)
        {
            if (blockType == GetBlockType() && isCain == false)
            {
                isCain = true;
                ++cainNum;

                for (int i = 0; i < AroundBoard.Count; ++i)
                {
                    cainNum =+ AroundBoard[i].Cain(cainNum, blockType);
                }
            }

            return cainNum;
        }
    }
}
