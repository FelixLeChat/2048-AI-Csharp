# A* Search
#
# Author: Michel Gagnon
#         michel.gagnon@polytml.ca

# Modified version: with priority queue

from node import *
import time
from PriorityQueue import *
from CostFollower import *

power_value = 1.0

def set_power_value(value):
    global power_value
    power_value = value


def astar_search_squared_heuristic(initialState):
    global power_value
    step = 0
    frontier = PriorityQueue()
    frontier.push(0, Node(initialState))
    startTime = int(round(time.time() * 1000))
    while frontier:
        node = frontier.pop()
        step += 1
        #node.state.show()
        #print 'Action:', node.action
        #print "Cost", node.g
        #print "Heuristic", node.h
        #print step
        #print '----------------'
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
            for newNode in node.expand():
                frontier.push(pow(newNode.h, power_value) + newNode.g, newNode)
    return None
