// <copyright file="MenuController.cs" company="Firoozeh Technology LTD">
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

using System;
using System.Threading.Tasks;
using FiroozehGameService.Core;
using FiroozehGameService.Core.GSLive;
using FiroozehGameService.Handlers;
using FiroozehGameService.Models;
using FiroozehGameService.Models.GSLive;
using FiroozehGameService.Models.GSLive.Command;
using FiroozehGameService.Utils;
using Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utils;
using LogType = FiroozehGameService.Utils.LogType;

/**
* @author Alireza Ghodrati
*/

namespace Controllers
{
    public class MenuController : MonoBehaviour
    {
        public GameObject StartMenu;
        public GameObject LoginMenu;
        public Button StartGameBtn;
        public Text StartMenuText;
        public Text Status;
    
    
        public InputField NickName;
        public InputField Email;
        public InputField Password;
        public Button Submit;
        public GameObject SwitchToRegisterOrLogin;
        public Text LoginErr;
                
        
        private async void Start()
        {
            DontDestroyOnLoad(this);
            if(GameService.IsAuthenticated()) return;
            
            SetEventListeners();
            await ConnectToGamesService();
        }

        private void Update()
        {
            if (!Input.GetKeyDown(KeyCode.Escape)) return;
            GameService.Logout();
            Application.Quit();
        }
        
        
        /// <summary>
        /// Get Last Save User Data
        /// It May Throw Exception
        /// </summary>
        private async Task GetSaveData()
        {
            try
            {
                var save = await GameService.GetSaveGame<Save>();
                FileUtil.SaveWins(save.WinCounts);
                Debug.LogError("GetSaveData Wins : " + save.WinCounts);
            }
            catch (Exception e)
            {
                Debug.LogError("GetSaveData Err : " + e.Message);
            }
        }
        
        /// <summary>
        /// Connect To GameService -> Login Or SignUp
        /// It May Throw Exception
        /// </summary>
        private async Task ConnectToGamesService () {
        //connecting to GamesService
        Status.text = "Status : Connecting...";
        StartGameBtn.interactable = false;
        SwitchToRegisterOrLogin.GetComponent<Button>().onClick.AddListener(() =>
        {
            if (NickName.IsActive())
            {
                NickName.gameObject.SetActive(false);
                SwitchToRegisterOrLogin.GetComponent<Text>().text = "Dont have an account? Register!";
            }
            else
            {
                NickName.gameObject.SetActive(true);
                SwitchToRegisterOrLogin.GetComponent<Text>().text = "Have an Account? Login!";
            }
        });

        if (FileUtil.IsLoginBefore())
        {
            try
            {
                await GameService.Login(FileUtil.GetUserToken());
                
                // Disable LoginUI
                StartMenu.SetActive(true);
                LoginMenu.SetActive(false);
            }
            catch (Exception e)
            {
                Status.color = Color.red;
                if (e is GameServiceException) Status.text = "GameServiceException : " + e.Message;
                else Status.text = "InternalException : " + e.Message;
            }
           
        }
        else
        {
            // Enable LoginUI
            StartMenu.SetActive(false);
            LoginMenu.SetActive(true);
            
            Submit.onClick.AddListener(async () =>
            {
                try
                {
                    if (NickName.IsActive()) // is SignUp
                    {
                        var nickName = NickName.text.Trim();
                        var email = Email.text.Trim();
                        var pass = Password.text.Trim();

                        if (string.IsNullOrEmpty(nickName)
                            && string.IsNullOrEmpty(email)
                            && string.IsNullOrEmpty(pass))
                            LoginErr.text = "Invalid Input!";
                        else
                        {
                            var userToken = await GameService.SignUp(nickName, email, pass);
                            FileUtil.SaveUserToken(userToken);
                        }

                    }
                    else
                    {
                        var email = Email.text.Trim();
                        var pass = Password.text.Trim();

                        if (string.IsNullOrEmpty(email) && string.IsNullOrEmpty(pass))
                            LoginErr.text = "Invalid Input!";
                        else
                        {
                            var userToken = await GameService.Login(email, pass);
                            FileUtil.SaveUserToken(userToken);
                            
                            // Disable LoginUI
                            StartMenu.SetActive(true);
                            LoginMenu.SetActive(false);
                        }
                    }
                }
                catch (Exception e)
                {
                    if (e is GameServiceException) LoginErr.text = "GameServiceException : " + e.Message;
                    else LoginErr.text = "InternalException : " + e.Message;
                }
               
            });
        }        
    }

        /// <summary>
        /// Set Event Listeners
        /// </summary>
        private void SetEventListeners()
        {
            RealTimeEventHandlers.SuccessfullyLogined += OnSuccessfullyLogined;
            RealTimeEventHandlers.Error += OnError;
                
            RealTimeEventHandlers.JoinedRoom += OnJoinRoom;
            RealTimeEventHandlers.LeftRoom += LeftRoom;
            LogUtil.LogEventHandler += LogEventHandler;
        }

        private void LogEventHandler(object sender, Log e)
        {
            if(e.Type == LogType.Normal) Debug.Log(e.Txt);
            else Debug.LogError(e.Txt);
        }
        
        private void LeftRoom(object sender, Member e)
        {
            Debug.Log("LeftRoom : " + e.Name);
        }


        private void OnJoinRoom(object sender, JoinEvent e)
        {
            Debug.Log("OnJoinRoom : " + e.JoinData.JoinedMember.Name);
            var activeScene = SceneManager.GetActiveScene();
            // Go To GameScene When Joined To Room
            if(activeScene.name == "MenuScene")
               SceneManager.LoadScene("GameScene");
        }

        private void OnError(object sender, ErrorEvent e)
        {
            
        }

        private void OnSuccessfullyLogined(object sender, EventArgs e)
        {
            Status.text = "Status : Connected!";
            StartGameBtn.interactable = true;
            StartGameBtn.onClick.AddListener(async () =>
            {
                await GameService.GSLive.RealTime.AutoMatch(new GSLiveOption.AutoMatchOption("DooDoo",2,2));
                
                StartMenuText.text = "MatchMaking...";
                StartGameBtn.interactable = false;
            });
        }
    }
}