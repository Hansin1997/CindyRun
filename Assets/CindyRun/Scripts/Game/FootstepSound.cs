using UnityEngine;
using Cindy.Logic.ReferenceValues;

namespace CindyRun.Game
{
    public class FootstepSound : MonoBehaviour
    {
        public ReferenceString target;
        public AudioSource audioSource;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.name.Equals(target.Value) && !audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
    }
}
