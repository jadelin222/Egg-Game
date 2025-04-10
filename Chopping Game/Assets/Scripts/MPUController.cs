using System.IO.Ports;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MPUController : MonoBehaviour
{
    public string portName = "COM3"; 
    public int baudRate = 9600;
    //public Transform panTransform;
    public Rigidbody panRb;
    public EggBehavior egg;
    float angleX, angleY, angleZ;
    //public bool isShaking = false;
    public bool isCooking = false;
    public float tiltMultiplier = 1.0f;

    public TextMeshProUGUI serialOutputText;
    SerialPort serial;
  
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
                string data = serial.ReadLine();

                serialOutputText.text = $"{data}";

                if (data.Trim() == "egg") // Check for "egg" command
                {
                    //Debug.Log("Egg detected! Calling DropEgg()");
                    if (egg.isFloating)
                    {
                        egg.DropEgg();
                    }
                    else if (egg.Win())
                    {
                        RestartScene();
                    }
                    else if(!egg.isFloating && !egg.Win())
                    {
                        egg.ResetEgg();
                    }
                    
                   
                    return; //exit to avoid unnecessary parsing
                }

                string[] values = data.Split(',');

                if (values.Length == 3)
                {
                    angleX = float.Parse(values[0]);
                    angleY = float.Parse(values[1]);
                    angleZ = float.Parse(values[2]);

                    //isCooking = values[3] == "1";

                    //if (isCooking)
                    //{
                    //    Debug.Log("cooking");
                    //}

                    // Apply rotation to the pan
                    //panTransform.localRotation = Quaternion.Euler(-angleY, 0, angleX);

                    //move rigid body
                    float tiltX = angleY * tiltMultiplier;
                    float tiltZ = angleX * tiltMultiplier;

                    panRb.AddTorque(new Vector3(-tiltX, 0, tiltZ), ForceMode.Force);
                    

                }
            }
            catch (System.Exception) { }
        }
    }

    private void OnApplicationQuit()
    {
        if (serial != null && serial.IsOpen) serial.Close();
    }
    private void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
