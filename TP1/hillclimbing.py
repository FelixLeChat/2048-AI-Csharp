# Hill-climbing Search
#
# Author: Michel Gagnon
#         michel.gagnon@polytml.ca

from node import *
from state import *
import time


def hillclimbing_search(initialState,maxSteps = 1000):
    step = maxSteps
    node = Node(initialState)

    best_candidate = None
    best_cost = sys.maxint

    startTime = int(round(time.time() * 1000))

    while step > 0:
        if node.state.isGoal():
            node.state.show()
            print 'Steps:', maxSteps - step + 1
            return node
        else:
            candidates = node.expand()
            candidates.sort(cmp = lambda n1,n2: -1 if n1.h < n2.h else (1 if n1.h > n2.h else 0))
            node = candidates.pop(0)
            step -= 1

            if node.h < best_cost:
                best_candidate = node

    currentTime = int(round(time.time() * 1000))-startTime
    print 'Cost:', best_candidate.h
    print 'Time(ms) :', currentTime

    return best_candidate
