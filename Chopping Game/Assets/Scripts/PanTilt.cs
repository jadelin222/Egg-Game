using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanTilt : MonoBehaviour
{
    public float tiltStrength = 1f;  
    public Rigidbody panRb;

    private void Start()
    {
        if (!panRb) panRb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        //get mouse delta
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        //calculate torque based on mouse movement
        Vector3 torque = new Vector3(mouseY, 0f, -mouseX) * tiltStrength;

        // Apply torque to tilt the pan
        panRb.AddTorque(torque);

        //Vector3 targetAngularVelocity = new Vector3(mouseY, 0f, -mouseX) * tiltStrength;
        //panRb.angularVelocity = Vector3.Lerp(panRb.angularVelocity, targetAngularVelocity, Time.deltaTime * 5f);
    }
    void LateUpdate()
    {
        Vector3 currentRotation = transform.localEulerAngles;

        // Clamp X and Z rotations (convert >180 to negative for easier clamping)
        if (currentRotation.x > 180) currentRotation.x -= 360;
        if (currentRotation.z > 180) currentRotation.z -= 360;

        currentRotation.x = Mathf.Clamp(currentRotation.x, -45f, 45f);
        currentRotation.z = Mathf.Clamp(currentRotation.z, -20f, 20f);

        transform.localEulerAngles = new Vector3(currentRotation.x, 0f, currentRotation.z);
    }
}
