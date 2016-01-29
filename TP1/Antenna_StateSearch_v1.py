# Antenna problem with state search


from node import *
from state import *

import numpy as np
import copy
import SearchHelper

state_generate = 0

class AntennaSearch(State):

    def __init__(self, houses_id, search_helper):
        self.HousesLeft = houses_id
        self.Antennas = []
        self.TotalCost = 0
        AntennaSearch.SearchHelper = search_helper

    """ We consider equals if the the cost and the houses left are the same """
    def equals(self, state):
        if set(self.HousesLeft) == set(state.HousesLeft) and self.TotalCost == state.TotalCost:
            return True
        return False

    """ Apply the action to himself """
    def executeAction(self, (x, y, squared_rayon, houses_in_range)):
        self.Antennas.append((x, y, squared_rayon))
        self.HousesLeft = [x for x in self.HousesLeft if x not in houses_in_range]

    """ The possibles actions are the positioning of an antenna that cover each x(1 to houses_left_count) nearest
        neighbor of each points """
    def possibleActions(self):
        actions = []
        houses_left_count = len(self.HousesLeft)

        visited_antenna = set()

        for k in range(1, houses_left_count + 1):
            for houseId in self.HousesLeft:
                nearest = AntennaSearch.SearchHelper.get_k_nearest(houseId, k, self.HousesLeft)
                nearest.append(houseId)
                if nearest:
                    antenna_pos = AntennaSearch.SearchHelper.find_middle_point(nearest)
                    squared_rayon = AntennaSearch.SearchHelper.calculate_squared_radius_to_fit(nearest, antenna_pos)

                    # Improvement to check if not already visited: 52953 -> 4810 state generated
                    antenna = (antenna_pos[0], antenna_pos[1], squared_rayon)
                    if antenna not in visited_antenna:
                        visited_antenna.add(antenna)
                        actions.append((antenna_pos[0], antenna_pos[1], squared_rayon, nearest))
                # If nearest is empty -> return himself
                else:
                    house_pos = self.SearchHelper.get_house_from_id(houseId)
                    actions.append((house_pos[0], house_pos[1], 1, [houseId]))
        global state_generate
        state_generate += len(actions)
        return actions

    def cost(self, (x, y, squared_rayon, houses_in_range)):
        return AntennaSearch.SearchHelper.calculate_cost(squared_rayon)

    def isGoal(self):
        return len(self.HousesLeft) == 0

    def show(self):
        global state_generate
        state_generate += 1
        print("State generate: {0}".format(state_generate))
        print("Houses left: {0}".format(self.HousesLeft))
        print("Antennas : {0}".format(self.Antennas))

    def heuristic(self):
        houses_left_count = len(self.HousesLeft)

        if houses_left_count == 0:
            return 0
        if houses_left_count == 1:
            return AntennaSearch.SearchHelper.calculate_cost(1)
        else:
            return self.awesome_heuristic()

    def awesome_heuristic(self):
        houses_left_count = len(self.HousesLeft)
        shortest_radius_list = AntennaSearch.SearchHelper.get_shortest_nearest_radius_list(self.HousesLeft)

        minimal_cost = sys.maxint
        for repartition in AntennaSearch.SearchHelper.get_combination(houses_left_count):
            cost = AntennaSearch.SearchHelper.calculate_cost_by_repartition(shortest_radius_list, repartition)
            if cost < minimal_cost:
                minimal_cost = cost

        return minimal_cost















