using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System;

public class ServerSocket : MonoBehaviour
{
    private static byte[] result = new byte[1024];
    private const int port = 8088;
    private static string IpStr = "127.0.0.1";
    private static Socket serverSocket;

    void Start()
    {
        IPAddress ip = IPAddress.Parse(IpStr); IPEndPoint ip_end_point = new IPEndPoint(ip, port);//创建服务器Socket对象，并设置相关属性
        serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);            //绑定ip和端口
        serverSocket.Bind(ip_end_point);            //设置最长的连接请求队列长度
        serverSocket.Listen(10);
        Debug.Log("启动监听"+ serverSocket.LocalEndPoint.ToString()+"成功" );
        Thread thread = new Thread(ClientConnectListen);
        thread.Start();

    }

    void Update()
    {
        
    }

    private static void ClientConnectListen()
    {
        while (true)
        {                //为新的客户端连接创建一个Socket对象
            Socket clientSocket = serverSocket.Accept();
            Console.WriteLine("客户端{0}成功连接", clientSocket.RemoteEndPoint.ToString());                //向连接的客户端发送连接成功的数据
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteString("Connected Server");
            clientSocket.Send(WriteMessage(buffer.ToBytes()));                //每个客户端连接创建一个线程来接受该客户端发送的消息
            Thread thread = new Thread(RecieveMessage);
            thread.Start(clientSocket);
        }
    }

    // 数据转换，网络发送需要两部分数据，一是数据长度，二是主体数据
    private static byte[] WriteMessage(byte[] message)
    {
        MemoryStream ms = null;
        using (ms = new MemoryStream())
        {
            ms.Position = 0;
            BinaryWriter writer = new BinaryWriter(ms);
            ushort msglen = (ushort)message.Length;
            writer.Write(msglen);
            writer.Write(message);
            writer.Flush();
            return ms.ToArray();
        }
    }

    // 接收指定客户端Socket的消息
    private static void RecieveMessage(object clientSocket)
    {
        Socket mClientSocket = (Socket)clientSocket;
        while (true)
        {
            try
            {
                int receiveNumber = mClientSocket.Receive(result);
                Console.WriteLine("接收客户端{0}消息， 长度为{1}", mClientSocket.RemoteEndPoint.ToString(), receiveNumber);
                ByteBuffer buff = new ByteBuffer(result);                    //数据长度
                int len = buff.ReadShort();                    //数据内容
                string data = buff.ReadString();
                Console.WriteLine("数据内容：{0}", data);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                mClientSocket.Shutdown(SocketShutdown.Both);
                mClientSocket.Close();
                break;
            }
        }
    }

}
