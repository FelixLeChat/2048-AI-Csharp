
from node import *
from state import *
import time
from operator import xor
from random import *
import hashlib
from Antenna import *

from simulated_annealing import *

# The state are [[House ids couver by the same antenna]...]
class AntennaLocalSearch(State):
    def __init__(self, houses_id, search_helper):
        self.HousesGroup = {}
        self.TotalCost = 0
        AntennaLocalSearch.SearchHelper = search_helper
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

            if(len(houses_left) == 1):
                to_add = HousesGroup(houses_left)
                self.HousesGroup[to_add.Id] = to_add
                return

            # Random house to start to seperate groups
            rand = random.randint(0, len(houses_left)-1)
            random_house = houses_left[rand]
            nearest = self.SearchHelper.Nearest[random_house]

            # remove to far
            to_remove = []
            for near in nearest:
                if(near[1] * c * c > 3 * k):
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
                #min 25% of being selected
                if(cost > 0.65):
                    cost = 0.65
                relative_cost.append((house[0],int((1.0-cost)*100)))

            # Generate a random number
            chance = random.randint(0, 100)

            # check against cost
            group_to_add = []
            group_to_add.append(random_house)
            houses_left.remove(random_house)

            for house in relative_cost:
                if(house[1] >= chance & house[0] in houses_left):
                    group_to_add.append(house[0])
                    houses_left.remove(house[0])

            if(len(group_to_add) > 0):
                to_add = HousesGroup(group_to_add)
                self.HousesGroup[to_add.Id] = to_add

        return



    # Todo: Improve this crap
    def equals(self,state):
        return self.TotalCost == state.TotalCost

    def show(self):
        print self.HousesGroup

    def executeAction(self, action):
        for id in action.Delete:
            del self.HousesGroup[id]

        for house_group in action.ReplaceBy:
            self.HousesGroup[house_group.Id] = house_group

    def possibleActions(self):
        actions = []

        # Random split
        id_to_split = random.choice(self.HousesGroup.values()).Id

        split_actions = self.get_splits_actions(id_to_split)

        if split_actions:
            actions.extend(split_actions)

        fusion_actions = self.get_fuse_action()

        if fusion_actions:
            actions.extend(fusion_actions)

        return actions

    def get_fuse_action(self):

        if len(self.HousesGroup) <= 1:
            return None

        list = (self.HousesGroup.keys())

        id = sample(list, 2)

        action = Action()
        action.add_delete(id[0])
        action.add_delete(id[1])

        fusionList = self.HousesGroup[id[0]].Houses
        fusionList.extend(self.HousesGroup[id[1]].Houses)

        action.add_replace(HousesGroup(fusionList))

        return [action]


    def get_splits_actions(self, id):

        house_groupe_to_split = self.HousesGroup[id].Houses
        size = len(house_groupe_to_split)

        if size > 1:
            action = Action()
            action.add_delete(id)
            random_list = list(self.SearchHelper.random_chunk(house_groupe_to_split, 1, size))

            for inner_list in random_list:
                action.add_replace(HousesGroup(inner_list))

            return [action]
        else:
            return None

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


class HousesGroup():
    def __init__(self, houses_id):
        self.Houses = houses_id
        self.Id = self.get_id()

        position, radius = AntennaLocalSearch.SearchHelper.find_middle_point(houses_id)
        self.Antenna = Antenna(position, radius)

    def get_id(self):
        s = str(sorted(self.Houses))
        id = hashlib.sha1(s).hexdigest()
        return id


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






