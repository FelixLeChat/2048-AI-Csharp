# Hill-climbing Search
#
# Author: Michel Gagnon
#         michel.gagnon@polytml.ca

from node import *
from state import *


def hillclimbing_search(initialState,maxSteps = 1000):
    step = maxSteps
    node = Node(initialState)
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

    return node
