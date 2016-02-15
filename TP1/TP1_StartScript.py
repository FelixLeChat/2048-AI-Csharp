import cProfile
from StateSearch import *
from LocalSearch import *
from AntennaVisualisation import *
import pstats
from lineProfiler import *
from random import randint

class Strategy():
    state = 1
    local = 2

######################################### ICI ##############################################
''' Change here for strategy between local and state search '''
choosen_strategy = Strategy.local

def startSearch():

    #positions = [(30,0),(10,10),(20,20),(30,40),(50,40),(60,10),(70,80),(80,80),(50,50),(40,10),(60,50),(40,80)]


    ######################################### ICI ##############################################
    ''' ICI pour mettre position '''
    positions = [(30,0),(10,10),(20,20),(30,40),(50,40)]

    #positions = generate_position(500, 300, 300)

    print(positions)

    k = 200
    c = 1

    search(positions, k , c)



def generate_position(count, x_max, y_max):
    positions = []
    for x in xrange(count):
        positions.append((randint(0, x_max), randint(0, y_max)))
    return positions


def search(positions, k, c):
    if choosen_strategy == Strategy.state:
        solution = state_search(positions, k, c)
    else:
        solution = local_search(positions, k, c)

    draw_plot(positions, solution)
    print(solution)


def run(mode):
    func = enumToFunc[mode]
    func()

# Exemple de comment utiliser le
#@do_profile(follow=[startSearch])
def run_line_profiler():
    startSearch()

def run_cprofiler():
    cProfile.run('startSearch()', 'result')
    p = pstats.Stats('result')
    p.sort_stats('time').print_stats(20)


class RunType():
    c_profile = 1
    line_profile = 2
    normal = 3

enumToFunc = {RunType.c_profile: run_cprofiler, RunType.line_profile: run_line_profiler, RunType.normal: startSearch}

run(RunType.c_profile)
#startSearch()


