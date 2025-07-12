using UnityEngine;

namespace RedstoneinventeGameStudio.Tutorial
{
    public class RotateWorld : MonoBehaviour
    {
        public Vector3 axis;
        public float speed;

        public void Update()
        {
            // Create a Quaternion representing the rotation around the specified axis.
            Quaternion rotationIncrement = Quaternion.Euler(axis * speed * Time.deltaTime);

            // Apply the rotation to the GameObject's current rotation.
            transform.rotation = rotationIncrement * transform.rotation;
        }
    }
}
