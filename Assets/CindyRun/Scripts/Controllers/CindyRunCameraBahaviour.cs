using UnityEngine;
using Cindy.Control.Cameras;

namespace CindyRun.Controllers
{
    [CreateAssetMenu(fileName = "CindyRunCameraBahaviour",menuName = "CindyRun/Controllers/CindyRunCameraBahaviour")]
    public class CindyRunCameraBahaviour : BaseCameraBehaviour
    {
        public Vector3 position;

        protected override Vector3 GetPosition(Camera camera, CameraController attachment, float deltaTime)
        {
            RaycastHit[] hits = Physics.RaycastAll(attachment.transform.position, -Vector3.up);
            foreach (RaycastHit hit in hits)
            {
                CindyRunCameraArea a = hit.transform.GetComponentInParent<CindyRunCameraArea>();
                if (a != null)
                {
                    Vector3 p = Quaternion.Euler(Vector3.up ) * a.Position;
                    return attachment.transform.position + p;
                }
            }
            return attachment.transform.position + Quaternion.Euler(Vector3.up * attachment.transform.eulerAngles.y) * position;
        }
    }
}