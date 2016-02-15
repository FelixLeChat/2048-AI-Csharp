from SearchHelper import *
from AntennaLocalSearch import *
from beam_search import *

fine_tuning_mod = True
radius_in_int = True
use_cluser = True
#If false will use affinity
use_kmean = True

show_randomization = False

class LocalStrategy():
    beam_search = 3

strategy_to_use = LocalStrategy.beam_search

# Beam Search
nb_of_population = 3
start_factor = 5
beam_maxSteps = 100
no_improvement_limit = 5


def search(positions, k, c):
    sol = do_search(positions, k, c)
    formatted_solution = map(lambda x: (x.Position[0], x.Position[1], x.Radius), sol.state.get_antennas())
    return formatted_solution

def local_search(positions, k, c):
    sol = do_search(positions, k, c)
    return sol.state.get_antennas()

def do_search(positions, k, c):

    search_helper = SearchHelper(positions, k, c, fine_tuning_mod, radius_in_int)
    initial_state = get_local_representation(positions, search_helper, strategy_to_use)
    search_strategy = get_search_strategy(strategy_to_use)

    solution = search_strategy(initial_state)

    return solution


def get_search_strategy(choosen_strategy):

    enumToFunc = {LocalStrategy.beam_search : lambda x: beam_search(x, beam_maxSteps, nb_of_population, no_improvement_limit )}

    return enumToFunc[choosen_strategy]


def get_local_representation(positions, search_helper, strategy_to_use):

    if strategy_to_use == LocalStrategy.beam_search:
        initial_state = map(lambda x: AntennaLocalSearch(range(0, len(positions)), search_helper, show_randomization), range(nb_of_population * start_factor))
        return initial_state
    else:
        initial_state = AntennaLocalSearch(range(0, len(positions)), search_helper, show_randomization)
        return initial_state

print(search([(10,10), (20,20)], 10, 10))


