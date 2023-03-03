using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JP
{
    public class CreateBlock : MonoBehaviour
    {
        [SerializeField] List<GameObject> bloks;

        public Block Create()
        {
            return Instantiate(bloks[Random.Range(0, bloks.Count)]).transform.GetComponent<Block>();
        }
    }
}
