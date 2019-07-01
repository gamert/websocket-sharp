using Google.Protobuf;
using System;
using System.Configuration;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using WebSocketSharp;
using WebSocketSharp.Net;
using WebSocketSharp.Server;
using XS2APIProtocol;

namespace Example2
{



    public class Program
    {
        static xs2server _server = new xs2server();
        public static void Main(string[] args)
        {
            //TestProto();

            //            PlayerResult pr = new PlayerResult();
            Size2DI sdi = new Size2DI() { X = 64, Y = 64 };

            _server.Start(null);

            Console.WriteLine("\nPress Enter key to stop the server...");
            Console.ReadLine();

            _server.Stop();
        }

        static void TestProto()
        {
            Response res = new Response();
            res.Ping = new ResponsePing();
            res.Ping.BaseBuild = 1;
            res.Ping.DataBuild = 2;
            res.Ping.DataVersion = "3";
            res.Ping.GameVersion = "4";

            byte[] bb = res.ToByteArray();

            //ByteString a = res.ToByteString();

            //Console.WriteLine(a.ToStringUtf8());
            //Console.WriteLine(a.ToString());

            //Request req = Request.Parser.ParseFrom(a.ToByteArray());
            //if (req != null)
            //{
            //    if(req.RequestCase == Request.RequestOneofCase.Ping)
            //    {
            //        RequestPing ping = req.Ping;
            //        Console.WriteLine(ping.ToString());
            //    }
            //}
            Response res2 = Response.Parser.ParseFrom(bb);
            if (res2 != null)
            {
                if (res2.ResponseCase == Response.ResponseOneofCase.Ping)
                {
                    ResponsePing ping = res2.Ping;
                    Console.WriteLine(ping.ToString());
                }
            }

            //ÐòÁÐ»¯²Ù×÷
            //MemoryStream ms = new MemoryStream();


            //BinaryFormatter bm = new BinaryFormatter();
            //bm.Serialize(ms, p);
            //Serializer.Serialize<Person>(ms, p);
            //byte[] data = ms.ToArray();//length=27  709
            //responsePing.SerializeToString();
            //responsePing.WriteTo(ms);
            //string s  = responsePing.ToString();
            //byte[] a = ms.ToArray();




        }

        //public class DataUtils
        //{
        //    public static byte[] ObjectToBytes<T>(T instance)
        //    {
        //        try
        //        {
        //            byte[] array;
        //            if (instance == null)
        //            {
        //                array = new byte[0];
        //            }
        //            else
        //            {
        //                MemoryStream memoryStream = new MemoryStream();
        //                ProtoBuf.Serializer.Serialize(memoryStream, instance);
        //                array = new byte[memoryStream.Length];
        //                memoryStream.Position = 0L;
        //                memoryStream.Read(array, 0, array.Length);
        //                memoryStream.Dispose();
        //            }

        //            return array;

        //        }
        //        catch (Exception ex)
        //        {

        //            return new byte[0];
        //        }
        //    }

        //    public static T BytesToObject<T>(byte[] bytesData, int offset, int length)
        //    {
        //        if (bytesData.Length == 0)
        //        {
        //            return default(T);
        //        }
        //        try
        //        {
        //            MemoryStream memoryStream = new MemoryStream();
        //            memoryStream.Write(bytesData, 0, bytesData.Length);
        //            memoryStream.Position = 0L;
        //            T result = Serializer.Deserialize<T>(memoryStream);
        //            memoryStream.Dispose();
        //            return result;
        //        }
        //        catch (Exception ex)
        //        {
        //            return default(T);
        //        }
        //    }
        //}


