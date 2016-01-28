# Depth-first Search
#
# Author: Michel Gagnon
#         michel.gagnon@polytml.ca

from node import *
from state import *
import time

def depthfirst_search(initialState):
    frontier = [Node(initialState)]
    startTime = int(round(time.time() * 1000))
    step = 0

    while frontier:
        node = frontier.pop(0)
        step += 1
        # node.state.show()
        # print '----------------'
        if node.state.isGoal():
            node.state.show()
            print 'Cost:', node.g
            print 'Steps:', step
            print 'Time(ms) :', int(round(time.time() * 1000))-startTime
            return node
        elif node.isRepeated():
            continue
        else:
            frontier = node.expand() + frontier
    return None
