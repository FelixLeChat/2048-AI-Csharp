import cProfile
from StateSearch import *
from LocalSearch import *
from AntennaVisualisation import *
import pstats
from lineProfiler import *
from random import randint



def startSearch():

    # 12 points, Cost:1471, Step:1809, Time: 24,103 sec State generate: 52953
    # Pas solution optimale: vrai solution
    """
  -  {'middle': (45, 45), 'points': set([3713024257993580481, 3713030753124095031, 3713061063873891181, 3713030753134920281]), 'radius': 16.0}
-{'middle': (40, 80), 'points': set([3713037248191823131]), 'radius': 1}
-{'middle': (75, 80), 'points': set([3712993947061920131, 3713017762821929981]), 'radius': 5.0}
-{'middle': (60, 10), 'points': set([3713024257932959081]), 'radius': 1}
-{'middle': (25, 10), 'points': set([3713061063899871781, 3713067559082347531, 3713037248289250381, 3713074054245337831]), 'radius': 15.0}
    :return:
    """
    positions = [(30,0),(10,10),(20,20),(30,40),(50,40),(60,10),(70,80),(80,80),(50,50),(40,10),(60,50),(40,80)]

    #positions = [(30,0),(10,10),(20,20),(30,40),(50,40),(60,10),(70,80),(80,80),(50,50),(40,10),(60,50),(40,80),(50,70),(30,60),(40,60),(40,40)]

    #positions = [(30,0),(10,10),(20,20),(30,40),(50,40)]

    #positions = [(246, 111), (288, 172), (159, 220), (176, 17), (111, 54), (288, 252), (290, 111), (133, 12), (271, 143), (168, 270), (6, 29), (296, 264), (124, 160), (298, 8), (24, 0), (293, 224), (236, 29), (86, 125), (268, 20), (180, 90), (219, 240), (167, 106), (222, 158), (56, 148), (198, 22), (110, 121), (196, 117), (116, 27), (257, 220), (160, 115), (128, 154), (253, 167), (211, 127), (54, 186), (225, 26), (152, 31), (211, 204), (115, 89), (187, 280), (224, 18), (280, 195), (23, 168), (62, 48), (230, 47), (175, 261), (281, 65), (184, 224), (89, 221), (294, 140), (295, 71)]

    #positions = [(10,30),(10,50),(0,50)]

    #positions = generate_position(100, 200, 200)

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
    #solution = state_search(positions, k, c)
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
    p.sort_stats('time').print_stats(10)


class RunType():
    c_profile = 1
    line_profile = 2
    normal = 3

enumToFunc = {RunType.c_profile: run_cprofiler, RunType.line_profile: run_line_profiler, RunType.normal: startSearch}

#run(RunType.c_profile)
startSearch()


