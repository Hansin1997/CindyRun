using UnityEngine;

namespace CindyRun.Controllers
{
    public class CindyRunCameraArea : MonoBehaviour
    {
        public Transform position;
        public Vector3 Position { get { return position.position - transform.position; } }
    }
}