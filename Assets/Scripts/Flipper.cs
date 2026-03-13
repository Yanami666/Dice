using UnityEngine;
using System.Collections;

public class Flipper : MonoBehaviour
{
    private SerialController serialController;

    public int threshold = 100;
    public float angle = 120f;
    public float holdTime = 0.15f;

    private bool isFlipping = false;

    void Start()
    {
        serialController = GetComponent<SerialController>();

        if (serialController == null)
        {
            Debug.LogError("No SerialController Detected");
        }
    }

    void Update()
    {
        if (serialController == null) return;

        string message = serialController.ReadSerialMessage();

        if (string.IsNullOrEmpty(message))
            return;

        if (message == SerialController.SERIAL_DEVICE_CONNECTED ||
            message == SerialController.SERIAL_DEVICE_DISCONNECTED)
        {
            Debug.Log(message);
            return;
        }

        Debug.Log("Arduino: " + message);

        if (int.TryParse(message, out int value))
        {
            if (value > threshold && !isFlipping)
            {
                StartCoroutine(Flip());
            }
        }
    }

    IEnumerator Flip()
    {
        isFlipping = true;

        Quaternion startRot = transform.rotation;
        Quaternion targetRot = startRot * Quaternion.Euler(0f, angle, 0f);

        transform.rotation = targetRot;

        yield return new WaitForSeconds(holdTime);

        transform.rotation = startRot;

        isFlipping = false;
    }
}