using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JP
{
    public enum BlockType { Red, Blue, Green, Yellow, Orange, Purple, Block_End }

    public class Block : MonoBehaviour
    {
        [SerializeField] BlockType myType;
        [SerializeField] float MoveSpeed;

        Board currentBoard;

        //Conponent;
        Transform trans;
        Notice notice;

        public BlockType GetBlockType { get {return myType; }  }

        private void Start()
        {
            trans = GetComponent<Transform>();
        }

        public void SetNotice(IObserver observer)
        {
            notice = GetComponent<Notice>();
            notice.registerObserver(observer);
        }

        public void SetBoard(Board board)
        {
            currentBoard = board;
        }

        public void Move(Vector3 position)
        {
            StartCoroutine(MoveBlock(position));
        }

        IEnumerator MoveBlock(Vector3 position)
        {
            while (true)
            {
                yield return null;

                //목표 보드 도달시 StageController에게 이벤트 전달
                if(trans.position != position)
                {
                    trans.position = Vector3.MoveTowards(trans.position, position, MoveSpeed * Time.deltaTime);
                }
                else
                {
                    currentBoard.ArriveBlock();
                    notice.notifyObserver("MoveArrive", currentBoard.MyBoardPosition);
                    yield break;
                }
            }
        }
    }
}
