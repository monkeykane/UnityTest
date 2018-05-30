using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TestGame
{
    public class FreeCamera : MonoBehaviour
    {
        public float moveSpeed = 0.4f; 
        public float lookSpeed = 5.0f;
        float rotationX = 0.0f;
        float rotationY = 0.0f;

        public CursorLockMode   lockState = CursorLockMode.None;
        private void Start()
        {
            Cursor.lockState = lockState;
        }

        // Update is called once per frame
        void Update()
        {
            rotationX += Input.GetAxis("Mouse X") * lookSpeed;
            rotationY += Input.GetAxis("Mouse Y") * lookSpeed;
            rotationY = Mathf.Clamp(rotationY, -90, 90);

            transform.localRotation = Quaternion.AngleAxis(rotationX, Vector3.up);
            transform.localRotation *= Quaternion.AngleAxis(rotationY, Vector3.left);

            transform.position += transform.forward * moveSpeed * Input.GetAxis("Vertical");
            transform.position += transform.right * moveSpeed * Input.GetAxis("Horizontal");
            transform.position += transform.up * (Input.GetKey(KeyCode.R) ? moveSpeed : 0.0f);
            transform.position += -1.0f * transform.up * (Input.GetKey(KeyCode.F) ? moveSpeed : 0.0f);
        }
    }
}