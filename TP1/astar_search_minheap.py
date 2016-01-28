# A* Search
#
# Author: Michel Gagnon
#         michel.gagnon@polytml.ca

# Modified version: with priority queue

from node import *
from state import *
import datetime
import time
from PriorityQueue import *

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
            node.state.show()
            print 'Cost:', node.g
            print 'Steps:', step
            print 'Time(ms) :', int(round(time.time() * 1000))-startTime
            return node
        elif node.isRepeated():
            continue
        else:
            for newNode in node.expand():
                frontier.push(newNode.f, newNode)
    return None
