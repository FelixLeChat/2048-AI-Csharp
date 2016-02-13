# Hill-climbing Search
#
# Author: Michel Gagnon
#         michel.gagnon@polytml.ca

from node import *
from state import *
import time

def beam_search(initialPopulation,maxSteps = 10, populationSize = 5):
    step = maxSteps
    population = [Node(s) for s in initialPopulation]

    best_candidate = None
    best_cost = sys.maxint

    startTime = int(round(time.time() * 1000))

    while step > 0:
        print step
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

    best_candidate = newNodes[0]

    currentTime = int(round(time.time() * 1000))-startTime
    print 'Cost:', best_candidate.h
    print 'Time(ms) :', currentTime

    return best_candidate


