// <copyright file="GameMainController.cs" company="Firoozeh Technology LTD">
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
    public class GameMainController : MonoBehaviour
    {
        public GameObject Bullet;
        public GameObject Finish;
        public Button Jump, Shoot;

        public float MoveSpeed;
    
        private DamageUtil _damageUtil;
        private MobileMovementUtil _movementUtil;
        private MobileInputController _mobileInputController;

        private bool _onceShow;

    
        // Start is called before the first frame update
        void Start()
        {
            var rb = GetComponent<Rigidbody2D>();
            var health = GetComponentInChildren<HealthBar>();
            _mobileInputController = GetComponentInChildren<MobileInputController>();

            _damageUtil = new DamageUtil(health);
            _movementUtil = new MobileMovementUtil(rb,transform,Bullet,MoveSpeed,false);
       
        
            // Set Movement Events
            Jump.onClick.AddListener(() =>
            {
                _movementUtil.Jump();
            });
            Shoot.onClick.AddListener(() =>
            {
                _movementUtil.Shoot();
            });
        }

        void Update()
        {
            _movementUtil.UpdateMySelf(_mobileInputController.Coordinate());
            _movementUtil.UpdateOpponentStatus();
           
            // You Enemy kill You So You Die!
            if(_damageUtil.IsAlive()) return;
            if(_onceShow) return;
            
            Finish.SetActive(true);
            Finish.GetComponentInChildren<Text>().text = "You Dead!";
            Finish.GetComponentInChildren<Text>().color = Color.white;
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
