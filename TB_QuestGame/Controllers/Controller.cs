﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TB_QuestGame
{
    /// <summary>
    /// controller for the MVC pattern in the application
    /// </summary>
    public class Controller
    {
        #region FIELDS

        private ConsoleView _gameConsoleView;
        private Player _gamePirate;
        private Universe _gameUniverse;
        private IslandLocation _currentLocation;
        private bool _playingGame;

        #endregion

        #region PROPERTIES


        #endregion

        #region CONSTRUCTORS

        public Controller()
        {
            //
            // setup all of the objects in the game
            //
            InitializeGame();

            //
            // begins running the application UI
            //
            ManageGameLoop();
        }

        #endregion

        #region METHODS

        /// <summary>
        /// initialize the major game objects
        /// </summary>
        private void InitializeGame()
        {
            _gamePirate = new Player();
            _gameUniverse = new Universe();
            _gameConsoleView = new ConsoleView(_gamePirate, _gameUniverse);
            _playingGame = true;

            Console.CursorVisible = false;
        }

        /// <summary>
        /// method to manage the application setup and game loop
        /// </summary>
        private void ManageGameLoop()
        {
            PlayerAction travelerActionChoice = PlayerAction.None;                                             

            //
            // display splash screen
            //
            _playingGame = _gameConsoleView.DisplaySpashScreen();

            //
            // player chooses to quit
            //
            if (!_playingGame)
            {
                Environment.Exit(1);
            }

            //
            // display introductory message
            //
            _gameConsoleView.DisplayGamePlayScreen("Quest Intro", Text.QuestIntro(), ActionMenu.MissionIntro, "");
            _gameConsoleView.GetContinueKey();

            //
            // initialize the mission traveler
            // 
            InitializeMission(travelerActionChoice);

            //
            // prepare game play screen
            //
            _currentLocation = _gameUniverse.GetIslandLocationById(_gamePirate.IslandLocationId);
            _gameConsoleView.DisplayGamePlayScreen("Current Location", Text.CurrentLocationInfo(_currentLocation), ActionMenu.MainMenu, "");
            _gameConsoleView.DisplayColoredText("", PlayerAction.LookAround, _currentLocation);

            //
            // game loop
            //
            while (_playingGame)
            {
                //
                // update all game stats/info
                //
                UpdateGameStatus();

                travelerActionChoice = _gameConsoleView.GetActionMenuChoice(ActionMenu.MainMenu);
      
                //
                // choose an action based on the user's menu choice
                //
                switch (travelerActionChoice)
                {
                    case PlayerAction.None:
                        break;

                    case PlayerAction.EditPlayerInfo:                       
                        _gameConsoleView.DisplayEditPirateInformation(_currentLocation.CommonName, travelerActionChoice, _currentLocation);
                        _gameConsoleView.DisplayGamePlayScreen("Current Location", Text.CurrrentLocationInfo(), ActionMenu.MainMenu, "");
                        break;

                    case PlayerAction.PlayerInfo:
                        _gameConsoleView.DisplayPirateInfo();
                        _gameConsoleView.DisplayColoredText(_currentLocation.CommonName, travelerActionChoice, _currentLocation);
                        break;

                    case PlayerAction.ListDestinations:
                        _gameConsoleView.DisplayListOfIslandLocations();
                        _gameConsoleView.DisplayColoredText(_currentLocation.CommonName, travelerActionChoice, _currentLocation);
                        break;

                    case PlayerAction.LookAround:
                        _gameConsoleView.DisplayLookAround();
                        _gameConsoleView.DisplayColoredText(_currentLocation.CommonName, travelerActionChoice, _currentLocation);
                        break;

                    case PlayerAction.Travel:

                        //
                        // determine if the player has a ship in order to travel
                        //
                        if (_gamePirate.Ship == Player.ShipType.None)
                        {
                            _gamePirate.ShipOwner = false;
                            _gameConsoleView.DisplayInputErrorMessage("You currently do not own a ship needed to travel. Obtain a ship, and try again.");
                            break;
                        }
                        else
                        {
                            _gamePirate.ShipOwner = true;
                        }

                        //
                        // get new location choice and update current location
                        //
                        _gamePirate.IslandLocationId = _gameConsoleView.DisplayGetNextIslandLocation(travelerActionChoice, _currentLocation);
                        _currentLocation = _gameUniverse.GetIslandLocationById(_gamePirate.IslandLocationId);                        

                        //
                        // display game play screen with current location info and coordiantes
                        //
                        _gameConsoleView.DisplayGamePlayScreen("Current Location", Text.CurrentLocationInfo(_currentLocation), ActionMenu.MainMenu, "");
                        _gameConsoleView.DisplayColoredText(_currentLocation.CommonName, PlayerAction.LookAround, _currentLocation);
                        break;

                    case PlayerAction.PirateLocationsVisited:
                        _gameConsoleView.DisplayLocationsVisited();
                        _gameConsoleView.DisplayColoredText(_currentLocation.CommonName, travelerActionChoice, _currentLocation);                    
                        break;

                    case PlayerAction.Exit:
                        _gameConsoleView.DisplayClosingScreen();
                        _playingGame = false;
                        break;

                    default:
                        break;
                }
            }

            //
            // close the application
            //
            Environment.Exit(1);
        }

        /// <summary>
        /// initialize the player info
        /// </summary>
        private void InitializeMission(PlayerAction choosenAction)
        {
            //Player pirate = _gameConsoleView.GetInitialPirateInfo();
            Player pirate = new Player();

            //_gamePlayer.Age = pirate.Age;
            //_gamePlayer.Gender = pirate.Gender;
            //_gamePlayer.Personality = pirate.Personality;

            _gamePirate.Name = _gameConsoleView.GetPirateName(pirate, choosenAction, _currentLocation);
            _gamePirate.Age = 50;
            _gamePirate.Gender = Character.GenderType.MALE;
            _gamePirate.Personality = true;
            //_gamePlayer.Name = "Bill";

            _gamePirate.ShipName = "Queen Anne's Revenge";
            _gamePirate.ShipOwner = true;
            _gamePirate.Coin = 1000000;
            _gamePirate.Weapon = Player.WeaponType.DYNAMITE;
            _gamePirate.Ship = Player.ShipType.BritishManOWar;

            //
            // echo the pirates's info
            //
           _gameConsoleView.DisplayGamePlayScreen("Quest Setup - Complete", Text.InitializeMissionEchoPirateInfo(pirate), ActionMenu.MissionIntro, "");
           _gameConsoleView.DisplayColoredText("", choosenAction, _currentLocation);
            _gameConsoleView.GetContinueKey();
        }

        /// <summary>
        /// updates all the game's info/stats
        /// </summary>
        private void UpdateGameStatus()
        {
            if (!_gamePirate.HasVisited(_currentLocation.IslandLocationID))
            {
                //
                // add a new location to visited list
                //
                _gamePirate.IslandLocationsVisited.Add(_currentLocation.IslandLocationID);
            }
        }





        #endregion
    }
}
