# Hill-climbing Search
#
# Author: Michel Gagnon
#         michel.gagnon@polytml.ca

from node import *
from state import *

def beam_search(initialPopulation,maxSteps = 1000, populationSize = 100):
    step = maxSteps
    population = [Node(s) for s in initialPopulation]
    while step > 0:
        newNodes = []
        for node in population:
            if node.state.isGoal():
                node.state.show()
                print 'Steps:', maxSteps - step + 1
                return node
            else:
                newNodes += node.expand()
        newNodes.sort(cmp = lambda n1,n2: -1 if n1.h < n2.h else (1 if n1.h > n2.h else 0))
        population = newNodes[0:populationSize]
        step -= 1

    return None


