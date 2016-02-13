
from node import *
from state import *
import time
from operator import xor
from random import *
import hashlib
from Antenna import *
from AntennaVisualisation import *
import copy

from simulated_annealing import *

# The state are [[House ids couver by the same antenna]...]
class AntennaLocalSearch(State):
    def __init__(self, houses_id, search_helper, show_randomization):
        self.HousesGroup = {}
        self.TotalCost = 0
        AntennaLocalSearch.SearchHelper = search_helper
        self.ShowRandomization = show_randomization

        self.randomize(houses_id)

    def randomize(self, houses_id):

        if(len(houses_id) == 1):
            whole = HousesGroup(houses_id)
            self.HousesGroup[whole.Id] = whole

        # changes to get chosen depending the distance to the selected point
        houses_left = houses_id
        k = self.SearchHelper.K
        c = self.SearchHelper.C

        while len(houses_left) != 0:

            houses_count = len(houses_left)

            if(houses_count == 1):
                to_add = HousesGroup(houses_left)
                self.HousesGroup[to_add.Id] = to_add
                break

            # Random house to start to seperate groups
            rand = random.randint(0, len(houses_left)-1)
            random_house = houses_left[rand]
            nearest = self.SearchHelper.Nearest[random_house]

            # remove to far
            to_remove = []
            # more point there is, the farest he goes to group them together
            heuristic_count = 3*k +(houses_count/20)*k

            for near in nearest:
                if(near[1] * c * c > heuristic_count):
                    to_remove.append(near)
            for remove in to_remove:
                nearest.remove(remove)


            # Max distances
            max_cost = 0
            for house in nearest:
                if(house[1] > max_cost):
                    max_cost = house[1]

            # relative distance cost
            relative_cost = []
            for house in nearest:
                cost = float(house[1])/float(max_cost)
                #min 50% of being selected
                if(cost > 0.50):
                    cost = 0.50
                relative_cost.append((house[0],int((1.0-cost)*100)))

            # Generate a random number
            chance = random.randint(0, 100)

            # check against cost
            group_to_add = []
            group_to_add.append(random_house)
            houses_left.remove(random_house)

            for house in relative_cost:
                index = house[0]
                if(house[1] >= chance):
                    if(index in houses_left):
                        group_to_add.append(index)
                        houses_left.remove(index)

            if(len(group_to_add) > 0):
                to_add = HousesGroup(group_to_add)
                self.HousesGroup[to_add.Id] = to_add

        if self.ShowRandomization:
            self.show_current_state()

        return

    def show_current_state(self):
        draw_plot(self.SearchHelper.HousesPosition, self.get_antennas())


    # Todo: Improve this crap
    def equals(self,state):
        return self.TotalCost == state.TotalCost and len(state.HousesGroup) == len(self.HousesGroup)

    def show(self):
        print self.HousesGroup

    def executeAction(self, action):
        for id in action.Delete:
            del self.HousesGroup[id]

        for house_group in action.ReplaceBy:
            self.HousesGroup[house_group.Id] = house_group

    def possibleActions(self):

        #self.show_current_state()

        actions = []


        split_actions = self.get_splits_actions()
        if split_actions:
            actions.extend(split_actions)



        fusion_actions = self.get_fuse_action()
        if fusion_actions:
            actions.extend(fusion_actions)

        return actions


    def get_splits_actions(self):
       """
       Simple split: only remove farest house
       :param self:
       :return:
       """
       actions = []

       for _, group in self.HousesGroup.iteritems():

            size = len(group.Houses)

            if size > 1:
                action = Action()
                action.add_delete(group.Id)

                farest_house = self.SearchHelper.get_farest_point_for_antenna(group.Antenna.Position,
                                                                              group.Antenna.Radius,
                                                                              group.Houses)
                copied_list = list(group.Houses)
                copied_list.remove(farest_house)
                group_1 = HousesGroup(copied_list)
                group_2 = HousesGroup([farest_house])

                action.add_replace(group_1)
                action.add_replace(group_2)

                actions.append(action)

       return actions




    def get_fuse_action(self):
        """
        Fusion with nearest house group (will perform poorly with many group (need to change)
        :param self:
        :return:
        """

        if len(self.HousesGroup) <= 1:
            return None

        actions = []
        visited_group = set()

        for _, group in self.HousesGroup.iteritems():
            if group in visited_group:
                continue

            visited_group.add(group)

            nearest_group = None
            nearest_distance = sys.maxint

            for _, group_to_compare in self.HousesGroup.iteritems():
                if group_to_compare in visited_group or group.Id == group_to_compare.Id:
                    continue

                distance = self.SearchHelper.calculate_squared_distance(group.Antenna.Position, group_to_compare.Antenna.Position)
                if distance < nearest_distance:
                    nearest_group = group_to_compare
                    nearest_distance = group_to_compare

            if nearest_group:
                visited_group.add(nearest_group)

                action = Action()
                action.add_delete(group.Id)
                action.add_delete(nearest_group.Id)

                fusionList = list(group.Houses)
                fusionList.extend(nearest_group.Houses)

                action.add_replace(HousesGroup(fusionList))

                actions.append(action)

        return actions



    def cost(self,action):
        return 1

    def isGoal(self):
        return False

    def heuristic(self):
        return self.calculate_cost()

    def calculate_cost(self):
        antennas = self.get_antennas()
        total_Cost = 0
        for antenna in antennas:
            total_Cost += AntennaLocalSearch.SearchHelper.calculate_cost(antenna.Radius)
        return total_Cost


    def get_antennas(self):
        antennas = []
        for key, value in self.HousesGroup.iteritems():
            antennas.append(value.Antenna)
        return antennas


    def consistent(self):
        return True


"""   Representation of the group of house, which is use for representation in the local search"""

class HousesGroup():
    def __init__(self, houses_id):
        self.Houses = houses_id
        self.Id = self.get_id()

        position, radius = AntennaLocalSearch.SearchHelper.find_middle_point(houses_id)
        self.Antenna = Antenna(position, radius)

    def get_id(self):
        """
        s = str(sorted(self.Houses))
        id = hashlib.sha1(s).hexdigest()
        return id
        """
        return random.randint(0, sys.maxint)


class Action():
    def __init__(self):
        # List of houseGroup hash
        self.Delete = []
        # List of HouseGroup to replace by
        self.ReplaceBy = []


    def add_delete(self, id):
        self.Delete.append(id)

    def add_replace(self, house_group):
        self.ReplaceBy.append(house_group)






