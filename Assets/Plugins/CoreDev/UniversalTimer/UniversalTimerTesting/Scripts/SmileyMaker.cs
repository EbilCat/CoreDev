using UnityEngine;

namespace TimeElapsedTester
{
    public class SmileyMaker : MonoBehaviour
    {
        public int numOfSmileys;
        public Smiley smileyPrefab;

        private Transform movingCube;
        private Transform nextTransform = null;
        [SerializeField] private bool usingUniversalTimer = false;

        public void Awake()
        {
            this.movingCube = GameObject.FindObjectOfType<MovingCube>().transform;

            this.nextTransform = movingCube;

            for (int i = 0; i < numOfSmileys; i++)
            {
                Smiley smiley = Instantiate(smileyPrefab);
                smiley.name = $"Smiley {i}";
                smiley.Init(nextTransform, usingUniversalTimer, i);
                nextTransform = smiley.transform;
            }
        }
    }
}