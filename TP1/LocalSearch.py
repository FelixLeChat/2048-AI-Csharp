
from simulated_annealing import *
from SearchHelper import *
from AntennaLocalSearch import *
from hillclimbing import *

fine_tuning_mod = True
radius_in_int = True
use_cluser = True
#If false will use affinity
use_kmean = True


def local_search(positions, k, c):

    sol = search(positions, k, c)
    return sol

def search(positions, k, c):

    search_helper = SearchHelper(positions, k, c, fine_tuning_mod, radius_in_int)
    initial_state = get_local_representation(positions, search_helper)
    search_strategy = get_search_strategy(positions)

    solution = search_strategy(initial_state)

    return solution.state.get_antennas()


def get_search_strategy(positions):

    #chosen_strategy = simulated_annealing_search
    chosen_strategy = hillclimbing_search

    return chosen_strategy


def get_local_representation(positions, search_helper):
    initial_state = AntennaLocalSearch(range(0, len(positions)), search_helper)
    return initial_state