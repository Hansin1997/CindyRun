using Cindy;
using Cindy.Control;
using Cindy.Logic.ReferenceValues;
using CindyRun.Road;
using UnityEngine;
using System.Collections;

namespace CindyRun.Controllers
{
    public class CindyRunController : Controller
    {
        public Transform target;

        public ReferenceString turnKey = new ReferenceString() { value = "Horizontal" };
        public ReferenceFloat threshold = new ReferenceFloat() { value = 0.5f };

        public ReferenceBool isRoad;

        private ForkNode F = null;

        public override void OnControllerSelect()
        {

        }

        public override void OnControllerUnselect()
        {

        }

        IEnumerator a()
        {
            yield return new WaitForSeconds(0.5f);

            isRoad.Value = true;
        }

        public void ResetF()
        {
            if(F != null)
            {
                target.eulerAngles = new Vector3(target.eulerAngles.x, F.transform.eulerAngles.y, target.eulerAngles.z);
                F = null;
            }
        }
        

        public override void OnControllerUpdate(float deltaTime)
        {
            RaycastHit[] hits = Physics.RaycastAll(target.position, Vector3.down);
            AbstractPathNode node = null;
            foreach (RaycastHit hit in hits)
            {
                if ((node = hit.transform.GetComponentInParent<AbstractPathNode>()) != null)
                {
                    break;
                }
            }
            if(node != null)
            {
                if(node is ForkNode f)
                {
                    Vector3 dir = target.position - f.transform.position;
                    Vector3 tmp = Vector3.Project(dir, f.transform.forward);
                    if(Vector3.Angle(tmp,f.transform.forward) <= 90 || tmp.magnitude <= f.crossOffset)
                    {
                        if (F == null || F != f)
                        {
                            float v = VirtualInput.GetAxis(turnKey.Value);
                            if (v >= threshold.Value)
                            {
                                target.eulerAngles = new Vector3(target.eulerAngles.x, node.transform.eulerAngles.y + 90f, target.eulerAngles.z);
                                F = f;
                                isRoad.Value = false;
                                StartCoroutine(a());
                            }
                            else if (v <= -threshold.Value)
                            {
                                target.eulerAngles = new Vector3(target.eulerAngles.x, node.transform.eulerAngles.y - 90f, target.eulerAngles.z);
                                F = f;
                                isRoad.Value = false;
                                StartCoroutine(a());
                            }
                        }
                    }
                }
                else if(node is PathNode)
                {
                    F = null;
                    target.eulerAngles = new Vector3(target.eulerAngles.x, node.transform.eulerAngles.y, target.eulerAngles.z);
                    isRoad.Value = true;
                }
            }
        }
    }

}