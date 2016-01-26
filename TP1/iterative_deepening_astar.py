# IDA* Search
#
# Author: Michel Gagnon
#         michel.gagnon@polytml.ca

from node import *
from state import *

def iterative_deepening_astar_search(initialState):
    limit = 0
    while True:
        (minCost,solution) = rec_df_search(Node(initialState),limit)
        if solution != None:
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
        
