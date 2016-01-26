# -*- coding: utf-8 -*-
#
#  Class Node
# Author: Michel Gagnon
#         École Polytechnique de Montréal
# Date:   January 12, 2015
#
# This class represents a node in a tree search

import sys
import math
import copy

class Node(object):
    def __init__(self, state, action = None, cost = 0, previous = None):
        self.state = state             # state represented by the node
        self.previous = previous       # previous state in the solution path
        self.g = cost                  # cost to reach this node from initial state
        self.h = state.heuristic()     # estimate of remaining cost to achieve goal
        self.f = self.g + self.h       # total estimated cost
        self.action = action           # action that resulted in the state represented by the node

    ####################
    # Public methods
    ####################

    # Returns a lis of all new nodes that represents next possible states in the exploration
    def expand(self):
        return map(lambda s: self._createNode(s), self.state.possibleActions())

    # Check is current node state already exists in path from initial node
    def isRepeated(self):
        return self._findState(self.previous,self.state)

    # Extracts the sequence of states and actions that lead to current node
    def extractSolution(self):
        solution = []
        currentNode = self
        while currentNode.previous:
            solution.append((currentNode.action))
            currentNode = currentNode.previous
        solution.reverse()
        return solution

    ####################
    # Private methods
    ####################

    def _createNode(self,action):
        newState = copy.deepcopy(self.state)
        newState.executeAction(action)
        return Node(newState, action, self.g + self.state.cost(action) , self)
                                    
    def _findState(self,node,state):
        if node == None:
            return False
        else:
            if node.state.equals(state):
                return True
            else:
                return self._findState(node.previous,state)
        
