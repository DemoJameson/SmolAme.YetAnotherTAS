using System;
using System.Threading;
using System.Threading.Tasks;
using GodSharp.Sockets;
using TAS.Core.Hotkey;
using TAS.Core.Input;
using TAS.Core.Utils;
using TAS.Shared;
using TAS.Shared.Communication.GameToStudio;
using TAS.Shared.Communication.StudioToGame;

namespace TAS.Core;

public static class CommunicationServer {
    private static ITcpServer server;
    public static bool Connected => server != null && server.Connections.IsNotEmpty();

    public static void Start(int? port = null) {
        server?.Stop();
        port ??= 19982;
        server = new TcpServer(port.Value) {
            OnConnected = c => {
                Console.WriteLine($"{c.RemoteEndPoint} connected.");
                
                Task.Run(()=> {
                    Thread.Sleep(100);
                    SendMessage(new HotkeySettingsMessage(Hotkeys.KeysInteractWithStudio));
                });
            },
            OnReceived = c => {
                IStudioToGameMessage serverToClientMessage = BinaryFormatterHelper.FromByteArray<IStudioToGameMessage>(c.Buffers);

                if (serverToClientMessage is HotkeyMessage hotkeyMessage) {
                    ReceiveHotkeyMessage(hotkeyMessage);
                } else if (serverToClientMessage is PathMessage pathMessage) {
                    ReceivePathMessage(pathMessage);
                }
            },
            OnDisconnected = c => { Console.WriteLine($"{c.RemoteEndPoint} disconnected."); },
            OnStarted = c => { Console.WriteLine($"{c.LocalEndPoint} started."); },
            OnStopped = c => { Console.WriteLine($"{c.LocalEndPoint} stopped."); },
            OnException = c => { Console.WriteLine($"{c.RemoteEndPoint} exception: {c.Exception}."); },
            OnServerException = c => { Console.WriteLine($"{c.LocalEndPoint} exception: {c.Exception}."); }
        };
        server.UseKeepAlive(true, 500, 500);
        server.Start();
        Console.WriteLine("CommunicationServer Started!");
    }

    public static void SendMessage(IGameToStudioMessage message) {
        if (!Connected) {
            return;
        }
        
        foreach (ITcpConnection connection in server.Connections.Values) {
            connection.Send(BinaryFormatterHelper.ToByteArray(message));
        }
    }

    public static void Stop() {
        server?.Stop();
        server = null;
    }

    private static void ReceiveHotkeyMessage(HotkeyMessage hotKeyMessage) {
        Hotkeys.KeysDict[hotKeyMessage.HotkeyID].OverrideCheck = !hotKeyMessage.released;
    }

    private static void ReceivePathMessage(PathMessage pathMessage) {
        InputController.StudioTasFilePath = pathMessage.Path;
    }
}