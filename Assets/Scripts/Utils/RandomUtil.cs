// <copyright file="RandomUtil.cs" company="Firoozeh Technology LTD">
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


using UnityEngine;

namespace Utils
{
    public class RandomUtil : MonoBehaviour
    {
        public GameObject Obj;
        
        private void Start()
        {
            Spawn();
        }

        private void Update()
        {
            
        }

        private void Spawn ()
        {
            var spawnPosition = new Vector3
            {
                x = Random.Range(-8, 8),
                y = Random.Range(-0.5f, -2.5f)
            };

            var obj = Instantiate(Obj, spawnPosition, Quaternion.identity);
            obj.SetActive(true);
        }
    }
}