// <copyright file="GsLiveHandler.cs" company="Firoozeh Technology LTD">
// Copyright (C) 2019 Firoozeh Technology LTD. All Rights Reserved.
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//    limitations under the License.
// </copyright>

using System.Collections.Generic;
using FiroozehGameService.Core;
using FiroozehGameService.Handlers;
using FiroozehGameService.Models.Enums;
using FiroozehGameService.Models.GSLive;
using FiroozehGameService.Models.GSLive.Command;
using FiroozehGameService.Models.GSLive.RT;
using Models;
using Newtonsoft.Json;
using UnityEngine;
using Event = Models.Event;

/**
* @author Alireza Ghodrati
*/

namespace Handlers
{
    
    public class GsLiveHandler
    {
        private Member _me, _opponent;
        private bool _updateRequired, _opponentLeft;
        private Event _newEventReceived;
        
        public GsLiveHandler()
        {
            SetListeners();
            //GameService.GSLive.RealTime.GetRoomMembersDetail();
        }


        
        public bool IsUpdateRequired() => _updateRequired;
        public bool IsOpponentLeftGame() => _opponentLeft;


        public Event GetNewEvent()
        {
            _updateRequired = false;
            return _newEventReceived;
        }

      
        public void SendPosToServer(float x,float y)
        {
            if(!GameService.GSLive.IsRealTimeAvailable()) return;
            
            var newEvent = new Event
            {
                Action = EventAction.Pos,
                Payload = new Payload
                {
                    X = x,
                    Y = y
                }
            };
            
            var json = JsonConvert.SerializeObject(newEvent , new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore});
            GameService.GSLive.RealTime.SendPublicMessage(json,GProtocolSendType.Reliable);
        }

        public void SendMoveToServer(float x,float y)
        {
            if(!GameService.GSLive.IsRealTimeAvailable()) return;
            
            var newEvent = new Event
            {
                Action = EventAction.Move,
                Payload = new Payload
                {
                    X = x,
                    Y = y
                }
            };
            
            var json = JsonConvert.SerializeObject(newEvent , new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore});
            GameService.GSLive.RealTime.SendPublicMessage(json,GProtocolSendType.UnReliable);
        }
        
        public void SendJumpToServer()
        {
            if(!GameService.GSLive.IsRealTimeAvailable()) return;
            
            var newEvent = new Event
            {
                Action = EventAction.Jump
            };
            
            var json = JsonConvert.SerializeObject(newEvent , new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore});
            GameService.GSLive.RealTime.SendPublicMessage(json,GProtocolSendType.Reliable);
        }
        
        public void SendShootToServer()
        {
            if(!GameService.GSLive.IsRealTimeAvailable()) return;
            
            var newEvent = new Event
            {
                Action = EventAction.Shoot
            };
            
            var json = JsonConvert.SerializeObject(newEvent , new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore});
            GameService.GSLive.RealTime.SendPublicMessage(json,GProtocolSendType.Reliable);
        }
        
        public void SendDieToServer()
        {
            if(!GameService.GSLive.IsRealTimeAvailable()) return;
            
            var newEvent = new Event
            {
                Action = EventAction.Die
            };
            
            var json = JsonConvert.SerializeObject(newEvent , new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore});
            GameService.GSLive.RealTime.SendPublicMessage(json,GProtocolSendType.Reliable);
        }
        
        

        private void SetListeners()
        {
            RealTimeEventHandlers.Error += OnError;
            RealTimeEventHandlers.LeftRoom += OnLeftRoom;
            RealTimeEventHandlers.NewMessageReceived += OnNewMessageReceived;
            RealTimeEventHandlers.RoomMembersDetailReceived += OnRoomMembersDetailReceived;
        }
        
        private void OnRoomMembersDetailReceived(object sender, List<Member> members)
        {
            foreach (var member in members)
            {
                if (member.User.IsMe) _me = member;
                else _opponent = member;
            }
        }

        private void OnNewMessageReceived(object sender, MessageReceiveEvent e)
        {
            _newEventReceived = JsonConvert.DeserializeObject<Event>(e.Message.Data);
            _updateRequired = true;
        }

        private void OnLeftRoom(object sender, Member e)
        {
            if(e.User.IsMe) return;
            _opponentLeft = true;
        }

        private void OnError(object sender, ErrorEvent e)
        {
            Debug.LogError("GsLiveHandler : " + e.Error);
        }
    }
}