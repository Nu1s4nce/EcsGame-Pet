using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

public class Client
{
    private static Socket _client;
    private byte[] _buffer = new byte[512];
    
    public Client()
    {
        _client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        _client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true );
        StartReceivingMessages();
    }

    public Socket GetSocket()
    {
        return _client;
    }

    public bool IsConnected()
    {
        return _client.Connected;
    }
    
    private void HandleResponse(string response)
    {
        if (response.StartsWith("Names"))
        {
            List<string> namesList = new();
            List<string> tempArr = response.Split(":").ToList();
            string[] arr = tempArr[1].Split(",");
            foreach (var str in arr)
            {
                namesList.Add(str);
            }
            //_pointsManager.SetUpNames(namesList);
        }
        switch (response)
        {
            case "clickApproved":
                //_pointsManager.HandleClick();
                break;
            case "clickFromServer":
                //_pointsManager.HandleClick();
                break;
        }
    }
    
    private async void StartReceivingMessages()
    {
        int bytes;
        do
        {
            bytes = await _client.ReceiveAsync(_buffer, SocketFlags.None);
            string response = Encoding.ASCII.GetString(_buffer, 0, bytes);
            Debug.Log(response);
            HandleResponse(response);
        } while (bytes > 0);
    }

    public void ConnectTo(EndPoint endPoint)
    {
        try
        {
            _client.ConnectAsync(endPoint);
            Debug.Log($"Successfully Connected to {endPoint}");
        }
        catch(SocketException ex)
        {
            Debug.Log(ex.Message);
            _client.Close();
        }
        StartReceivingMessages();
    }
    
    public async void SendMessageToSocket(DataStruct msg)
    {
        byte[] smth = getBytes(msg);
        await _client.SendAsync(smth, SocketFlags.None);
        Debug.Log(msg);
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
        _client.Shutdown(SocketShutdown.Both);
        _client.Close();
    }
}
