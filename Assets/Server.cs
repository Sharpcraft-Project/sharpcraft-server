using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fleck;

public class Server : MonoBehaviour
{

    public class Player
    {
        public IWebSocketConnection connection;
        public string PlayerName;
        public Vector3 Position;
        public Vector3 EulerAngles;
    }

    public int port;
    public WebSocketServer server;
    void Start()
    {
        server = new WebSocketServer("ws://0.0.0.0:" + port.ToString());
        server.Start(socket =>
        {
            socket.OnOpen = () => onOpen(socket);
            socket.OnClose = () => onClose(socket);
            socket.OnMessage = message => onMessage(socket, message);
        });
    }

    List<Player> Players;

    void onClose(IWebSocketConnection connection)
    {
        RemoveConnection(connection);
    }

    void RemoveConnection(IWebSocketConnection connection)
    {
        for (int i = 0; i < Players.Count; i++)
        {
            if (Players[i].connection == connection)
            {
                Players.Remove(Players[i]);
            }
        }
    }

    List<IWebSocketConnection> PendingPlayer;
    void onOpen(IWebSocketConnection connection)
    {
        Debug.Log(connection.ConnectionInfo.ClientIpAddress.ToString());
        Player player = new Player();
        player.connection = connection;
        Players.Add(player);
    }

    Player ConnectionToPlayer (IWebSocketConnection connection)
    {
        for(int i = 0; i < Players.Count; i++)
        {
            if(Players[i].connection == connection)
            {
                return Players[i];
            }
        }
        return null;
    }
    //end of raw server stuff

    void onMessage(IWebSocketConnection connection, string Message)
    {
        if (isChat(Message))
        {
            if (!isChatCommand(Message))
            {
                Broadcast(ConnectionToPlayer(connection).PlayerName + ":" + Split(Message)[1]);
            }
            else
            {
                RunCommand(Split(Message)[1], "XXXXX");
            }
        }
        else
        {
            Debug.Log(Message);
            connection.Send("ok");
        }
    }

    string[] Split(string str)
    {
        return str.Split(new char[] { 'γ' });
    }

    public void Broadcast(string Message)
    {
        for (int i = 0; i < Players.Count; i++)
        {
            Players[i].connection.Send(Message);
        }
    }

    //chat related
    bool isChat(string str)
    {
        if(Split(str)[0] == "MSG")
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    bool isChatCommand(string str)
    {
        if(Split(str)[1][0] == '/')
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void RunCommand(string str, string PlayerName)
    {

    }
    //chat related
}