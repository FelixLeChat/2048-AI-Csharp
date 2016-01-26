# A* Search
#
# Author: Michel Gagnon
#         michel.gagnon@polytml.ca

from node import *
from state import *

def astar_search(initialState):
    step = 0
    frontier = [Node(initialState)]
    while frontier:
        node = frontier.pop(0)
        step += 1
        node.state.show()
        print 'Action:', node.action
        print '----------------'
        if node.state.isGoal():
            node.state.show()
            print 'Cost:', node.g
            print 'Steps:', step
            return node
        elif node.isRepeated():
            continue
        else:
            frontier = frontier + node.expand()
            frontier.sort(cmp = lambda n1,n2: -1 if n1.f < n2.f else (1 if n1.f > n2.f else 0))
    return None
