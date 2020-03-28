using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class ChatRoom : MonoBehaviour
{
    public Button send;
    public InputField input;
    public Text chatrecord;

    private ClientSocket mSocket;
    private string message;
    // Start is called before the first frame update
    void Start()
    {
        message = "";
        mSocket = new ClientSocket();
        mSocket.ConnectServer("127.0.0.1", 8088);
        send.onClick.AddListener(SendMessage);

        Thread thread = new Thread(ReceiveMessage);
        thread.Start();
    }

    // Update is called once per frame
    void Update()
    {
        if (!message.Equals(""))
        {
            chatrecord.text += "\n" + message;
        }
        message = "";
    }

    void SendMessage()
    {
        if (!input.text.ToString().Equals(""))
        {
            mSocket.SendMessage(input.text.ToString());
            input.text = "";
        }
    }

    void ReceiveMessage()
    {
        while (true)
        {
            message = mSocket.ReceiveMessage();
        }
    }
}
