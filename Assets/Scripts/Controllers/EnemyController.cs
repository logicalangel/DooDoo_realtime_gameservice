// <copyright file="EnemyController.cs" company="Firoozeh Technology LTD">
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
using UnityEngine.UI;
using Utils;

/**
* @author Alireza Ghodrati
*/

namespace Controllers
{
    public class EnemyController : MonoBehaviour
    {
        public GameObject Bullet;
        public GameObject Finish;
        public float MoveSpeed;

        private Rigidbody2D _rigidbody2D;
        private DamageUtil _damageUtil;
        private MobileMovementUtil _movementUtil;
        private bool _onceShow;


        private void Start()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            var health = GetComponentInChildren<HealthBar>();
            
            _damageUtil = new DamageUtil(health);   
            _movementUtil = new MobileMovementUtil(_rigidbody2D,transform,Bullet,MoveSpeed,true);
        }
        
         
        public void MoveEnemy(float x,float y)
        {
            _movementUtil.UpdateMySelf(new Vector2(x,y));
        }

        public void SetPos(float x,float y)
        {
            _movementUtil.UpdatePos(new Vector2(x,y));
        }
        

        public void Shoot()
        {
            _movementUtil.Shoot();
        }

        public void Jump()
        {
            _movementUtil.Jump();
        }



        private async void Update()
        {
            if(_damageUtil.IsAlive()) return;

            if(_onceShow) return;
            // You Kill Your Enemy So You Win!
            Finish.SetActive(true);
            Finish.GetComponentInChildren<Text>().text = "You Win!";
            Finish.GetComponentInChildren<Text>().color = Color.black;
            Finish.GetComponentInChildren<Image>().color = Color.cyan;
            
            // Save Win in Server And Check Achievements
            await FileUtil.IncreaseWin();
            _onceShow = true;

        }
        
                
        private void OnTriggerEnter2D(Collider2D c)
        {
            // Hit Bullet -> Damage 10f Amount
            if (c.CompareTag("Bullet"))
                _damageUtil.Damage(10f);
        
        }
    }
}