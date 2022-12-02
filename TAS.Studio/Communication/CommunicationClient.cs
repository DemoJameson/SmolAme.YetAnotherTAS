using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using GodSharp.Sockets;
using TAS.Shared;
using TAS.Shared.Communication.GameToStudio;
using TAS.Shared.Communication.StudioToGame;

namespace TAS.Studio.Communication;

public static class CommunicationClient {
    private static TcpClient client;
    public static bool Connected => client?.Connected == true;

    public static void Connect(string host = null, int? port = null) {
        client?.Stop();

        host ??= Settings.Instance.CommunicationHost;
        port ??= Settings.Instance.CommunicationPort;

        client = new(host, port.Value) {
            OnConnected = c => {
                Console.WriteLine($"{c.RemoteEndPoint} connected.");
                Task.Run(() => {
                    Thread.Sleep(100);
                    SendMessage(new PathMessage(Studio.CurrentFileName ?? ""));
                });
            },
            OnReceived = c => {
                Console.WriteLine($"Received from {c.RemoteEndPoint}:");
                IGameToStudioMessage gameToStudioMessage = BinaryFormatterHelper.FromByteArray<IGameToStudioMessage>(c.Buffers);
                Console.WriteLine(gameToStudioMessage);

                switch (gameToStudioMessage) {
                    case GameInfoMessage gameInfoMessage:
                        ReceiveStudioInfo(gameInfoMessage);
                        break;
                    case HotkeySettingsMessage hotkeySettingsMessage:
                        ReceiveHotkeySettingsMessage(hotkeySettingsMessage);
                        break;
                    case UpdateTextsMessage updateTextsMessage:
                        ReceiveUpdateTextsMessage(updateTextsMessage);
                        break;
                }
            },
            OnDisconnected = c => { Console.WriteLine($"{c.RemoteEndPoint} disconnected."); },
            OnStarted = c => { Console.WriteLine($"{c.RemoteEndPoint} started."); },
            OnStopped = c => { Console.WriteLine($"{c.RemoteEndPoint} stopped."); },
            OnException = c => {
                Console.WriteLine($"{c.RemoteEndPoint} exception:Message:{c.Exception.Message},StackTrace:{c.Exception.StackTrace.ToString()}.");
            },
            OnTryConnecting = c => { Console.WriteLine($"try connect {c.Counter} ..."); }
        };

        // set keep alive
        client.UseKeepAlive(true, 500, 500);

        try {
            client.Start();
            Console.WriteLine("Started");
        } catch (Exception ex) {
            Console.WriteLine($"Message: {ex.Message}\nStackTrace: {ex.StackTrace}");
        }
    }

    public static void SendMessage(IStudioToGameMessage message) {
        if (client?.Connected != true) {
            return;
        }

        client.Connection.Send(BinaryFormatterHelper.ToByteArray(message));
    }

    private static void ReceiveStudioInfo(GameInfoMessage gameInfo) {
        try {
            CommunicationWrapper.GameInfo = gameInfo;
        } catch (InvalidCastException) {
            string studioVersion = Studio.Version.ToString(3);
            MessageBox.Show(
                $"TAS.Studio v{studioVersion} and TAS.Core v{ErrorLog.ModVersion} do not match. Please manually extract the TasStudio from the zip file.",
                "Communication Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Application.Exit();
        }
    }

    private static void ReceiveHotkeySettingsMessage(HotkeySettingsMessage hotkeySettingsMessage) {
        CommunicationWrapper.SetBindings(hotkeySettingsMessage.Settings.ToDictionary(pair => (HotkeyID) pair.Key, pair => pair.Value.Cast<KeyCodes>().ToList()));
    }

    private static void ReceiveUpdateTextsMessage(UpdateTextsMessage updateTextsMessage) {
        CommunicationWrapper.UpdateTexts(updateTextsMessage.Texts);
    }
}