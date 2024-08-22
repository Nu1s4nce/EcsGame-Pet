using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

public class Server
{
    public bool IsServer => _server != null;
    
    private Socket _server;
    private byte[] _buffer = new byte[512];
    private IPEndPoint _localEp;

    private bool _canConnect = true;
    private Socket _handler;

    private Dictionary<int, Socket> _allConnections = new();
    private List<string> _allNames = new(); 
    private int _id;
    
    public void StartServer(int port)
    {
        _localEp = new IPEndPoint(GetLocalIPAddress(), port);
        _server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        _server.Bind(_localEp);
        _server.Listen(100);

        HandleClients(_server);
        
        if (_server != null)
            Debug.Log($"Server started with ip:{GetLocalIPAddress()}");
    }
    public Socket GetSocket()
    {
        return _server;
    }
    public void SetConnect(bool b)
    {
        _canConnect = b;
    }
    public async void SendMessageToSocket(string msg)
    {
        Debug.Log(msg);
        byte[] smth = Encoding.ASCII.GetBytes(msg);
        //smth = AddByteToArray(smth, 0);
        await _handler.SendAsync(smth, SocketFlags.None);
    }
    public async void SendMessageToSocket(DataStruct msg, Socket handler)
    {
        byte[] smth = getBytes(msg);
        await handler.SendAsync(smth, SocketFlags.None);
    }
    public void SendMessageToAllSockets(DataStruct msg)
    {
        foreach (KeyValuePair<int, Socket> connection in _allConnections)
        {
            SendMessageToSocket(msg, connection.Value);
        }
        Debug.Log(msg);
    }
    
    private void HandleResponse(DataStruct response)
    {
        // if (response.StartsWith("Name"))
        // {
        //     string tempArr = response.Split(":")[1];
        //     SetUpNameToList(tempArr);
        // }
        // switch (response)
        // {
        //     case "click":
        //         //_pointsManager.HandleClick();
        //         SendMessageToAllSockets("clickApproved");
        //         break;
        //     case "GetAllNickNames":
        //         SendMessageToAllSockets(GetNickNamesString());
        //         break;
        // }
    }

    public void SetUpNameToList(string name)
    {
        _allNames.Add(name);
    }

    public List<string> GetAllNamesArray()
    {
        return _allNames;
    }

    private string GetNickNamesString()
    {
        return $"Names:{string.Join( ",", _allNames.ToArray())}";
    }

    private IPAddress GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip;
            }
        }

        Debug.Log("No network adapters with an IPv4 address in the system!");
        return null;
    }

    private void AddConnectionToTheList()
    {
        _id += 1;
        _allConnections.Add(_id, _handler);
    }
    private async void HandleClients(Socket server)
    {
        while (_canConnect)
        {
            _handler = await server.AcceptAsync();
            StartReceivingMessages(_handler);
            AddConnectionToTheList();
            Debug.Log("Адрес подключенного клиента:" + _handler.RemoteEndPoint);
        }
    }

    private async void StartReceivingMessages(Socket socket)
    {
        int bytes;
        do
        {
            bytes = await socket.ReceiveAsync(_buffer, SocketFlags.None);
            DataStruct response = fromBytes(_buffer);
            Debug.Log(response);
            HandleResponse(response);
        } while (bytes > 0);
    }
    private byte[] getBytes(DataStruct str) {
        int size = Marshal.SizeOf(str);
        byte[] arr = new byte[size];
        IntPtr ptr = IntPtr.Zero;
        try
        {
            ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(str, ptr, true);
            Marshal.Copy(ptr, arr, 0, size);
        }
        finally
        {
            Marshal.FreeHGlobal(ptr);
        }
        return arr;
    }
    private DataStruct fromBytes(byte[] arr)
    {
        DataStruct str = new DataStruct();
        int size = Marshal.SizeOf(str);
        IntPtr ptr = IntPtr.Zero;
        try
        {
            ptr = Marshal.AllocHGlobal(size);
            Marshal.Copy(arr, 0, ptr, size);
            str = (DataStruct)Marshal.PtrToStructure(ptr, str.GetType());
        }
        finally
        {
            Marshal.FreeHGlobal(ptr);
        }
        return str;
    }
    public void ShutDownSocket()
    {
        if (_server.Connected)
        {
            _server.Shutdown(SocketShutdown.Both);
            _server.Close();
        }
    }
}