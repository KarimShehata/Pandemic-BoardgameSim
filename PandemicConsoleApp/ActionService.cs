using System;
using System.Collections.Generic;
using PandemicConsoleApp.Actions;
using Action = PandemicConsoleApp.Actions.Action;

namespace PandemicConsoleApp
{
    internal class ActionService
    {

        #region Public Methods

        public static List<Action> GetAvailableActions(Player currentActivePlayer, List<Player> players, Map map, List<int> researchStationLocations, CureState[] cures)
        {
            var availableactions = new List<Action> { new PassAction() };

            // Drive / Ferry Actions
            availableactions.AddRange(GetAvailableDriveFerryActions(currentActivePlayer.Location, map));
            // Direct Flight Actions
            availableactions.AddRange(GetAvailableDirectFlightActions(currentActivePlayer.Hand, currentActivePlayer.Location));
            // Charter Flight Actions
            availableactions.AddRange(GetAvailableCharterFlightActions(currentActivePlayer.Location, currentActivePlayer.Hand));
            // Shuttle Flight Actions
            availableactions.AddRange(GetAvailableShuttleFlightActions(currentActivePlayer.Location, researchStationLocations));

            // Build Research Station Action
            var buildResearchStationAction = GetBuildResearchStationAction(currentActivePlayer, researchStationLocations);
            if (buildResearchStationAction != null)
                availableactions.Add(buildResearchStationAction);

            // Treat Disease Action
            availableactions.AddRange(GetTreatDiseaseAction(map, currentActivePlayer.Location));

            // Share Knowledge Action
            availableactions.AddRange(GetShareKnowledgeActions(currentActivePlayer, players));

            // Discover Cure Action
            availableactions.AddRange(GetDiscoverCureAction(currentActivePlayer, researchStationLocations, cures));

            if (currentActivePlayer.Role == Role.Dispatcher) // Dispatcher Special Actions
            {
                availableactions.AddRange(GetAvailableDispatherSpecialFlightActions(currentActivePlayer, players));
                availableactions.AddRange(GetAvailableDispatherSpecialMoveActions(currentActivePlayer, players, map, researchStationLocations));
            }
            else if (currentActivePlayer.Role == Role.OperationsExpert && researchStationLocations.Contains(currentActivePlayer.Location)) // Operations Expert Special Action
                availableactions.AddRange(GetAvailableOperationsExpertSpecialActions(currentActivePlayer));


            return availableactions;
        }

        private static List<MovementAction> GetAvailableDispatherSpecialMoveActions(Player currentActivePlayer, List<Player> players, Map map, List<int> researchStationLocations)
        {
            var list = new List<MovementAction>();

            foreach (var player in players)
            {
                if (currentActivePlayer == player) continue;

                // Todo support passive actions

                // Drive / Ferry Actions
                list.AddRange(GetAvailableDriveFerryActions(player.Location, map));
                // Direct Flight Actions
                list.AddRange(GetAvailableDirectFlightActions(currentActivePlayer.Hand, player.Location));
                // Charter Flight Actions
                list.AddRange(GetAvailableCharterFlightActions(player.Location, currentActivePlayer.Hand));
                // Shuttle Flight Actions
                list.AddRange(GetAvailableShuttleFlightActions(player.Location, researchStationLocations));
            }

            return list;
        }

        #endregion Public Methods

        #region Private Methods

        private static List<CharterFlightAction> GetAvailableCharterFlightActions(int playerLocation, List<int> playerHand)
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

        private static List<DirectFlightAction> GetAvailableDirectFlightActions(List<int> playerHand, int playerLocation)
        {
            var list = new List<DirectFlightAction>();

            foreach (var card in playerHand)
            {
                if (card < 48 && card != playerLocation) //if city card and not current city
                    list.Add(new DirectFlightAction(card));
            }

            return list;
        }

        private static List<DispatherSpecialFlightAction> GetAvailableDispatherSpecialFlightActions(Player player, List<Player> players)
        {
            var list = new List<DispatherSpecialFlightAction>();

            foreach (var movingPlayer in players)
            {
                foreach (var destinationPlayer in players)
                {
                    if (movingPlayer == destinationPlayer) continue;

                    list.Add(new DispatherSpecialFlightAction(movingPlayer, destinationPlayer));
                }
            }

            return list;
        }

        private static List<DriveFerryAction> GetAvailableDriveFerryActions(int cityId, Map map)
        {
            var list = new List<DriveFerryAction>();

            var connectedCities = map.Cities[cityId].Neighbours;

            foreach (var connectedCity in connectedCities)
            {
                list.Add(new DriveFerryAction(connectedCity.Id));
            }

            return list;
        }

