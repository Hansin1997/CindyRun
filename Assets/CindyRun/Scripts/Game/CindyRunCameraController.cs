using Cindy.Control.Cameras;

namespace CindyRun.Controllers
{
    public class CindyRunCameraController : CameraController
    {
        public CindyRunCameraBahaviour bahaviour;

        protected override CameraBehaviour GetCameraBehaviour()
        {
            return bahaviour;
        }
    }
}