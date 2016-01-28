# Lowest-cost Search
#
# Author: Michel Gagnon
#         michel.gagnon@polytml.ca

from node import *
from state import *
import time

def lowestcost_search(initialState):
    step = 0
    frontier = [Node(initialState)]
    visited = set()
    startTime = int(round(time.time() * 1000))

    while frontier:
        node = frontier.pop(0)
        visited.add(node.state)
        step += 1
        # node.state.show()
        # print '----------------'
        if node.state.isGoal():
            node.state.show()
            print 'Cost:', node.g
            print 'Steps:', step
            print 'Time(ms) :', int(round(time.time() * 1000))-startTime
            return node
        else:
            frontier = frontier + [child for child in node.expand() if child.state not in visited]
            frontier.sort(cmp = lambda n1,n2: -1 if n1.g < n2.g else (1 if n1.g > n2.g else 0))
    return None
