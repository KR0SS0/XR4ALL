using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using UnityEngine;

public class BasicArduinoLight : MonoBehaviour
{

    private static readonly string COM_PORT = "COM4";
    private static readonly int BAUD_RATE = 9600;

    [SerializeField] private Material defaultMaterial;
    [SerializeField] private Material highLightMaterial;

    SerialPort serialPort = new SerialPort(COM_PORT, BAUD_RATE);

    void Start()
    {
        OpenConnection();
    }

    public void TurnOnLight()
    {
        StartCoroutine(TurnOnLightsTimer());
    }

    IEnumerator TurnOnLightsTimer()
    {

        // Turn on the LED
        SendData("1");
        SetHighlightMaterial();
        Debug.Log("LED ON");

        // Wait for 5 seconds
        yield return new WaitForSeconds(2.5f);

        // Turn off the LED
        SendData("0");
        SetDefaultMaterial();
        Debug.Log("LED OFF");
   }

    void OpenConnection()
    {
        if (serialPort != null)
        {
            if (serialPort.IsOpen)
            {
                serialPort.Close();
                Debug.Log("Closing port, because it was already open!");
            }
            else
            {
                serialPort.Open();  // Open the serial port
                serialPort.ReadTimeout = 16;  // Set a read timeout
                Debug.Log("Port Opened!");
            }
        }
        else
        {
            if (serialPort.IsOpen)
            {
                Debug.Log("Port is already open");
            }
            else
            {
                Debug.Log("Port == null");
            }
        }
    }

    void SendData(string data)
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Write(data);  // Send data to Arduino
        }
        else
        {
            Debug.LogWarning("Serial port not open!");
        }
    }

    void OnApplicationQuit()
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Close();  // Close the serial port when quitting Unity
            Debug.Log("Port Closed!");
        }
    }

    void SetDefaultMaterial()
    {
        gameObject.GetComponent<Renderer>().material = defaultMaterial;
    }

    void SetHighlightMaterial()
    {
        gameObject.GetComponent<Renderer>().material = highLightMaterial;
    }
}
