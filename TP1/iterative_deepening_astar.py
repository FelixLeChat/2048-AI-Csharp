# IDA* Search
#
# Author: Michel Gagnon
#         michel.gagnon@polytml.ca

from node import *
from state import *
import time

def iterative_deepening_astar_search(initialState):
    limit = 0
    startTime = int(round(time.time() * 1000))
    step = 0
    while True:
        (minCost,solution) = rec_df_search(Node(initialState),limit)
        step += 1
        if solution != None:
            print 'Cost:', solution.g
            print 'Steps:', step
            print 'Time(ms) :', int(round(time.time() * 1000))-startTime
            return solution
        limit = minCost


def rec_df_search(node,limit):
    if node.state.isGoal():
        return (node.f,node)
    elif not node.isRepeated():
        # Node is expanded only if f(n) <= current limit
        if node.f <= limit:
            nextLimit = 999999999999999999
            for n in  node.expand():
                (l,result) = rec_df_search(n,limit)
                if l > limit and l < nextLimit:
                    nextLimit = l
                if result != None:
                    return (l,result)
        else:
            nextLimit = node.f
                
    return (nextLimit,None)
        
