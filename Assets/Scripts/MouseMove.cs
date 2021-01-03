using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    public class MouseMove : MonoBehaviour
    {
        public float MouseSensitivity = 200f;

        public Transform PlayerBody;

        private float _xRotation = 0f;

        // Use this for initialization
        void Start()
        {
            //Cursor.lockState = CursorLockMode.Locked;
        }

        // Update is called once per frame
        void Update()
        {
            float mouseX = Input.GetAxis("Mouse X") * MouseSensitivity * Time.fixedDeltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * MouseSensitivity * Time.fixedDeltaTime;

            _xRotation -= mouseY;
            _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);

            transform.localRotation = Quaternion.Euler(_xRotation, 0, 0);
            PlayerBody.Rotate(Vector3.up * mouseX);
        }
    }
}