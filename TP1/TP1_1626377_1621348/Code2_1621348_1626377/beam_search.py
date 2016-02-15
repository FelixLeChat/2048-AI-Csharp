# Hill-climbing Search
#
# Author: Michel Gagnon
#         michel.gagnon@polytml.ca

from node import *
import time

def beam_search(initialPopulation,maxSteps = 100, populationSize = 40, no_improvement_limit = 5):
    step = maxSteps
    population = [Node(s) for s in initialPopulation]

    best_candidate = None
    state_generated = 0

    startTime = int(round(time.time() * 1000))

    no_improvement_count = 0

    while step > 0:
        print step
        newNodes = []
        for node in population:
            if node.state.isGoal():
                node.state.show()
                print 'Steps:', maxSteps - step + 1
                return node
            else:
                generated_node = node.expand()
                newNodes += generated_node
                state_generated += len(generated_node)
        newNodes.sort(cmp = lambda n1,n2: -1 if n1.h < n2.h else (1 if n1.h > n2.h else 0))
        population = newNodes[0:populationSize]
        step -= 1

        current_best = newNodes[0]
        if best_candidate and current_best.h >= best_candidate.h:
            no_improvement_count += 1
            if no_improvement_count >= no_improvement_limit:
                break
        else:
            best_candidate = current_best
            no_improvement_count = 0


    currentTime = int(round(time.time() * 1000))-startTime
    print 'Cost:', best_candidate.h
    print 'Time(ms) :', currentTime
    print 'State generated :', state_generated

    return best_candidate


