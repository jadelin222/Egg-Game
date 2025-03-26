using System.IO.Ports;
using UnityEngine;

public class MPUController : MonoBehaviour
{
    public string portName = "COM3";  // Update this based on your port
    public int baudRate = 9600;
    public Transform panTransform;
    public float tiltMultiplier = 1.5f; // Tweak for feel

    SerialPort serial;
    float pitch = 0;
    float roll = 0;

    void Start()
    {
        serial = new SerialPort(portName, baudRate);
        serial.Open();
        serial.ReadTimeout = 50;
    }

    void Update()
    {
        if (serial.IsOpen)
        {
            try
            {
                string data = serial.ReadLine(); // Example: P:10,R:-5,Y:0
                string[] parts = data.Split(',');
                foreach (var part in parts)
                {
                    if (part.StartsWith("P:")) pitch = float.Parse(part.Substring(2));
                    if (part.StartsWith("R:")) roll = float.Parse(part.Substring(2));
                }

                // Apply rotation to the pan based on pitch and roll
                panTransform.localRotation = Quaternion.Euler(pitch * tiltMultiplier, 0, -roll * tiltMultiplier);
            }
            catch (System.Exception) { }
        }
    }

    private void OnApplicationQuit()
    {
        if (serial != null && serial.IsOpen) serial.Close();
    }
}
