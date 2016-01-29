from astar_search import *
from depthfirst_search import *
from iterative_deepening_astar import *
from lowestcost_search import *
from astar_search_minheap import *
from bestfirst_search import *
from Antenna_StateSearch_v1 import AntennaSearch
from SearchHelper import *


def search(positions, k, c):
    # Config

    fine_tuning_mod = True
    radius_in_int = True

    # Bad optimisation (stay to False)
    # False : 6.618 -> True : 11.848
    use_cached_squared = False

    search_helper = SearchHelper(positions, k, c, fine_tuning_mod, radius_in_int, use_cached_squared)

    initial_state = get_state_strategy(positions, search_helper)
    search_strategy = get_search_strategy()

    solution = search_strategy(initial_state)

    return solution


def get_state_strategy(positions, search_helper):
    initial_state = AntennaSearch(range(0, len(positions)), search_helper)
    return initial_state


def get_search_strategy():

    # 12 points, Cost:1471, Step:1809, Time: 23,860 sec State generate: 52953
    # With Set(actions) : Cost:1471, Step:311, Time: 4780 sec State generate: 4810
    chosen_strategy = astar_search_minheap

    # 12 points, Cost:2075, Step:7, Time: 559 sec State generate: 365
    #chosen_strategy = depthfirst_search

    # 12 points, Cost:2722, Step:2, Time: 351 sec State generate: 145
    #chosen_strategy = bestfirst_search

    return chosen_strategy


#### Profiling result ####

# Antenna
# 132,590 : Search
# 3,185   : Awesome heuristic
# 3,730   : Draw plot
# 3,268   : Heuristic
# 2,689   : Get Shortest nearest Radius left
# 1,466   : Get Squared Rayon
# 1,156   : Possible Actions
# 1,147   : Get Nearest

# A Star Search
# 128,732 : A Star Search

# Copy
# 9,723 : Deep Copy
# 9,442 : Deep Copy inst
# 8,603 : Deep Copy Dict
# 6,911 : Deep Copy List
# 4,320 : Deep Copy Tuple

# Node
# 14,924 : Expand Node
# 13,691 : Create Node

# Autre
# 13,787 : map
# 111,804 : sort of list object