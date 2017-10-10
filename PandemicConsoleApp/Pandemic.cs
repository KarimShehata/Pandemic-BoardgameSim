using System;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.Linq;

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

        private readonly int[] _startingHands = { 4, 3, 2 };
        private int _actionCount = 4;
        private int[] _cubeReserve = { 24, 24, 24, 24 };
        private int[] _cures = { 0, 0, 0, 0 };
        // 0-not found -- 1-cure found -- 2-eradecated
        private Difficulty _difficulty;

        private bool _gameEnded = false;
        private int _infectionRateIdx = 0;
        private int[] _infectionRates = { 2, 2, 2, 3, 3, 4, 4 };
        private int _outbreakCounter = 0;
        private int _playerCount;
        private List<int> _researchStationLoactions = new List<int>();
        private int _researchStationReserve = 6;
        private List<Role> _roles = new List<Role>();
        private Stack<Role> _shuffeledRoles;

        #endregion Private Fields

        #region Public Constructors

        public Pandemic(Difficulty difficulty, int playerCount)
        {
            _difficulty = difficulty;
            _playerCount = playerCount;

            Map = new Map();

            InitRoles();

            InitInfectionDeck();

            AddResearchStationToCity(11);  //Atlanta

            Infect();

            InitPlayerDeck();

            InitPlayers();

            PreparePlayerDeck();

            BeginPlay();
        }

        #endregion Public Constructors

        #region Public Methods

        public void AddCubesToCity(int cubeCount, int city)
        {
            var cubeColor = city / 12;

            _cubeReserve[cubeColor] -= cubeCount;

            if (_cubeReserve[cubeColor] < 0)
                throw new NotSupportedException(); //Todo Game Lost: ran out of cubes

            if (Map.Cities[city].Cubes[cubeColor] < 3)
                Map.Cities[city].Cubes[cubeColor] += cubeCount;
            else
                throw new NotImplementedException(); //Todo Outbreak triggered

            if (Map.Cities[city].Cubes[cubeColor] > 3)
                throw new NotSupportedException(); //more than 3 cubes of a single color on one city space
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
            Console.WriteLine("======== Infection Discard Pile ========");
            PrintInfectionDiscardPile();
            Console.WriteLine("======== Player Discard Pile ========");
            PrintPlayerDiscardPile();
            Console.WriteLine("======== ======== ========");
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
                Console.WriteLine($"Turn {roundCounter + 1}:");
                TakeTurn(roundCounter % _playerCount);
            }

        }

        private void DoActions(Player player)
        {
            for (var i = 0; i < _actionCount; i++)
            {
                var availableActions = GetAvailableActions(player);
                Console.WriteLine("==> " + availableActions.Count);
            }
        }

        private void DrawPlayerCards(Player player)
        {
            throw new NotImplementedException();
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

        private List<IAction> GetAvailableActions(Player currentActivePlayer)
        {
            var availableactions = new List<IAction>();

            // Drive / Ferry Actions
            availableactions.AddRange(GetAvailableDriveFerryActions(currentActivePlayer.Location));
            // Direct Flight Actions
            availableactions.AddRange(GetAvailableDirectFlightActions(currentActivePlayer.Hand));
            // Charter Flight Actions
            availableactions.AddRange(GetAvailableCharterFlightActions(currentActivePlayer.Location, currentActivePlayer.Hand));
            // Shuttle Flight Actions
            availableactions.AddRange(GetAvailableShuttleFlightActions(currentActivePlayer.Location));

            // Build Research Station Action
            var buildResearchStationAction = GetBuildResearchStationAction(currentActivePlayer.Location, currentActivePlayer.Hand);
            if (buildResearchStationAction != null)
                availableactions.Add(buildResearchStationAction);

            // Treat Disease Action
            var treatDiseaseAction = GetTreatDiseaseAction(currentActivePlayer.Location);
            if (treatDiseaseAction != null)
                availableactions.Add(treatDiseaseAction);

            // Share Knowledge Action
            var shareKnowledgeAction = GetShareKnowledgeActions(currentActivePlayer);
            if (shareKnowledgeAction.Count > 0)
                availableactions.AddRange(shareKnowledgeAction);

            return availableactions;
        }

        private List<CharterFlightAction> GetAvailableCharterFlightActions(int playerLocation, List<int> playerHand)
        {
            var list = new List<CharterFlightAction>();

            if (!playerHand.Contains(playerLocation)) return list;

            //Charter Flight possible
            for (var i = 0; i < 48; i++) // All Cities
            {
                if (i != playerLocation) // All other Cities are valid destinations
                    list.Add(new CharterFlightAction(playerLocation, i));
            }

            return list;
        }

        private List<DirectFlightAction> GetAvailableDirectFlightActions(List<int> playerHand)
        {
            var list = new List<DirectFlightAction>();

            foreach (var card in playerHand)
            {
                if (card < 48) //if city card
                    list.Add(new DirectFlightAction(card));
            }

            return list;
        }

        private List<DriveFerryAction> GetAvailableDriveFerryActions(int cityId)
        {
            var list = new List<DriveFerryAction>();

            var connectedCities = Map.Cities[cityId].Neighbours;

            foreach (var connectedCity in connectedCities)
            {
                list.Add(new DriveFerryAction(connectedCity.Id));
            }

            return list;
        }

        private List<ShuttleFlightAction> GetAvailableShuttleFlightActions(int playerLocation)
        {
            var list = new List<ShuttleFlightAction>();

            if (_researchStationLoactions.Count <= 1) return list;

            if (!_researchStationLoactions.Contains(playerLocation)) return list;

            foreach (var researchStationLoaction in _researchStationLoactions)
            {
                if (researchStationLoaction != playerLocation)
                    list.Add(new ShuttleFlightAction(researchStationLoaction));
            }

            return list;
        }

        private BuildResearchStationAction GetBuildResearchStationAction(int playerLocation, List<int> playerHand)
        {
            return playerHand.Contains(playerLocation) ? new BuildResearchStationAction(playerLocation) : null;
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

        private List<ShareKnowledgeAction> GetShareKnowledgeActions(Player player)
        {
            //Todo make two way

            var list = new List<ShareKnowledgeAction>();

            if (!player.Hand.Contains(player.Location)) return list;

            var playersAtSameLocation = new List<Player>();

            foreach (var p in Players)
            {
                if (p.Location == player.Location)
                    playersAtSameLocation.Add(p);
            }

            if (playersAtSameLocation.Count < 2) return list;

            foreach (var p in playersAtSameLocation)
            {
                if (p != player)
                    list.Add(new ShareKnowledgeAction(player.Location, p));
            }

            return list;
        }

        private TreatDiseaseAction GetTreatDiseaseAction(int playerLocation)
        {
            return Map.IsCityInfected(playerLocation) ? new TreatDiseaseAction() : null;
        }
        private void Infect()
        {
            for (var i = 3; i > 0; i--)
            {
                for (var j = 0; j < 3; j++)
                {
                    var city = InfectionDrawPile.Pop();

                    AddCubesToCity(i, city);

                    InfectionDiscardPile.Push(city);
                }
            }
        }

        private void InfectCities()
        {
            throw new NotImplementedException();
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
                var player = new Player(i)
                {
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
            DoActions(Players[playerNumber]);
            DrawPlayerCards(Players[playerNumber]);
            InfectCities();
        }

        #endregion Private Methods

    }
}