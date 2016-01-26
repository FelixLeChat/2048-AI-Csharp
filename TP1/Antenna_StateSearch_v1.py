# Antenna problem with state search


from node import *
from state import *
from astar_search import *
import math


import numpy as np
import copy

class AntennaSearch(State):

    def __init__(self, houses):
        self.housesLeft = houses
        self.antennas = []
        self.totalCost = 0

    def equals(self,state):
        return False

    def executeAction(self,(x, y, rayon, housesInRange)):
        self.antennas.append((x, y, rayon))
        self.housesLeft = [x for x in self.housesLeft if x not in housesInRange]
        helle = 0

# TODO
    def possibleActions(self):
        actions = []
        houseLeft = len(self.housesLeft)

        if(houseLeft == 1):
            test = 1

        for k in range(1, houseLeft + 1):
            for houseId in self.housesLeft:
                nearest = self.getKNearest(houseId, k)
                nearest.append(houseId)
                if nearest:
                    antennaPos = middlePoint(nearest)
                    rayon = self.getRayon(nearest, antennaPos)
                    actions.append((antennaPos[0], antennaPos[1], rayon, nearest))
                # If nearest is empty -> return himself
                else:
                    housePos = housesMap[houseId]
                    actions.append((housePos[0], housePos[1], 1, [houseId]))

        return actions

    def getRayon(self, housesId, antenna):
        maxRayonSquared = 1
        for house in housesId:
            housePos = housesMap[house]
            squaredRayon = SquaredDistance(housePos, antenna)
            if(squaredRayon > maxRayonSquared):
                maxRayonSquared = squaredRayon
        return math.sqrt(maxRayonSquared)

    # TODO : upgrade
    ''' Return a list of id '''
    def getKNearest(self, houseId, k):
        kNearest = []
        for key, distance in Nearest[houseId]:
            if key in self.housesLeft:
                k -= 1
                kNearest.append(key)
                if(k == 0):
                    break
        return kNearest

    def cost(self,(x, y, rayon, housesInRange)):
        global K, C
        cost = K + C*pow(rayon,2)
        return cost

    def isGoal(self):
        return len(self.housesLeft) == 0

    def show(self):
        print("Houses left: {0}".format(self.housesLeft))
        print("Antennas : {0}".format(self.antennas))


# TODO
    def heuristic(self):
        return 0



housesMap = {}

K = 0
C = 0
Nearest = {}

def search(Positions, k, c):
    init(Positions, k, c)
    solution = astar_search(AntennaSearch(range(0, len(Positions))))
    print(solution)


def init(Position, k, c):
    global K, C
    K = k
    C = c

    # Initiate index-position map
    for index in xrange(len(Position)):
        housesMap[index] = Position[index]

    # Initiate nearest structure
    for i in xrange(len(Position)-1):
        for j in range(i + 1, len(Position)):
            distance = SquaredDistance(Position[i], Position[j])

            if i not in Nearest:
                Nearest[i] = []
            if j not in Nearest:
                Nearest[j] = []

            Nearest[i].append((j, distance))
            Nearest[j].append((i, distance))

    for key, value in Nearest.iteritems():
        value.sort(key=lambda tup: tup[1])



def SquaredDistance(a, b):
    squaredRayon = pow(a[0]-b[0], 2) + pow(a[1]-b[1],2)
    return squaredRayon

''' Sorry its ugly '''
def middlePoint(pointsId):
    sumX = 0
    sumY = 0
    size = len(pointsId)
    for id in pointsId:
        sumX += housesMap[id][0]
        sumY += housesMap[id][1]
    return (sumX/size, sumY/size)


search([(30,0),(10,10),(20,20),(30,40),(50,40), (10,20), (20,30), (0,0)],200,1)