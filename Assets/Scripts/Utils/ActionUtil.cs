// <copyright file="ActionUtil.cs" company="Firoozeh Technology LTD">
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

using UnityEngine;

/**
* @author Alireza Ghodrati
*/

namespace Utils
{
    public class ActionUtil
    {
        private Rigidbody2D _rigidbody2D;
        private readonly Transform _transform;
        private readonly GameObject _bullet;
        
        public ActionUtil(Rigidbody2D rigidbody2D,Transform transform,GameObject bullet)
        {
            _rigidbody2D = rigidbody2D;
            _transform = transform;
            _bullet = bullet;
        }

        public void Shoot(bool facingRight)
        {
            var obj = Object.Instantiate(_bullet, new Vector3(facingRight ? _transform.position.x + 1 : _transform.position.x - 1 , _transform.position.y), Quaternion.identity);
            obj.tag = "Bullet";
            obj.GetComponent<Rigidbody2D>().velocity = new Vector2(facingRight ? 6 : -6, 0);
        }

        public void Jump()
        {
            _rigidbody2D.AddForce(new Vector2(0, 200));
        }
    }
}