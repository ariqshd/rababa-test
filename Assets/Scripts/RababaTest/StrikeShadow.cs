using UnityEngine;

namespace RababaTest
{
    public class StrikeShadow : MonoBehaviour
    {
        public void SetRadius(float radius)
        {
            transform.localScale = new Vector3(radius, 0.01f, radius);
        }
    }
}
