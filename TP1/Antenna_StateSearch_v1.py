# Antenna problem with state search


from node import *
from state import *

import numpy as np
import copy
import SearchHelper
from Antenna import *

state_generate = 0

class AntennaSearch(State):

    def __init__(self, houses_id, search_helper):
        self.HousesLeft = houses_id
        self.Antennas = []
        self.TotalCost = 0
        AntennaSearch.SearchHelper = search_helper

    def combine(self, state):
        self.Antennas = self.Antennas + state.Antennas
        self.TotalCost += state.TotalCost
        self.HousesLeft = self.HousesLeft + state.HousesLeft

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

        #for k in range(0, houses_left_count + 1):
        for k in range(houses_left_count, -1 , -1):
            for houseId in self.HousesLeft:
                nearest = AntennaSearch.SearchHelper.get_k_nearest(houseId, k, self.HousesLeft)
                nearest.append(houseId)
                if nearest:

                    antenna_pos, radius = AntennaSearch.SearchHelper.find_middle_point(nearest)

                    # Improvement to check if not already visited: 52953 -> 4810 state generated
                    antenna = (antenna_pos[0], antenna_pos[1])
                    if antenna not in visited_antenna:
                        visited_antenna.add(antenna)
                        actions.append((Antenna(antenna_pos, radius), nearest))
                # If nearest is empty -> return himself
                #else:
                #    house_pos = self.SearchHelper.get_house_from_id(houseId)
                #    actions.append((house_pos[0], house_pos[1], 1, [houseId]))
        global state_generate
        state_generate += len(actions)
        return actions

    def get_action_for_target_house(self, house_id):
        houses_left_count = len(self.HousesLeft)
        actions = []
        pro_rata = [AntennaSearch.SearchHelper.calculate_cost(1)]

        for k in range(0, houses_left_count + 1):
            nearest = AntennaSearch.SearchHelper.get_k_nearest(house_id, k, self.HousesLeft)
            farest = nearest[-1]
            minimal_radius = AntennaSearch.SearchHelper.calculate_distance_between_house(house_id, farest)

            #Minimum radius is 1
            distance_with_farest = max(1, minimal_radius)
            minimal_possible_cost = AntennaSearch.SearchHelper.calculate_cost(minimal_radius)
            current_pro_rata = minimal_possible_cost / (k+1)
            current_minimal_pro_rata = min(pro_rata)

            # Only add the antenna as a possible one if the pro rata cost is lower than the previous one
            if current_pro_rata < current_minimal_pro_rata:
                nearest.append(house_id)
                antenna_pos, radius = AntennaSearch.SearchHelper.find_middle_point(nearest)
                actions.append((Antenna(antenna_pos, radius), nearest))
            else:
                k = math.ceil(minimal_possible_cost/current_minimal_pro_rata)

                # check how many houses will be neessary for it to be acceptable!


    def cost(self, (antenna, houses_in_range)):
        return AntennaSearch.SearchHelper.calculate_cost(antenna.Radius)

    def isGoal(self):
        return len(self.HousesLeft) == 0

    def show(self):
        global state_generate
        state_generate += 1
        #print("State generate: {0}".format(state_generate))
        #print("Houses left: {0}".format(self.HousesLeft))
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
        shortest_radius_list, _ = AntennaSearch.SearchHelper.get_shortest_nearest_radius_list(self.HousesLeft)

        ''' Not use anymore
        minimal_cost = sys.maxint
        for repartition in AntennaSearch.SearchHelper.get_combination(houses_left_count):
            cost = AntennaSearch.SearchHelper.calculate_cost_by_repartition(shortest_radius_list, repartition)
            if cost < minimal_cost:
                minimal_cost = cost
                best_repartition = repartition
        '''

        minimal_cost = self.calculate_best_estimate(self.calculate_price_by_house(shortest_radius_list), houses_left_count)

        return minimal_cost

    def awesome_heuristic_2(self):
        shortest_radius_list, associate_house_list = AntennaSearch.SearchHelper.get_shortest_nearest_radius_list(self.HousesLeft)


    def calculate_price_by_house(self, shortest_radius_list):
        price_by_house_ratio = [0] * len(shortest_radius_list)
        for index in xrange(len(shortest_radius_list)):
            cost = AntennaSearch.SearchHelper.calculate_cost(shortest_radius_list[index])
            price_by_house_ratio[index] = cost / (index+1)
        return price_by_house_ratio

    def calculate_best_estimate(self, price_by_house_ration, house_left):
        if house_left == 0:
            return 0

        best_cost = min(price_by_house_ration[0: house_left])
        best_index = list.index(price_by_house_ration, best_cost)
        factor = math.floor(house_left / (best_index+1))
        house_left_to_cover = int( house_left - (best_index+1)*factor)
        total_cost = factor*best_cost*(best_index+1)

        return total_cost + self.calculate_best_estimate(price_by_house_ration, house_left_to_cover)


















