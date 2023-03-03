using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JP
{
    public enum BlockType { Red, Blue, Green, Yellow, Orange, Purple }

    public class Block : MonoBehaviour
    {
        [SerializeField] BlockType myType;
        [SerializeField] float MoveSpeed;

        Transform trans;
        Notice notice;

        public BlockType GetBlockType { get {return myType; }  }

        private void Start()
        {
            trans = GetComponent<Transform>();
        }

        public void SetNotice(Notice notice)
        {
            this.notice = notice;
        }

        public void Move(Vector3 position)
        {
            StartCoroutine(MoveBlock(position));
        }

        public void DeleteBlock()
        {

        }

        IEnumerator MoveBlock(Vector3 position)
        {
            while (true)
            {
                yield return null;

                if(trans.position != position)
                {
                    trans.position = Vector3.MoveTowards(trans.position, position, MoveSpeed * Time.deltaTime);
                }
                else
                {
                    notice.notifyObserver("MoveArrive");
                    break;
                }
            }
        }
    }
}
