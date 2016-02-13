# Hill-climbing Search
#
# Author: Michel Gagnon
#         michel.gagnon@polytml.ca

from node import *
from state import *
from random import *
from numpy import *
import time


def decrease(t):
    return t*0.9

def simulated_annealing_search(initialState,T = 100,limit = 0.1,maxSteps = 200):
    temperature = T
    best_candidate = None
    best_cost = sys.maxint

    startTime = int(round(time.time() * 1000))

    while temperature > limit:
        node = Node(initialState)
        print temperature
        step = maxSteps
        is_new_node = True
        candidates = []
        while step > 0:
            if node.state.isGoal():
                node.state.show()
                return node
            else:
                if is_new_node:
                    candidates = node.expand()
                    is_new_node = False
                candidate = choice(candidates)
                if candidate.h < node.h:
                    node = candidate
                    is_new_node = True
                elif random.random() < exp(float(node.h - candidate.h)/temperature):
                    node = candidate
                    is_new_node = True

                if node.h < best_cost:
                    best_cost =  node.h
                    best_candidate = node
                step -= 1
        temperature = decrease(temperature)

    currentTime = int(round(time.time() * 1000))-startTime
    print 'Cost:', best_candidate.h
    print 'Time(ms) :', currentTime

    return best_candidate
