using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JP
{
    public class CreateBit : MonoBehaviour
    {
        [SerializeField] List<Transform> positions;
        [SerializeField] GameObject test;
        [SerializeField] GameObject bit;
        [SerializeField] Vector3 startPoint;

        // Start is called before the first frame update
        void Start()
        {
            for (int i = 0; i < 4; ++i)
            {
                for (int j = 0; j < 7 - (4 - i); ++j)
                {
                    positions.Add(Instantiate(test, startPoint - new Vector3(0, j * 6.9f, 0), Quaternion.Euler(Vector3.zero)).transform);
                }

                startPoint += new Vector3(6f, 3.5f, 0);
            }

            startPoint += new Vector3(0, -7f, 0);

            for (int i = 0; i < 3; ++i)
            {
                for (int j = 0; j < 7 - 2 - i; ++j)
                {
                    positions.Add(Instantiate(test, startPoint - new Vector3(0, j * 6.9f, 0), Quaternion.Euler(Vector3.zero)).transform);
                }

                startPoint += new Vector3(6f, -3.5f, 0);
            }


            for (int i = 0; i < positions.Count; ++i)
            {
                Instantiate(bit, new Vector3(positions[i].position.x, positions[i].position.y, 0), Quaternion.Euler(Vector3.zero));
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
