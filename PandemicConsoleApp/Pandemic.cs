using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using PandemicConsoleApp.Actions;
using Action = PandemicConsoleApp.Actions.Action;

namespace PandemicConsoleApp
{
    internal class Pandemic
    {

        #region Public Fields

        public Stack<int> InfectionDiscardPile = new Stack<int>();
        public Stack<int> InfectionDrawPile;
        public Map Map;
        public Stack<int> PlayerDiscardPile = new Stack<int>();
        public Stack<int> PlayerDrawPile;
        public List<Player> Players = new List<Player>();

        #endregion Public Fields

        #region Private Fields

        private const int ActionCount = 4;
        private const int FullCubeCount = 24;
        private const int HandLimit = 7;
        private readonly int[] _cubeReserve = { FullCubeCount, FullCubeCount, FullCubeCount, FullCubeCount };
        private readonly int[] _cures = { 0, 0, 0, 0 };
        // 0-not found -- 1-cure found -- 2-eradecated
        private readonly Difficulty _difficulty;
        private readonly int[] _startingHands = { 4, 3, 2 };

        private List<int> _citiesWithOutbreaksThisTurn = new List<int>();
        private bool _gameEnded = false;

        private int _infectionRateIdx = 0;

        private int[] _infectionRates = { 2, 2, 2, 3, 3, 4, 4 };

        private int _outbreakCounter = 0;

        private bool _output = false;

        private int _playerCount;

        private List<int> _researchStationLoactions = new List<int>();

        private int _researchStationReserve = 6;

        private List<Role> _roles = new List<Role>();

        private Stack<Role> _shuffeledRoles;

        #endregion Private Fields

        #region Public Constructors

        public Pandemic(Difficulty difficulty, int playerCount, bool output = false)
        {
            _difficulty = difficulty;
            _playerCount = playerCount;
            _output = output;

            Map = new Map();

            InitRoles();

            InitInfectionDeck();

            AddResearchStationToCity(11);  //Atlanta

            InfectFirst9Cities();

            InitPlayerDeck();

            InitPlayers();

            PreparePlayerDeck();

            BeginPlay();
        }

        #endregion Public Constructors

        #region Private Enums

        private enum DiseaseColor
        {
            Blue,
            Black,
            Red,
            Yellow
        }

        #endregion Private Enums

        #region Public Methods

        public void AddCubesToCity(int cubeCount, int city, int diseaseColor)
        {
            do
            {
                _cubeReserve[diseaseColor]--;

                if (_cubeReserve[diseaseColor] < 0)
                {
                    _gameEnded = true;
                    return;
                }

                if (Map.Cities[city].Cubes[diseaseColor] < 3)
                    Map.Cities[city].Cubes[diseaseColor]++;
                else
                {
                    if (!_citiesWithOutbreaksThisTurn.Contains(city))
                        TriggerOutbreak(city, diseaseColor);
                    cubeCount = 0;
                }

                cubeCount--;
            } while (cubeCount > 0);
        }

        public void PrintBoardState()
        {
            Console.WriteLine("======== Map State ========");
            Map.PrintState();
            Console.WriteLine("======== Player Locations ========");
            PrintPlayerInfo();
            Console.WriteLine("======== Research Stations ========");
            PrintReseachStations();
            Console.WriteLine("======== Cure State ========");
            PrintCureState();
            Console.WriteLine("======== Cube State ========");
            PrintCubeState();
            Console.WriteLine("======== Infection DiscardPlayerCard Pile ========");
            PrintInfectionDiscardPile();
            Console.WriteLine("======== Player DiscardPlayerCard Pile ========");
            PrintPlayerDiscardPile();
            Console.WriteLine("======== ======== ========");
        }

        public void RemoveCubeFromCity(int playerLocation, int diseaseColor)
        {
            _cubeReserve[diseaseColor]++;

            Map.Cities[playerLocation].Cubes[diseaseColor]--;

            if (_cubeReserve[diseaseColor] == FullCubeCount && _cures[diseaseColor] == 1)
                _cures[diseaseColor]++;
        }

        #endregion Public Methods

        #region Private Methods

        private static void PrintInfectioDeck(int[] shuffeledInfectionDeck)
        {
            foreach (var i in shuffeledInfectionDeck)
            {
                Console.WriteLine(i);
            }
        }

        private void AddResearchStationToCity(int city)
        {
            if (_researchStationReserve == 0)
                throw new ArgumentOutOfRangeException(); //Todo handle replacement of research station

            _researchStationReserve--;
            _researchStationLoactions.Add(city);
        }

