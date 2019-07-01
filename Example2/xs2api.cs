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

    void SendResponse(Response resp)
    {
        byte[] bb = resp.ToByteArray();
        Context.WebSocket.Send(bb);
    }

    //TODO: 填充ResponseGameInfo
    void FillGameInfo(ResponseGameInfo gameInfo)
    {
        gameInfo.MapName = requestCreateGame.BattlenetMapName;//requestCreateGame.MapCase;
        gameInfo.LocalMapPath = requestCreateGame.LocalMap.MapPath; //TODO: 保存requestCreateGame.LocalMap.MapData;
        gameInfo.ModNames.Add("1");

        //根据requestJoinGame
        //+		requestJoinGame	{{ "race": "Zerg", "options": { "raw": true, "score": true, "featureLayer": { "width": 24, "resolution": { "x": 32, "y": 32 }, "minimapResolution": { "x": 32, "y": 32 } } } }}	XS2APIProtocol.RequestJoinGame
        //填充InterfaceOptions
        gameInfo.Options = requestJoinGame.Options;
        //gameInfo.Options.Raw = requestJoinGame.Options.Raw;
        //gameInfo.Options.RawAffectsSelection = true;
        //gameInfo.Options.RawCropToPlayableArea = true;
        //gameInfo.Options.Score = requestJoinGame.Options.Score;
        //gameInfo.Options.ShowCloaked = true;
        //gameInfo.Options.ShowPlaceholders = true;

        ////填充..Options.FeatureLayer
        //gameInfo.Options.FeatureLayer = requestJoinGame.Options.FeatureLayer;// Resolution = new Size2DI() { X = 64, Y = 64 };
        //gameInfo.Options.Render = requestJoinGame.Options.Render;
        //gameInfo.Options.FeatureLayer.MinimapResolution = new Size2DI() { X = 64, Y = 64 };
        //gameInfo.Options.FeatureLayer.Width = 32;
        //gameInfo.Options.FeatureLayer.CropToPlayableArea = true;

        //填充..Options.Render
        //gameInfo.Options.Render.Resolution = new Size2DI() { X = 64, Y = 64 };
        //gameInfo.Options.Render.MinimapResolution = new Size2DI() { X = 64, Y = 64 };
        //gameInfo.Options.Render.Width = 32;
        //gameInfo.Options.Render.CropToPlayableArea = true;


        uint PlayerId = 1;
        //填充..PlayerInfo
        foreach (PlayerSetup ps in requestCreateGame.PlayerSetup)
        {
            PlayerInfo pi = new PlayerInfo();
            pi.Difficulty = ps.Difficulty;
            pi.PlayerId = PlayerId++;
            pi.PlayerName = ps.PlayerName;// "player" + i;
            pi.RaceActual = ps.Race;
            pi.RaceRequested = ps.Race;
            pi.Type = ps.Type;

            gameInfo.PlayerInfo.Add(pi);
        }
        //填充..StartRaw
        gameInfo.StartRaw = new StartRaw();
        gameInfo.StartRaw.MapSize = new Size2DI() { X = 64, Y = 64 };
        //gameInfo.StartRaw.PathingGrid = 
        //gameInfo.StartRaw.PlacementGrid
        //gameInfo.StartRaw.PlayableArea
        gameInfo.StartRaw.StartLocations.Add(new Point2D() { X = 64, Y = 64 });
        //gameInfo.StartRaw.TerrainHeight.

    }

    //TODO: 原始单位...
    void FillObservationRaw(ObservationRaw rawData)
    {
        rawData.Player = new PlayerRaw();
        rawData.Player.Camera = new Point() { X = 0, Y = 0, Z = 0 };

        //out["units"] = self.transform_unit_control(raw)
        //rawData.Units.Clear();
        for (uint i = 0;i < 2;++i)
        {
            Unit u = new Unit();
            u.Alliance = i== 0 ? Alliance.Self: Alliance.Enemy;
            u.Tag = i;
            u.UnitType = 86;    //UNIT_TYPE.ZERG_HATCHERY = 86 基地...
            //owner_ = other.owner_;
            u.Pos = new Point() { X=0,Y = 0,Z = 0};
            u.Facing = 0.1f;
            u.Radius = 1.0f;
            //buildProgress_ = other.buildProgress_;
            u.Cloak = CloakState.CloakedAllied;
            //buffIds_ = other.buffIds_.Clone();
            u.DetectRange = 1.0f;
            u.RadarRange = 1.0f;
            //isSelected_ = other.isSelected_;
            //isOnScreen_ = other.isOnScreen_;
            //isBlip_ = other.isBlip_;
            //isPowered_ = other.isPowered_;
            u.IsActive = true;
            //attackUpgradeLevel_ = other.attackUpgradeLevel_;
            //armorUpgradeLevel_ = other.armorUpgradeLevel_;
            //shieldUpgradeLevel_ = other.shieldUpgradeLevel_;
            u.Health = 1.0f;
            u.HealthMax = 1.0f;
            //shield = 1.0f;
            //shieldMax = 1.0f;
            u.Energy = 1.0f;
            u.EnergyMax = 1.0f;
            u.MineralContents = 2;
            u.VespeneContents = 2;
            //isFlying_ = other.isFlying_;
            //isBurrowed_ = other.isBurrowed_;
            //isHallucination_ = other.isHallucination_;
            //orders_ = other.orders_.Clone();
            u.AddOnTag = 2;
            //passengers_ = other.passengers_.Clone();
            //cargoSpaceTaken_ = other.cargoSpaceTaken_;
            //cargoSpaceMax_ = other.cargoSpaceMax_;
            //assignedHarvesters_ = other.assignedHarvesters_;
            //idealHarvesters_ = other.idealHarvesters_;
            u.WeaponCooldown = 1.0f;
            u.EngagedTargetTag = 2;
            //buffDurationRemain_ = other.buffDurationRemain_;
            //buffDurationMax_ = other.buffDurationMax_;
            //rallyTargets_ = other.rallyTargets_.Clone();
            rawData.Units.Add(u);
        }

        rawData.MapState = new MapState();
        rawData.Effects.Clear();
        rawData.Event = new Event();
    }

    //TODO: 填充观察者信息:
    //self._units = observation['units']
    //self._player = observation['player']
    //self._raw_data = observation['raw_data']
    void FillObservation(ResponseObservation responseObservation)
    {
        //responseObservation.Chat
        Observation obs = new Observation();// responseObservation.Observation;
        obs.Abilities.Clear();
        obs.Alerts.Clear();
        //obs.FeatureLayerData.MinimapRenders.
        obs.GameLoop = 0;
        obs.PlayerCommon = new PlayerCommon();// = 1;
        obs.RawData = new ObservationRaw();
        FillObservationRaw(obs.RawData);

        obs.RenderData = new ObservationRender();
        obs.Score = new Score();
        obs.UiData = new ObservationUI();

        //不能有结果，否则结束战斗...
        //foreach (PlayerInfo pi in responseGameInfo.PlayerInfo)
        //{
        //    PlayerResult pr = new PlayerResult();
        //    pr.PlayerId = pi.PlayerId;
        //    pr.Result = Result.Undecided;
        //    responseObservation.PlayerResult.Add(pr);
        //}
        
        responseObservation.Observation = obs;
    }

    //TODO: 
    void FillResponseData(ResponseData data)
    {
        data.Abilities.Clear();
        data.Buffs.Clear();
        data.Effects.Clear();
        data.Units.Clear();
        data.Upgrades.Clear();
    }

    //用户请求创建...
    RequestCreateGame requestCreateGame;
    RequestJoinGame requestJoinGame;
    ResponseGameInfo responseGameInfo = new ResponseGameInfo();
    //1.单个战斗的支持
    //2.
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
                    {
                        requestCreateGame = req.CreateGame;
                        //Log.Warn(requestCreateGame.ToString());

                        Response resp = new Response();
                        resp.CreateGame = new ResponseCreateGame();
                        //resp.CreateGame.

                        resp.Status = Status.InitGame;      //各种状态...
                        SendResponse(resp);
                        return;
                    }
                case Request.RequestOneofCase.JoinGame:
                    {
                        //+		requestJoinGame	{{ "race": "Zerg", "options": { "raw": true, "score": true, "featureLayer": { "width": 24, "resolution": { "x": 32, "y": 32 }, "minimapResolution": { "x": 32, "y": 32 } } } }}	XS2APIProtocol.RequestJoinGame
                        requestJoinGame = req.JoinGame;
                        Log.Warn(requestJoinGame.ToString());

                        Response resp = new Response();
                        resp.JoinGame = new ResponseJoinGame();
                        resp.JoinGame.PlayerId = 1;

                        resp.Status = Status.InGame;      //各种状态...
                        SendResponse(resp);
                        return;
                    }
                case Request.RequestOneofCase.RestartGame:
                    {
                        //+		requestJoinGame	{{ "race": "Zerg", "options": { "raw": true, "score": true, "featureLayer": { "width": 24, "resolution": { "x": 32, "y": 32 }, "minimapResolution": { "x": 32, "y": 32 } } } }}	XS2APIProtocol.RequestJoinGame
                        RequestRestartGame restartGame = req.RestartGame;
                        Log.Warn(restartGame.ToString());

                        Response resp = new Response();
                        resp.RestartGame = new ResponseRestartGame();
                        //resp.RestartGame.PlayerId = 1;

                        resp.Status = Status.InGame;      //各种状态...
                        SendResponse(resp);
                        return;
                    }
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
                    {
                        RequestGameInfo requestGameInfo = req.GameInfo;
                        Log.Warn(requestGameInfo.ToString());

                        Response resp = new Response();
                        resp.GameInfo = responseGameInfo;
                        FillGameInfo(resp.GameInfo);
                        resp.Status = Status.InGame;      //各种状态...
                        SendResponse(resp);
                        return;
                    }
                case Request.RequestOneofCase.Observation:
                    {
                        RequestObservation requestObservation = req.Observation;
                        Log.Warn(requestObservation.ToString());

                        Response resp = new Response();
                        resp.Observation = new ResponseObservation();
                        FillObservation(resp.Observation);
                        resp.Status = Status.InGame;      //各种状态...
                        SendResponse(resp);
                        return;
                    }
                case Request.RequestOneofCase.Action:
                    {
                        RequestAction requestAction = req.Action;
                        Log.Warn(requestAction.ToString());

                        Response resp = new Response();
                        resp.Action = new ResponseAction();
                        resp.Status = Status.InGame;      //各种状态...
                        SendResponse(resp);
                        return;
                    }
                case Request.RequestOneofCase.ObsAction:
                    {
                        RequestObserverAction requestObsAction = req.ObsAction;
                        Log.Warn(requestObsAction.ToString());

                        Response resp = new Response();
                        resp.ObsAction = new ResponseObserverAction();
                        resp.Status = Status.InGame;      //各种状态...
                        SendResponse(resp);
                        return;
                    }
                case Request.RequestOneofCase.Step:
                    {
                        RequestStep requestStep = req.Step;
                        Log.Warn(requestStep.ToString());

                        Response resp = new Response();
                        resp.Step = new ResponseStep();
                        resp.Step.SimulationLoop = 2;
                       
                        resp.Status = Status.InGame;      //各种状态...
                        SendResponse(resp);
                        return;
                    }
                case Request.RequestOneofCase.Data:
                    {
                        RequestData requestData = req.Data;
                        Log.Warn(requestData.ToString());

                        Response resp = new Response();
                        resp.Data = new ResponseData();
                        FillResponseData(resp.Data);

                        resp.Status = Status.InGame;      //各种状态...
                        SendResponse(resp);
                        return;
                    }
                case Request.RequestOneofCase.Query:
                    {
                        RequestQuery requestQuery = req.Query;
                        Log.Warn(requestQuery.ToString());

                        Response resp = new Response();
                        resp.Query = new ResponseQuery();
                        resp.Status = Status.InGame;      //各种状态...
                        SendResponse(resp);
                        return;
                    }
                case Request.RequestOneofCase.SaveReplay:
                    break;
                case Request.RequestOneofCase.MapCommand:
                    {
                        RequestMapCommand requestMapCommand = req.MapCommand;
                        Log.Warn(requestMapCommand.ToString());

                        Response resp = new Response();
                        resp.MapCommand = new ResponseMapCommand();
                        resp.Status = Status.InGame;      //各种状态...
                        SendResponse(resp);
                        return;
                    }
                case Request.RequestOneofCase.ReplayInfo:
                    break;
                case Request.RequestOneofCase.AvailableMaps:
                    {
                        RequestAvailableMaps requestAvailableMaps = req.AvailableMaps;
                        Log.Warn(requestAvailableMaps.ToString());

                        Response resp = new Response();
                        resp.AvailableMaps = new ResponseAvailableMaps();
                        resp.Status = Status.InGame;      //各种状态...
                        SendResponse(resp);
                        return;
                    }
                case Request.RequestOneofCase.SaveMap:
                    {
                        RequestSaveMap requestSaveMap = req.SaveMap;
                        Log.Warn(requestSaveMap.ToString());

                        Response resp = new Response();
                        resp.SaveMap = new ResponseSaveMap();
                        resp.Status = Status.InGame;      //各种状态...
                        SendResponse(resp);
                        return;
                    }
                case Request.RequestOneofCase.Ping:
                    {
                        RequestPing ping = req.Ping;
                        Log.Warn(ping.ToString());

                        Response resp = new Response();
                        resp.Ping = new ResponsePing();
                        resp.Ping.BaseBuild = 1;
                        resp.Ping.DataBuild = 2;
                        resp.Ping.DataVersion = "3";
                        resp.Ping.GameVersion = "4";
                        resp.Status = Status.Launched;      //各种状态...

                        byte[] bb = resp.ToByteArray();
                        Context.WebSocket.Send(bb);
                        //Sessions.SendTo(s, e.id);//e.
                        //responsePing.WriteTo(_websocket);

                        return;
                    }
                case Request.RequestOneofCase.Debug:
                    {
                        RequestDebug requestDebug = req.Debug;
                        Log.Warn(requestDebug.ToString());

                        Response resp = new Response();
                        resp.Debug = new ResponseDebug();
                        resp.Status = Status.InGame;      //各种状态...
                        SendResponse(resp);
                        return;
                    }
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
