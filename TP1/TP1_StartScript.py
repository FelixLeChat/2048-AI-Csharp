import cProfile
import StateSearch
from AntennaVisualisation import *
import pstats
from lineProfiler import *
from enum import Enum



def startSearch():

    # 12 points, Cost:1471, Step:1809, Time: 24,103 sec State generate: 52953
    # Pas solution optimale: vrai solution
    """
    {'middle': (45, 45), 'points': set([3713024257993580481, 3713030753124095031, 3713061063873891181, 3713030753134920281]), 'radius': 16.0}
{'middle': (40, 80), 'points': set([3713037248191823131]), 'radius': 1}
{'middle': (75, 80), 'points': set([3712993947061920131, 3713017762821929981]), 'radius': 5.0}
{'middle': (60, 10), 'points': set([3713024257932959081]), 'radius': 1}
{'middle': (25, 10), 'points': set([3713061063899871781, 3713067559082347531, 3713037248289250381, 3713074054245337831]), 'radius': 15.0}
    :return:
    """

    positions = [(30,0),(10,10),(20,20),(30,40),(50,40),(60,10),(70,80),(80,80),(50,50),(40,10),(60,50),(40,80)]

    #positions = [(30,0),(10,10),(20,20),(30,40),(50,40),(60,10),(70,80),(80,80),(50,50),(40,10),(60,50),(40,80),(50,70),(30,60),(40,60),(40,40)]

    #positions = [(30,0),(10,10),(20,20),(30,40),(50,40)]

    search(positions, 200, 1)


def search(positions, k, c):
    solution = StateSearch.search(positions, k, c)
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
    p.sort_stats('time').print_stats(10)


class RunType(Enum):
    c_profile = 1
    line_profile = 2
    normal = 3

enumToFunc = {RunType.c_profile: run_cprofiler, RunType.line_profile: run_line_profiler, RunType.normal: startSearch}

run(RunType.line_profile)


""" Old benchmark

     # 16 points, Cost:1532, step:2785, Time: 372,063
    #search([(30,0),(10,10),(20,20),(30,40),(50,40),(60,10),(70,80),(80,80),(50,50),(40,10),(60,50),(40,80),(50,70),(30,60),(40,60),(40,40)],200,1)

    # 14 points, Cost:1532, Step:2920, Time: 80,100 sec
    #search([(30,0),(10,10),(20,20),(30,40),(50,40),(60,10),(70,80),(80,80),(50,50),(40,10),(60,50),(40,80),(50,70),(30,60)],200,1)

    # 13 points, Cost:1520, Step:5514, Time: 61,939 sec
    #search([(30,0),(10,10),(20,20),(30,40),(50,40),(60,10),(70,80),(80,80),(50,50),(40,10),(60,50),(40,80),(50,70)],200,1)

     # 11 points, Cost:1270, Step:547, Time: 5,339 sec
    #search([(30,0),(10,10),(20,20),(30,40),(50,40),(60,10),(70,80),(80,80),(50,50),(40,10),(60,50)],200,1)

    # min heap : Cost : 1270, Steps : 547, Time : 6569
    # normal:    Cost : 1270, Steps : 547, Time : 9371
    #positions = [(30,0),(10,10),(20,20),(30,40),(50,40),(60,10),(70,80),(80,80),(50,50),(40,10),(60,50)]

    # 5 points, Cost:700, Step:7, Time: 2 ms
    #positions = [(30,0),(10,10),(20,20),(30,40),(50,40)]
"""