using Google.Protobuf;
using System;
using System.IO;
using System.Threading;
using WebSocketSharp;
using WebSocketSharp.Server;
using XS2APIProtocol;

public class xs2api : WebSocketBehavior
{
    private string _name;
    private static int _number = 0;
    private string _prefix;

    public xs2api()
      : this(null)
    {
    }

    public xs2api(string prefix)
    {
        _prefix = !prefix.IsNullOrEmpty() ? prefix : "anon#";
    }

    private string getName()
    {
        var name = Context.QueryString["name"];
        return !name.IsNullOrEmpty() ? name : _prefix + getNumber();
    }

    private static int getNumber()
    {
        return Interlocked.Increment(ref _number);
    }

    protected override void OnClose(CloseEventArgs e)
    {
        Sessions.Broadcast(String.Format("{0} got logged off...", _name));
    }

    protected override void OnMessage(MessageEventArgs e)
    {
        //   Sessions.Broadcast(String.Format("{0}: {1}", _name, e.Data));
        //如何处理e.Data?
        Request req = Request.Parser.ParseFrom(e.RawData);
        if(req!=null)
        {
            switch (req.RequestCase)
            {
                case Request.RequestOneofCase.CreateGame:
                    break;
                case Request.RequestOneofCase.JoinGame:
                    break;
                case Request.RequestOneofCase.RestartGame:
                    break;
                case Request.RequestOneofCase.StartReplay:
                    break;
                case Request.RequestOneofCase.LeaveGame:
                    break;
                case Request.RequestOneofCase.QuickSave:
                    break;
                case Request.RequestOneofCase.QuickLoad:
                    break;
                case Request.RequestOneofCase.Quit:
                    break;
                case Request.RequestOneofCase.GameInfo:
                    break;
                case Request.RequestOneofCase.Observation:
                    break;
                case Request.RequestOneofCase.Action:
                    break;
                case Request.RequestOneofCase.ObsAction:
                    break;
                case Request.RequestOneofCase.Step:
                    break;
                case Request.RequestOneofCase.Data:
                    break;
                case Request.RequestOneofCase.Query:
                    break;
                case Request.RequestOneofCase.SaveReplay:
                    break;
                case Request.RequestOneofCase.MapCommand:
                    break;
                case Request.RequestOneofCase.ReplayInfo:
                    break;
                case Request.RequestOneofCase.AvailableMaps:
                    break;
                case Request.RequestOneofCase.SaveMap:
                    break;
                case Request.RequestOneofCase.Ping:
                    {
                        RequestPing ping = req.Ping;
                        Log.Warn(ping.ToString());
                        //.WriteTo();
                        ResponsePing responsePing = new ResponsePing();
                        responsePing.BaseBuild = 1;
                        responsePing.DataBuild = 2;
                        responsePing.DataVersion = "3";
                        responsePing.GameVersion = "4";

                        //序列化操作
                        MemoryStream ms = new MemoryStream();
                        //BinaryFormatter bm = new BinaryFormatter();
                        //bm.Serialize(ms, p);
                        //Serializer.Serialize<Person>(ms, p);
                        //byte[] data = ms.ToArray();//length=27  709
                        //responsePing.SerializeToString();
                        responsePing.WriteTo(ms);
                        //string s  = responsePing.ToString();
                        byte[] a = ms.ToArray();
                        Context.WebSocket.Send(a);
                        //Sessions.SendTo(s, e.id);//e.
                        //responsePing.WriteTo(_websocket);

                    }
                    break;
                case Request.RequestOneofCase.Debug:
                    break;
            }
        }
        else
        {

        }
    }
    //
    //void _write<T>(ref IMessage<T> message)
    //{

    //}
    //"""Actually serialize and write the request."""
    //with sw("serialize_request"):
    //  request_str = request.SerializeToString()
    //with sw("write_request") :
    //  with catch_websocket_connection_errors():
    //    self._sock.send(request_str)



    protected override void OnOpen()
    {
        _name = getName();
    }
}
