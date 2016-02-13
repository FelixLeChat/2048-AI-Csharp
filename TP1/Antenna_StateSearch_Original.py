# Antenna problem with state search


from node import *
from state import *

import numpy as np
import copy
import SearchHelper
from Antenna import *

state_generate = 0

class AntennaSearch_original(State):

    def __init__(self, houses_id, search_helper):
        self.HousesLeft = houses_id
        self.Antennas = []
        self.TotalCost = 0
        AntennaSearch_original.SearchHelper = search_helper

    """ We consider equals if the the cost and the houses left are the same """
    def equals(self, state):
        if set(self.HousesLeft) == set(state.HousesLeft) and self.TotalCost == state.TotalCost:
            return True
        return False

    """ Apply the action to himself """
    def executeAction(self, (antenna, houses_in_range)):
        self.Antennas.append(antenna)
        self.HousesLeft = [x for x in self.HousesLeft if x not in houses_in_range]

    """ The possibles actions are the positioning of an antenna that cover each x(1 to houses_left_count) nearest
        neighbor of each points """
    def possibleActions(self):
        actions = []
        houses_left_count = len(self.HousesLeft)

        visited_antenna = set()

        for k in range(1, houses_left_count + 1):
            for houseId in self.HousesLeft:
                nearest = self.SearchHelper.get_k_nearest(houseId, k, self.HousesLeft)
                nearest.append(houseId)
                if nearest:
                    antenna_pos, radius = self.SearchHelper.find_middle_point(nearest)
                    squared_rayon = self.SearchHelper.calculate_squared_radius_to_fit(nearest, antenna_pos)

                    # Improvement to check if not already visited: 52953 -> 4810 state generated
                    antenna = (antenna_pos[0], antenna_pos[1], squared_rayon)
                    if antenna not in visited_antenna:
                        visited_antenna.add(antenna)
                        actions.append((Antenna(antenna_pos, radius), nearest))
                # If nearest is empty -> return himself
                else:
                    house_pos = self.SearchHelper.get_house_from_id(houseId)
                    actions.append((Antenna(house_pos, 1), [houseId]))
        global state_generate
        state_generate += len(actions)
        return actions

    def cost(self, (antenna, houses_in_range)):
        return self.SearchHelper.calculate_cost(antenna.Radius)

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
            return self.SearchHelper.calculate_cost(1)
        else:
            return self.awesome_heuristic()

    def awesome_heuristic(self):
        houses_left_count = len(self.HousesLeft)
        shortest_radius_list, _ = self.SearchHelper.get_shortest_nearest_radius_list(self.HousesLeft)

        minimal_cost = self.calculate_best_estimate(self.calculate_price_by_house(shortest_radius_list), houses_left_count)

        return minimal_cost

    def calculate_best_estimate(self, price_by_house_ration, house_left_count):
        total_cost = 0
        while house_left_count > 0:
            cost, house_left_count = self.do_calculate_best_ratio(price_by_house_ration, house_left_count)
            total_cost += cost
        return cost

    def calculate_price_by_house(self, shortest_radius_list):
        price_by_house_ratio = [0] * len(shortest_radius_list)
        for index in xrange(len(shortest_radius_list)):
            cost = self.SearchHelper.calculate_cost(shortest_radius_list[index])
            price_by_house_ratio[index] = cost / (index+1)
        return price_by_house_ratio


    def do_calculate_best_ratio(self, price_by_house_ration, house_left):
        if house_left == 0:
            return 0

        best_cost = min(price_by_house_ration[0: house_left])
        best_index = list.index(price_by_house_ration, best_cost)
        factor = math.floor(house_left / (best_index+1))
        house_left_to_cover = int( house_left - (best_index+1)*factor)
        total_cost = factor*best_cost*(best_index+1)

        return total_cost, house_left_to_cover