        private static List<OperationsExpertSpecialFlightAction> GetAvailableOperationsExpertSpecialActions(Player player)
        {
            var list = new List<OperationsExpertSpecialFlightAction>();

            for (var destiantion = 0; destiantion < 48; destiantion++)
            {
                if (destiantion == player.Location) continue;

                foreach (var card in player.Hand)
                {
                    if (card > 47) continue;  // not city card

                    if (card == player.Location) continue; // charter flight

                    if (card == destiantion) continue; // direct flight

                    list.Add(new OperationsExpertSpecialFlightAction(destiantion, card));
                }
            }

            return list;
        }

        private static List<ShuttleFlightAction> GetAvailableShuttleFlightActions(int playerLocation, List<int> reseachStationLocations)
        {
            var list = new List<ShuttleFlightAction>();

            if (reseachStationLocations.Count <= 1) return list;

            if (!reseachStationLocations.Contains(playerLocation)) return list;

            foreach (var researchStationLoaction in reseachStationLocations)
            {
                if (researchStationLoaction != playerLocation)
                    list.Add(new ShuttleFlightAction(researchStationLoaction));
            }

            return list;
        }

        private static BuildResearchStationAction GetBuildResearchStationAction(Player player, List<int> reseachStationLocations)
        {
            if (reseachStationLocations.Contains(player.Location))
                return null;

            if (player.Role == Role.OperationsExpert)       // Operations Expert Special Ability
                return new BuildResearchStationAction(player.Location, true);

            if (player.Hand.Contains(player.Location))
                return new BuildResearchStationAction(player.Location);

            return null;
        }

        private static List<List<int>> GetCombinations(List<int> list, int combinationLength)
        {
            var combinations = new List<List<int>>();

            var count = Math.Pow(2, list.Count);
            for (var i = 1; i < count; i++)
            {
                var str = Convert.ToString(i, 2).PadLeft(list.Count, '0');
                var combination = new List<int>();
                for (var j = 0; j < str.Length; j++)
                {
                    if (str[j] == '1')
                    {
                        combination.Add(list[j]);
                    }
                }

                if (combination.Count == combinationLength)
                    combinations.Add(combination);
            }

            return combinations;
        }

        private static List<DiscoverCureAction> GetDiscoverCureAction(Player player, List<int> reseachStationLocations, CureState[] cures)
        {
            var list = new List<DiscoverCureAction>();

            if (!reseachStationLocations.Contains(player.Location)) return list;

            var cardsByColor = new[] { new List<int>(), new List<int>(), new List<int>(), new List<int>() };
            var cureColor = -1;

            var isScientist = player.Role == Role.Scientist;
            var numCardsRequiredForCure = isScientist ? 4 : 5;  // Scientist Special Ability

            foreach (var i in player.Hand)  // group cards by cardsOfSameColor
            {
                if (i > 47) continue; // event card

                var x = (i / 48.0) * 4;

                cardsByColor[(int)x].Add(i);
            }

            for (var i = 0; i < cardsByColor.Length; i++) // check if has enough cards
            {
                if (cardsByColor[i].Count >= numCardsRequiredForCure)
                {
                    cureColor = i;
                }
            }

            if (cureColor < 0) return list; // not enough cards of single cardsOfSameColor

            if (cures[cureColor] > 0) return list; // cure already found

            var combinations = GetCombinations(cardsByColor[cureColor], numCardsRequiredForCure);

            foreach (var combination in combinations)
            {
                list.Add(new DiscoverCureAction(combination, cureColor, player));
            }

            return list;
        }

        private static List<ShareKnowledgeAction> GetShareKnowledgeActions(Player player, List<Player> players)
        {
            var list = new List<ShareKnowledgeAction>();

            Player givingPlayer = null;
            var receivingPlayers = new List<Player>();

            foreach (var p in players)
            {
                if (p.Location != player.Location) continue;

                if (p.Hand.Contains(player.Location))
                {
                    givingPlayer = p;
                }
                else
                {
                    receivingPlayers.Add(p);
                }
            }

            if (givingPlayer == null) return list; //No player at current location with that location card

            foreach (var p in receivingPlayers)
            {
                list.Add(new ShareKnowledgeAction(givingPlayer, p, player.Location)); //Todo add logic for special ability
            }

            return list;
        }

        private static List<TreatDiseaseAction> GetTreatDiseaseAction(Map map, int playerLocation)
        {
            var list = new List<TreatDiseaseAction>();

            if (!map.IsCityInfected(playerLocation)) return list;

            for (var i = 0; i < map.Cities[playerLocation].Cubes.Length; i++)
            {
                if (map.Cities[playerLocation].Cubes[i] > 0)
                    list.Add(new TreatDiseaseAction(i));
            }
            return list;
        }

        #endregion Private Methods

    }
}