        private void BeginPlay()
        {
            var roundCounter = 0;
            while (!_gameEnded)
            {
                if (_output)
                    Console.WriteLine($"Turn #{roundCounter + 1}");

                TakeTurn(roundCounter % _playerCount);
                roundCounter++;
            }
        }

        private void CheckAndDiscardToHandLimit(Player player)
        {
            while (player.Hand.Count > HandLimit)
            {
                player.Hand.RemoveAt(Program.random.Next(player.Hand.Count)); //todo add palyer choice logic
            }
        }

        private void CheckIfWon()
        {
            if (_cures.All(x => x > 0))
            {
                Program.winCounnt++;
                _gameEnded = true;
                Console.WriteLine("All 4 Cures discovered!");
            }
        }

        private void DoActions(Player player)
        {
            for (var i = 0; i < ActionCount; i++)
            {
                var availableActions = ActionService.GetAvailableActions(player, Players, Map, _researchStationLoactions, _cures);

                if (_output)
                {
                    Console.WriteLine($"P#{player.Id} {player.Role} @ {Map.CityNames[player.Location]}- Action {i + 1}/{ActionCount}");

                    Console.Write("Hand: ");
                    foreach (var card in player.Hand)
                    {
                        if (card < 48)
                            Console.Write(Map.CityNames[card] + ", ");
                        else
                            Console.Write(card + ", ");
                    }
                    Console.WriteLine();

                    PrintAvailableActions(availableActions);
                }

                var choice = Program.random.Next(availableActions.Count); //GetPlayerChoice(availableActions); Todo

                if (_output)
                {
                    Console.WriteLine($"Action {choice} was chosen randomly");
                    Console.WriteLine("Press Spacebar to continue");
                    do
                    {
                    } while (Console.ReadKey(true).Key != ConsoleKey.Spacebar);
                }

                PerformAction(player, availableActions[choice]);
            }

        }

        private void DrawPlayerCards(Player player)
        {
            if (PlayerDrawPile.Count < 2)
            {
                _gameEnded = true;  //Todo Game lost ran out of player cards
                return;
            }

            for (var i = 0; i < 2; i++)
            {
                var card = PlayerDrawPile.Pop();

                if (card != 53)
                    player.Hand.Add(card);
                else
                    TriggerEpidemic();
            }
            CheckAndDiscardToHandLimit(player);
        }

        private List<int> DrawStartingHand()
        {
            var hand = new List<int>();

            for (var j = 0; j < _startingHands[_playerCount - 2]; j++)
            {
                hand.Add(PlayerDrawPile.Pop());
            }

            return hand;
        }

        private int GetPlayerChoice(List<Action> availableActions)
        {
            int choice;
            bool isValidInput;

            do
            {
                isValidInput = false;

                Console.Write("Choose action: ");
                var input = Console.ReadLine();

                if (int.TryParse(input, out choice))
                {
                    isValidInput = choice < availableActions.Count && choice >= 0;
                }

                if (!isValidInput)
                {
                    Console.WriteLine("Input invalid");
                }

            } while (!isValidInput);

            return choice;
        }

        private string GetPlayerHandString(Player player)
        {
            var returnString = string.Empty;
            foreach (var i in player.Hand)
            {
                returnString += i + ", ";
            }

            return returnString.Remove(returnString.Length - 2);
        }

        private void Infect(int numCubes = 1)
        {
            if (InfectionDrawPile.Count == 0) throw new Exception(); // somehow the infection drawpile was emptied

            var city = InfectionDrawPile.Pop();
            InfectionDiscardPile.Push(city);
            var diseasColor = city / 12;

            if (_output)
                Console.WriteLine($"{numCubes} {(DiseaseColor)diseasColor} added to {Map.CityNames[city]}");

            AddCubesToCity(numCubes, city, diseasColor);
        }

        private void InfectCities()
        {
            for (var i = 0; i < _infectionRates[_infectionRateIdx]; i++)
            {
                Infect();
            }
        }

        private void InfectFirst9Cities()
        {
            for (var i = 0; i < 3; i++)
            {
                for (var j = 0; j < 3; j++)
                {
                    Infect(3 - i);
                }
            }
        }

        private void InitInfectionDeck()
        {
            var infectionDeck = new int[48];

            for (var i = 0; i < infectionDeck.Length; i++)
            {
                infectionDeck[i] = i;
            }

            var shuffeledInfectionDeck = Utils.Shuffle(infectionDeck);

            InfectionDrawPile = new Stack<int>(shuffeledInfectionDeck);
        }

