import cProfile
import StateSearch
from AntennaVisualisation import *
import re
import pstats

def startSearch():

    # 16 points, Cost:1532, step:2785, Time: 372,063
    #search([(30,0),(10,10),(20,20),(30,40),(50,40),(60,10),(70,80),(80,80),(50,50),(40,10),(60,50),(40,80),(50,70),(30,60),(40,60),(40,40)],200,1)

    # 14 points, Cost:1532, Step:2920, Time: 80,100 sec
    #search([(30,0),(10,10),(20,20),(30,40),(50,40),(60,10),(70,80),(80,80),(50,50),(40,10),(60,50),(40,80),(50,70),(30,60)],200,1)

    # 13 points, Cost:1520, Step:5514, Time: 61,939 sec
    #search([(30,0),(10,10),(20,20),(30,40),(50,40),(60,10),(70,80),(80,80),(50,50),(40,10),(60,50),(40,80),(50,70)],200,1)

    # 12 points, Cost:1471, Step:1809, Time: 24,103 sec State generate: 52953
    positions = [(30,0),(10,10),(20,20),(30,40),(50,40),(60,10),(70,80),(80,80),(50,50),(40,10),(60,50),(40,80)]


    # 11 points, Cost:1270, Step:547, Time: 5,339 sec
    #search([(30,0),(10,10),(20,20),(30,40),(50,40),(60,10),(70,80),(80,80),(50,50),(40,10),(60,50)],200,1)

    # min heap : Cost : 1270, Steps : 547, Time : 6569
    # normal:    Cost : 1270, Steps : 547, Time : 9371
    #positions = [(30,0),(10,10),(20,20),(30,40),(50,40),(60,10),(70,80),(80,80),(50,50),(40,10),(60,50)]

    # 5 points, Cost:700, Step:7, Time: 2 ms
    #positions = [(30,0),(10,10),(20,20),(30,40),(50,40)]

    search(positions, 200, 1)


def search(positions, k, c):
    solution = StateSearch.search(positions, k, c)
    draw_plot(positions, solution)
    print(solution)


def run(debug=False):
    if debug:
        cProfile.run('startSearch()', 'result')
        p = pstats.Stats('result')
        p.sort_stats('time').print_stats(10)
    else:
        startSearch()

run(True)

