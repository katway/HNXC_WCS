﻿using System;
using System.Collections.Generic;
using System.Text;
using THOK.MCP;
using THOK.TCP;
using System.Net.Sockets;
using System.Net;

namespace THOK.MCP.Service.TCP
{
    public class TCPService:THOK.MCP.AbstractService 
    {
        private Server server = null;
        private Client client = null;
        private string ip = "127.0.0.1";
        private int port = 6000;
        private IProtocolParse protocol = null;

        public override void Initialize(string file)
        {
            Config.Configuration config = new Config.Configuration(file);
            protocol = (IProtocolParse)ObjectFactory.CreateInstance(config.Type);

            ip = config.IP;
            port = config.Port;
            server = new Server();
            server.OnReceive += new ReceiveEventHandler(server_OnReceive);
        }

        private void server_OnReceive(object sender, ReceiveEventArgs e)
        {
            Message message = null;
            if (null != protocol)
                message = protocol.Parse(e.Read());
            else
            {
                message = new Message(e.Read());
            }
            if (message.Parsed)
                DispatchState(message.Command, message.Parameters);
        }

        public override void Release()
        {
            server.StopListen();
        }

        public override void Start()
        {
            server.StartListen(ip, port);
        }

        public override void Stop()
        {
            server.StopListen();
        }

        public override object Read(string itemName)
        {
            throw new Exception("TCPService未实现Read方法，请用System.Net.Sockets.TCPClient类发送TCP消息。");
        }

        public override bool Write(string itemName, object state)
        {
            //if(server.OnlineCount)

            server.Write("127.0.0.1", "");
            return true;
            //throw new Exception("TCPService未实现Write方法，请用System.Net.Sockets.TCPClient类发送TCP消息。");
            //TcpClient tcpClient = new TcpClient();
            //tcpClient.Connect(IPAddress.Parse(ip), port);

            //NetworkStream ns = tcpClient.GetStream();


            //if (ns.CanWrite)
            //{
            //    Byte[] sendBytes = Encoding.UTF8.GetBytes(state.ToString());
            //    ns.Write(sendBytes, 0, sendBytes.Length);
            //}           

            //ns.Close();
            //tcpClient.Close();

        }
    }
}