        private void InitPlayerDeck()
        {
            var playerDeck = new int[53];

            for (var i = 0; i < playerDeck.Length; i++)
            {
                playerDeck[i] = i;
            }

            var shuffeledplayerDeck = Utils.Shuffle(playerDeck);

            PlayerDrawPile = new Stack<int>(shuffeledplayerDeck);
        }

        private void InitPlayers()
        {
            for (var i = 0; i < _playerCount; i++)
            {
                var player = new Player
                {
                    Id = i,
                    Location = 11,
                    Role = PickRole(),
                    Hand = DrawStartingHand()
                };

                Players.Add(player);
            }
        }

        private void InitRoles()
        {
            var values = Enum.GetValues(typeof(Role));

            foreach (Role value in values)
            {
                _roles.Add(value);
            }

            var rnd = new Random();
            _shuffeledRoles = new Stack<Role>(_roles.OrderBy(y => rnd.Next()));
        }

        private void PerformAction(Player player, Action action)
        {

            switch (action.ActionType)
            {
                case ActionType.DriveFerry:
                    var driveFerryAction = action as DriveFerryAction;
                    player.Location = driveFerryAction.Destiantion;
                    break;
                case ActionType.DirectFlight:
                    var directFlightAction = action as DirectFlightAction;
                    DiscardPlayerCard(player, directFlightAction.Destiantion);
                    player.Location = directFlightAction.Destiantion;
                    break;
                case ActionType.CharterFlight:
                    var charterFlightAction = action as CharterFlightAction;
                    DiscardPlayerCard(player, charterFlightAction.Cost);
                    player.Location = charterFlightAction.Destiantion;
                    break;
                case ActionType.ShuttleFlight:
                    var shuttleFlightAction = action as ShuttleFlightAction;
                    player.Location = shuttleFlightAction.Destiantion;
                    break;
                case ActionType.BuildResearchStation:
                    var buildResearchStationAction = action as BuildResearchStationAction;
                    if (buildResearchStationAction.Cost >= 0)   // Action can be free because of Operations Expert
                    {
                        DiscardPlayerCard(player, buildResearchStationAction.Cost);
                    }
                    _researchStationLoactions.Add(buildResearchStationAction.Cost);
                    break;
                case ActionType.TreatDisease:
                    var treatDiseaseAction = action as TreatDiseaseAction;
                    TreatDisease(player, treatDiseaseAction.DiseaseColor);
                    break;
                case ActionType.ShareKnowledge:
                    var shareKnowledgeAction = action as ShareKnowledgeAction;
                    shareKnowledgeAction.GivingPlayer.Hand.Remove(shareKnowledgeAction.LocationCard);
                    shareKnowledgeAction.ReceivingPlayer.Hand.Add(shareKnowledgeAction.LocationCard);
                    CheckAndDiscardToHandLimit(shareKnowledgeAction.ReceivingPlayer);
                    break;
                case ActionType.DiscoverCure:
                    var discoverCureAction = action as DiscoverCureAction;
                    foreach (var card in discoverCureAction.Cards)
                    {
                        discoverCureAction.Player.Hand.Remove(card);
                    }
                    _cures[discoverCureAction.CureColor]++;
                    CheckIfWon();
                    if (_cubeReserve[discoverCureAction.CureColor] == FullCubeCount)
                        _cures[discoverCureAction.CureColor]++;
                    break;
                case ActionType.Pass:
                    break;
                case ActionType.OperationsExpertSpecialFlightAction:
                    var operationsExpertSpecialFlightAction = action as OperationsExpertSpecialFlightAction;
                    DiscardPlayerCard(player, operationsExpertSpecialFlightAction.Cost);
                    player.Location = operationsExpertSpecialFlightAction.Destiantion;
                    break;
                case ActionType.DispatherSpecialFlightAction:
                    var dispatherSpecialFlightAction = action as DispatherSpecialFlightAction;
                    dispatherSpecialFlightAction.MovingPlayer.Location =
                        dispatherSpecialFlightAction.DestinationPlayer.Location;

                    CheckAndTriggerMedicSpecialAction(dispatherSpecialFlightAction.MovingPlayer, dispatherSpecialFlightAction); // Trigger Medic Special Ability if Medic was moved by Dispatcher
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            CheckAndTriggerMedicSpecialAction(player, action);
        }

        private void CheckAndTriggerMedicSpecialAction(Player player, Action action)
        {
            if (player.Role != Role.Medic || !(action is MovementAction)) return;

            for (var i = 0; i < _cures.Length; i++)   // Medic Special Ability
            {
                if (_cures[i] > 0)
                    TreatDisease(player, i);
            }
        }

        private void DiscardPlayerCard(Player player, int card)
        {
            player.Hand.Remove(card);
            PlayerDiscardPile.Push(card);
        }

        private Role PickRole()
        {
            return _shuffeledRoles.Pop();
        }

        private void PreparePlayerDeck()
        {
            var piles = PlayerDrawPile.ToArray().Split(4 + (int)_difficulty);

            var completePile = new List<int>();

            var rnd = new Random();

            foreach (var pile in piles)
            {
                var list = pile.ToList();

                list.Insert(rnd.Next(0, pile.Count()), 53);  //Epdemic Card

                completePile.AddRange(list);
            }

            PlayerDrawPile = new Stack<int>(completePile);
        }

        private void PrintAvailableActions(List<Action> availableActions)
        {
            for (var i = 0; i < availableActions.Count; i++)
            {
                availableActions[i].PrintAction(i);
            }
        }

        private void PrintCubeState()
        {
            var totalReserveCubes = _cubeReserve[0] + _cubeReserve[1] + _cubeReserve[2] + _cubeReserve[3];

            Console.WriteLine($"Cube Reserves {_cubeReserve[0]}/{_cubeReserve[1]}/{_cubeReserve[2]}/{_cubeReserve[3]} = {totalReserveCubes}");
        }

        private void PrintCureState()
        {
            Console.WriteLine($"Cures {_cures[0]}/{_cures[1]}/{_cures[2]}/{_cures[3]}");
        }

        private void PrintInfectionDiscardPile()
        {
            foreach (var i in InfectionDiscardPile.ToArray().Reverse())
            {
                Console.Write($"{i}, ");
            }

            Console.WriteLine();
        }

        private void PrintPlayerDiscardPile()
        {
            foreach (var i in PlayerDiscardPile.ToArray().Reverse())
            {
                Console.Write($"{i}, ");
            }

            Console.WriteLine();
        }

        private void PrintPlayerInfo()
        {
            foreach (var player in Players)
            {
                Console.WriteLine($"Player #{player.Id} the {player.Role} is in City #{player.Location} and has {GetPlayerHandString(player)} in hand");
            }
        }

        private void PrintReseachStations()
        {
            foreach (var citiyWithResearchStation in _researchStationLoactions)
            {
                Console.Write(citiyWithResearchStation + ", ");
            }

            Console.WriteLine();
        }

        private void TakeTurn(int playerNumber)
        {
            Console.WriteLine("===== Action Phase =====");
            DoActions(Players[playerNumber]);
            Console.WriteLine("===== Draw Phase =====");
            DrawPlayerCards(Players[playerNumber]);
            Console.WriteLine("===== Infection Phase =====");
            InfectCities();

            _citiesWithOutbreaksThisTurn.Clear();
        }

        private void TreatDisease(Player player, int diseaseColor)
        {
            var isMedicOrCureFound = _cures[diseaseColor] > 0 || player.Role == Role.Medic;

            do
            {
                RemoveCubeFromCity(player.Location, diseaseColor);
            } while (Map.Cities[player.Location].Cubes[diseaseColor] > 0 && isMedicOrCureFound); // Medic Special Ability
        }

        private void TriggerEpidemic()
        {
            // 1. Increase
            _infectionRateIdx++;

            // 2. Infect
            var list = InfectionDrawPile.ToList();

            var infectedCity = list[list.Count - 1];
            list.RemoveAt(list.Count - 1);
            InfectionDiscardPile.Push(infectedCity);

            InfectionDrawPile = new Stack<int>(list);

            if (_output)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Epidemic @ {Map.CityNames[infectedCity]}");
                Console.ForegroundColor = ConsoleColor.White;
            }

            if (_cures[infectedCity / 12] < 2)
            {
                AddCubesToCity(3, infectedCity, infectedCity / 12);
            }

            _citiesWithOutbreaksThisTurn.Clear();  // remove epidemic city from list

            // 3. Intensify
            var shuffledInfectionDiscardPile = Utils.Shuffle(InfectionDiscardPile);
            foreach (var card in shuffledInfectionDiscardPile)
            {
                InfectionDrawPile.Push(card);
            }


        }

        private void TriggerOutbreak(int city, int diseaseColor)
        {
            _citiesWithOutbreaksThisTurn.Add(city);

            foreach (var neighbour in Map.Cities[city].Neighbours)
            {
                AddCubesToCity(1, neighbour.Id, diseaseColor);
            }

            if (_output)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Outbreak of {(DiseaseColor)diseaseColor} @ {Map.CityNames[city]}");
                Console.ForegroundColor = ConsoleColor.White;
            }

            //todo handle outbreak loops
        }

        #endregion Private Methods

    }
}