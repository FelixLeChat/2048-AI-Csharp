# -*- coding: utf-8 -*-

# Class State
#
# Author: Michel Gagnon
#         École Polytechnique de Montréal
# Date:   January 12, 2015
#
# To solve a problem, you must define you sub-class that redefines its methods

class State:
    # State is changed according to action
    def executeActions(self,action):
        pass

    # Checks whether current state and the one passed as parameter are exactly the same
    def equals(self,state):
        return False

    # Checks whether the state is a goal state
    def isGoal(self):
        return False

    # Prints to the console a description of the state
    def show(self):
        pass

    # State is updated according to action
    def executeAction(self,action):
        pass

    # Returns a list of possible actions with the current state
    def possibleActions(self):
        return []

    # Returns the cost of executing some action
    # By default, we suppose that all actions have the same cost = 1
    def cost(self,action):
        return 1

    # Returns a heuristic value that provides an estimate of the remaining
    # cost to achieve the goal
    # By default, value is 0
    def heuristic(self):
        return 0

 
