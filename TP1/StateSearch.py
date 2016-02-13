from astar_search import *
from depthfirst_search import *
from iterative_deepening_astar import *
from lowestcost_search import *
from astar_search_minheap import *
from bestfirst_search import *
from Antenna_StateSearch_v1 import AntennaSearch
from astar_search_squared_heuristic import *
from SearchHelper import *
import gc
from Antenna_StateSearch_Original import AntennaSearch_original
#from Clustering import *

# Config

fine_tuning_mod = True
radius_in_int = False
use_cluser = False
#If false will use affinity
use_kmean = True

# Bad optimisation (stay to False)
# False : 6.618 -> True : 11.848
use_cached_squared = False

use_depth_first_search = False



solution = None
cluster_limit = 12

normal_a_start_limit = 14
power_heuristic_level_1 = 25
power_level_1 = 1.1
power_heuristic_level_2 = 30
power_level_2 = 1.2
power_heuristic_level_3 = 50
power_level_3 = 1.3

quantile_init_value = 0.6
min_quantile = 0.001



def state_search(positions, k, c):

    sol = search_partial_solution(positions, None, k, c, quantile_init_value)
    return sol.state.Antennas


def search_partial_solution(positions, current_solution, k, c, current_quantile):
    current_quantile = max(min_quantile, current_quantile)

    for partial_positions in get_clustered_positions(use_cluser, positions, current_quantile):

        size = len(partial_positions)

        # We don't need to cluster anymore
        if size < cluster_limit or current_quantile <= min_quantile or not use_cluser:
            print("********************************")
            print("Attacking next {0} houses".format(size))
            search_helper = SearchHelper(partial_positions, k, c, fine_tuning_mod, radius_in_int, use_cached_squared)

            initial_state = get_state_strategy(partial_positions, search_helper)
            search_strategy = get_search_strategy(partial_positions)

            partial_solution = search_strategy(initial_state)

            del initial_state
            del search_helper
            del search_strategy
        else:
            partial_solution = search_partial_solution(partial_positions, current_solution, k, c, current_quantile/2)

        if current_solution and partial_solution:
            current_solution.combine(partial_solution)
        elif partial_solution:
            current_solution = partial_solution

        del partial_positions
        gc.collect()
    return current_solution


def get_clustered_positions(use_cluser, positions, current_quantile):
    #if use_cluser and len(positions) >= cluster_limit:
         #return find_cluster(positions, current_quantile, use_kmean)
    #else:
        return [positions]


def get_state_strategy(positions, search_helper):
    initial_state = AntennaSearch(range(0, len(positions)), search_helper)
    initial_state = AntennaSearch_original(range(0, len(positions)), search_helper)
    return initial_state


def get_search_strategy(positions):

    # 12 points, Cost:1471, Step:1809, Time: 23,860 sec State generate: 52953
    # With Set(actions) : Cost:1471, Step:311, Time: 4780 sec State generate: 4810
    size = len(positions)

    if use_depth_first_search:
        chosen_strategy = depthfirst_search
    else:
        if size <= normal_a_start_limit:
            chosen_strategy = astar_search_minheap

        elif size <= power_heuristic_level_1:
            set_power_value(power_level_1)
            chosen_strategy = astar_search_squared_heuristic

        elif size <= power_heuristic_level_2:
            set_power_value(power_level_2)
            chosen_strategy = astar_search_squared_heuristic

        elif size <= power_heuristic_level_3:
            set_power_value(power_level_3)
            chosen_strategy = astar_search_squared_heuristic

        else:
            chosen_strategy = bestfirst_search

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