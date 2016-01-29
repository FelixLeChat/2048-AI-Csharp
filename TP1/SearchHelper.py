"""
Helper for most common operation in the Antenna Search
    Caching, pre-calculation
    Utility method
"""


import copy
import math
import sys


class SearchHelper:

    def __init__(self, houses, k, c, fine_tuning_mod=True, radius_in_int=True, use_cached_squared=False):
        self.MiddleFineTuning = fine_tuning_mod
        self.RadiusInInt = radius_in_int
        self.HousesMap = {}
        self.K = k
        self.C = c
        self.Nearest = {}
        self.CombinationRepartition = {}

        self.UsedCachedSquared = use_cached_squared
        self.CachedSquared = {}

        self.init(houses)

    ''' Initialise the SearchHelper '''
    def init(self, positions):
        self.init_house_mapping(positions)
        self.init_nearest(positions)
        self.init_repartition_combination(len(positions))

    ''' Initiate index-position map '''
    def init_house_mapping(self, positions):
        for index in xrange(len(positions)):
            self.HousesMap[index] = positions[index]

    ''' For each houses will calculate the distance with each other houses '''
    def init_nearest(self, positions):
        # Initiate nearest structure
        for i in xrange(len(positions)-1):
            for j in range(i + 1, len(positions)):
                distance = self.calculate_squared_distance(positions[i], positions[j])

                if i not in self.Nearest:
                    self.Nearest[i] = []
                if j not in self.Nearest:
                    self.Nearest[j] = []

                self.Nearest[i].append((j, distance))
                self.Nearest[j].append((i, distance))

        for key, value in self.Nearest.iteritems():
            value.sort(key=lambda tup: tup[1])

    ''' Will create all the combination where the sum of the groups will give a number of house '''
    def init_repartition_combination(self, house_count):

        for k in range(0, house_count):
            firstList = [0] * house_count
            firstList[k] = 1
            self.CombinationRepartition[k] = []
            self.CombinationRepartition[k].append(firstList)

            for r in range(k-1, -1, -1):
                diff = k - r
                for previousList in self.CombinationRepartition[r]:
                    copyList = copy.deepcopy(previousList)
                    copyList[diff-1] += 1
                    self.CombinationRepartition[k].append(copyList)
                    b_set = set(map(tuple, self.CombinationRepartition[k]))
                    self.CombinationRepartition[k] = map(list, b_set)

    ''' Calculate the squared distance between two points '''
    def calculate_squared_distance(self, a, b):
        dx = a[0] - b[0]
        dy = a[1] - b[1]

        if self.UsedCachedSquared:
            squared_rayon = self.get_cached_squared(dx) + self.get_cached_squared(dy)
        else:
            # if ImproveDistanceCalculation: 1.5 -> 1.3
            squared_rayon = dx*dx + dy*dy
            # squared_rayon = pow((a[0] - b[0]), 2) + pow(a[1] - b[1], 2)

        return squared_rayon

    def get_cached_squared(self, value):
        value = abs(value)
        if value not in self.CachedSquared:
            self.CachedSquared[value] = value*value
        return self.CachedSquared[value]

    def calculate_cost(self, squared_radius):
        cost = self.K + self.C * squared_radius
        return cost

    def get_house_from_id(self, house_id):
        return self.HousesMap[house_id]

    def get_combination(self, houses_count_left):
        return self.CombinationRepartition[houses_count_left-1]

    ''' Givent a list of houses id, will return the middle of that will give the smallest radius to cover them '''
    def find_middle_point(self, houses_id):
        sum_x = 0
        sum_y = 0
        size = len(houses_id)
        for current_id in houses_id:
            sum_x += self.HousesMap[current_id][0]
            sum_y += self.HousesMap[current_id][1]
        current_middle_point = (sum_x/size, sum_y/size)

        if self.MiddleFineTuning:
            return self.improve_middle_point(houses_id,
                                             current_middle_point,
                                             self.calculate_squared_radius_to_fit(houses_id, current_middle_point))
        else:
            return current_middle_point

    ''' Will fine tuning the middle point by moving around until
        it find the position that will give the smallest radius '''
    def improve_middle_point(self, points_id, current_middle, radius_to_improve):
        possible_direction = [(1, 0), (-1, 0), (0, 1), (0, -1)]
        current_best = current_middle
        for direction in possible_direction:
            new_x = current_middle[0] + direction[0]
            new_y = current_middle[1] + direction[1]
            new_possible_middle = (new_x, new_y)
            radius = self.calculate_squared_radius_to_fit(points_id, new_possible_middle)
            if radius < radius_to_improve:
                return self.improve_middle_point(points_id, new_possible_middle, radius)
        return current_best

    """ Get the radius of the antenna, so it can reach every houses in the list """
    def calculate_squared_radius_to_fit(self, houses_id, antenna):
        max_rayon_squared = 1
        for house in houses_id:
            house_pos = self.HousesMap[house]
            squared_rayon = self.calculate_squared_distance(house_pos, antenna)
            if squared_rayon > max_rayon_squared:
                max_rayon_squared = squared_rayon

        if self.RadiusInInt:
            ceil_value = math.ceil(math.sqrt(max_rayon_squared))
            int_radius = ceil_value * ceil_value
            return int_radius
        else:
            return max_rayon_squared

    ''' Return a list of the shortest Radius to link (1 element, 2 elements,...) '''
    def get_shortest_nearest_radius_list(self, houses_left):
        solo_cost = self.K + self.C

        cost_list = [solo_cost]

        house_left_count = len(houses_left)

        for k in range(1, house_left_count):
            current_minimal_cost = sys.maxint

            for houseId in houses_left:
                nearest = self.get_k_nearest(houseId, k, houses_left)
                nearest.append(houseId)
                antenna_position = self.find_middle_point(nearest)
                rayon = self.calculate_squared_radius_to_fit(nearest, antenna_position)
                if rayon < current_minimal_cost:
                    current_minimal_cost = rayon
            cost_list.append(current_minimal_cost)

        return cost_list

    ''' Use in the heuristic cost, the shortestDistanceList represent the lowest radius to connect
        xi elements (0, 1, ...) and the repartitionOfAntenna (y1, y2,...) says there is y1 antenna of radius x1,... '''
    def calculate_cost_by_repartition(self, shortest_radius_list, repartition_of_antenna):
        totalCost = 0
        for index in xrange(len(shortest_radius_list)):
            totalCost += repartition_of_antenna[index] * self.calculate_cost(shortest_radius_list[index])
        return totalCost

    """ Return a list of the k nearest houses id of the given house id """
    def get_k_nearest(self, house_id, k, houses_left):
        k_nearest = []
        for key, distance in self.Nearest[house_id]:
            if key in houses_left:
                k -= 1
                k_nearest.append(key)
                if(k == 0):
                    break
        return k_nearest

    """ Return the squared distance of the closes house not cover """
    def get_closest_distance(self, house_id, houses_left):
        for key, distance in self.Nearest[house_id]:
            if key in houses_left:
                return distance


