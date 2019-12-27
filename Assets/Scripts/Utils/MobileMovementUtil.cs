// <copyright file="MobileMovementUtil.cs" company="Firoozeh Technology LTD">
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


/**
* @author Alireza Ghodrati
*/

using System;
using Controllers;
using Handlers;
using Models;
using UnityEngine;

namespace Utils
{
    public class MobileMovementUtil
    {
        private readonly Rigidbody2D _rigidbody2D;
        private readonly Transform _transform;
        private readonly ActionUtil _actionUtil;
        private readonly GsLiveHandler _gsLiveHandler;
        
        private readonly float _moveSpeed;
        private bool _facingRight;
        private float _lastX;
        private readonly bool _isEnemy;
        private bool _sendPosOnce;
        
        
        public MobileMovementUtil(Rigidbody2D rigidbody2D,Transform transform,GameObject bullet,float moveSpeed,bool isEnemy)
        {
            _facingRight = true; // for player1 & negative for player2
            _rigidbody2D = rigidbody2D;
            _transform = transform;
            _moveSpeed = moveSpeed;
            _isEnemy = isEnemy;

            _actionUtil = new ActionUtil(_rigidbody2D,_transform,bullet);
            _gsLiveHandler = new GsLiveHandler();
        }

        public void UpdateMySelf(Vector2 coordinate)
        {
            _rigidbody2D.velocity = _isEnemy ? new Vector2(coordinate.x * _moveSpeed, coordinate.y) : new Vector2(coordinate.x * _moveSpeed,_rigidbody2D.velocity.y);
            Flip(coordinate.x);

            if(_isEnemy) return;
            if (!(Math.Abs(coordinate.x - _lastX) > 0.1f)) return;
            _lastX = coordinate.x;
            _gsLiveHandler.SendMoveToServer(coordinate.x,_rigidbody2D.velocity.y); 
            
            if(_sendPosOnce) return;
             _gsLiveHandler.SendPosToServer(_transform.position.x,_transform.position.y);
            _sendPosOnce = true;
        }
        
        public void UpdatePos(Vector2 coordinate)
        {
            _transform.position = new Vector3(coordinate.x,coordinate.y);
        }

        public void UpdateOpponentStatus()
        {
            if(!_gsLiveHandler.IsUpdateRequired()) return;

            // Update enemy Status
            var newEvent = _gsLiveHandler.GetNewEvent();
            
            var enemyController = GameObject.FindGameObjectWithTag("Enemy")
                .GetComponent<EnemyController>();

            switch (newEvent.Action)
            {
                case EventAction.Move:
                    enemyController.MoveEnemy(newEvent.Payload.X,newEvent.Payload.Y);
                    break;
                case EventAction.Shoot:
                    enemyController.Shoot();
                    break;
                case EventAction.Jump:
                    enemyController.Jump();
                    break;
                case EventAction.Die:
                    break;
                case EventAction.Pos:
                    enemyController.SetPos(newEvent.Payload.X,newEvent.Payload.Y);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Shoot()
        {
            _actionUtil.Shoot(_facingRight);
            
            if(_isEnemy) return;
            _gsLiveHandler.SendShootToServer();
        }

        public void Jump()
        {
            _actionUtil.Jump();
            
            if(_isEnemy) return;
            _gsLiveHandler.SendJumpToServer();
        }
        
        
        private void Flip(float horizontal)
        {
            if ((!(horizontal > 0) || _facingRight) && (!(horizontal < 0) || !_facingRight)) return;
            _facingRight = !_facingRight;
            var scale = _transform.localScale;
            scale.x *= -1;
            _transform.localScale = scale;
        }
        
    }
}