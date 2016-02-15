# A* Search
#
# Author: Michel Gagnon
#         michel.gagnon@polytml.ca

# Modified version: with priority queue

from node import *
import time
from PriorityQueue import *
from CostFollower import *


def astar_search_minheap(initialState):
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
            print '######## Solution State ############'
            node.state.show()
            print '----- Result for state-----'
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
                frontier.push(newNode.f, newNode)
    return None