        public static void Main0_(string[] args)
        {

            // Create a new instance of the WebSocketServer class.
            //
            // If you would like to provide the secure connection, you should
            // create a new instance with the 'secure' parameter set to true,
            // or a wss scheme WebSocket URL.

            var wssv = new WebSocketServer(4649);
            //var wssv = new WebSocketServer (5963, true);

            //var wssv = new WebSocketServer (System.Net.IPAddress.Any, 4649);
            //var wssv = new WebSocketServer (System.Net.IPAddress.Any, 5963, true);

            //var wssv = new WebSocketServer (System.Net.IPAddress.IPv6Any, 4649);
            //var wssv = new WebSocketServer (System.Net.IPAddress.IPv6Any, 5963, true);

            //var wssv = new WebSocketServer ("ws://0.0.0.0:4649");
            //var wssv = new WebSocketServer ("wss://0.0.0.0:5963");

            //var wssv = new WebSocketServer ("ws://[::0]:4649");
            //var wssv = new WebSocketServer ("wss://[::0]:5963");

            //var wssv = new WebSocketServer (System.Net.IPAddress.Loopback, 4649);
            //var wssv = new WebSocketServer (System.Net.IPAddress.Loopback, 5963, true);

            //var wssv = new WebSocketServer (System.Net.IPAddress.IPv6Loopback, 4649);
            //var wssv = new WebSocketServer (System.Net.IPAddress.IPv6Loopback, 5963, true);

            //var wssv = new WebSocketServer ("ws://localhost:4649");
            //var wssv = new WebSocketServer ("wss://localhost:5963");

            //var wssv = new WebSocketServer ("ws://127.0.0.1:4649");
            //var wssv = new WebSocketServer ("wss://127.0.0.1:5963");

            //var wssv = new WebSocketServer ("ws://[::1]:4649");
            //var wssv = new WebSocketServer ("wss://[::1]:5963");
#if DEBUG
            // To change the logging level.
            wssv.Log.Level = LogLevel.Trace;

            // To change the wait time for the response to the WebSocket Ping or Close.
            //wssv.WaitTime = TimeSpan.FromSeconds (2);

            // Not to remove the inactive sessions periodically.
            //wssv.KeepClean = false;
#endif
            // To provide the secure connection.
            /*
            var cert = ConfigurationManager.AppSettings["ServerCertFile"];
            var passwd = ConfigurationManager.AppSettings["CertFilePassword"];
            wssv.SslConfiguration.ServerCertificate = new X509Certificate2 (cert, passwd);
             */

            // To provide the HTTP Authentication (Basic/Digest).
            /*
            wssv.AuthenticationSchemes = AuthenticationSchemes.Basic;
            wssv.Realm = "WebSocket Test";
            wssv.UserCredentialsFinder = id => {
                var name = id.Name;

                // Return user name, password, and roles.
                return name == "nobita"
                       ? new NetworkCredential (name, "password", "gunfighter")
                       : null; // If the user credentials aren't found.
              };
             */

            // To resolve to wait for socket in TIME_WAIT state.
            //wssv.ReuseAddress = true;

            // Add the WebSocket services.
            wssv.AddWebSocketService<Echo>("/Echo");
            wssv.AddWebSocketService<Chat>("/Chat");

            // Add the WebSocket service with initializing.
            /*
            wssv.AddWebSocketService<Chat> (
              "/Chat",
              () =>
                new Chat ("Anon#") {
                  // To send the Sec-WebSocket-Protocol header that has a subprotocol name.
                  Protocol = "chat",
                  // To ignore the Sec-WebSocket-Extensions header.
                  IgnoreExtensions = true,
                  // To emit a WebSocket.OnMessage event when receives a ping.
                  EmitOnPing = true,
                  // To validate the Origin header.
                  OriginValidator = val => {
                      // Check the value of the Origin header, and return true if valid.
                      Uri origin;
                      return !val.IsNullOrEmpty ()
                             && Uri.TryCreate (val, UriKind.Absolute, out origin)
                             && origin.Host == "localhost";
                    },
                  // To validate the cookies.
                  CookiesValidator = (req, res) => {
                      // Check the cookies in 'req', and set the cookies to send to
                      // the client with 'res' if necessary.
                      foreach (Cookie cookie in req) {
                        cookie.Expired = true;
                        res.Add (cookie);
                      }

                      return true; // If valid.
                    }
                }
            );
             */

            wssv.Start();
            if (wssv.IsListening)
            {
                Console.WriteLine("Listening on port {0}, and providing WebSocket services:", wssv.Port);
                foreach (var path in wssv.WebSocketServices.Paths)
                    Console.WriteLine("- {0}", path);
            }

            Console.WriteLine("\nPress Enter key to stop the server...");
            Console.ReadLine();

            wssv.Stop();
        }
    }
}
