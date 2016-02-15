# Best-first Search

# Modified version: with priority queue

from node import *
import time
from PriorityQueue import *
from CostFollower import *

def bestfirst_search(initialState):
    step = 0
    frontier = PriorityQueue()
    frontier.push(0, Node(initialState))
    startTime = int(round(time.time() * 1000))
    while frontier:
        node = frontier.pop()
        step += 1
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
                frontier.push(newNode.h, newNode)
    return None
