# Depth-first Search
#
# Author: Michel Gagnon
#         michel.gagnon@polytml.ca

from node import *
import time
from CostFollower import *

cumulative_steps = 0
cumulative_time = 0
cumulative_cost = 0


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
            currentTime = int(round(time.time() * 1000))-startTime
            print 'Time(ms) :', currentTime

            cost_follower = get_cost_follower()
            cost_follower.update(step, currentTime, node.g)
            cost_follower.show()

            return node
        elif node.isRepeated():
            continue
        else:
            frontier = node.expand() + frontier
    return None
