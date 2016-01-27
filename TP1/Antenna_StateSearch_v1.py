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

    def executeAction(self,(x, y, squaredRayon, housesInRange)):
        self.antennas.append((x, y, squaredRayon))
        self.housesLeft = [x for x in self.housesLeft if x not in housesInRange]

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
                    squaredRayon = self.getSquaredRayon(nearest, antennaPos)
                    actions.append((antennaPos[0], antennaPos[1], squaredRayon, nearest))
                # If nearest is empty -> return himself
                else:
                    housePos = housesMap[houseId]
                    actions.append((housePos[0], housePos[1], 1, [houseId]))

        return actions

    def getSquaredRayon(self, housesId, antenna):
        maxRayonSquared = 1
        for house in housesId:
            housePos = housesMap[house]
            squaredRayon = SquaredDistance(housePos, antenna)
            if(squaredRayon > maxRayonSquared):
                maxRayonSquared = squaredRayon
        return maxRayonSquared

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

    def getClosestDistance(self, houseId):
        kNearest = []
        for key, distance in Nearest[houseId]:
            if key in self.housesLeft:
                return distance

    def cost(self,(x, y, squaredRayon, housesInRange)):
        return self.calculateCost(squaredRayon)

    def isGoal(self):
        return len(self.housesLeft) == 0

    def show(self):
        print("Houses left: {0}".format(self.housesLeft))
        print("Antennas : {0}".format(self.antennas))


    def heuristic(self):
        global K, C
        soloCost = K + C

        houseLeft = len(self.housesLeft)

        if(houseLeft == 0):
            return 0
        if(houseLeft == 1):
            return soloCost
        else:
            return self.awesomeHeuristic()

    def awesomeHeuristic(self):
        global K, C

        housesLeft = len(self.housesLeft)
        shortestRadiusList = self.getShortestNearestRadiusList()

        minCost = sys.maxint
        for repartition in combinaisonRepartition[housesLeft-1]:
            cost = self.calculateCostByRepartition(shortestRadiusList, repartition)
            if(cost < minCost):
                minCost = cost

        return minCost


    ''' Return a list of the shortest Radius to link (1 element, 2 elements,...) '''
    def getShortestNearestRadiusList(self):
        global K, C
        soloCost = K + C

        costList = [soloCost]

        houseLeft = len(self.housesLeft)

        for k in range(1, houseLeft + 1):
            currentMinimalCost = 0
            for houseId in self.houseLeft:

                nearest = self.getKNearest(houseId, k)
                nearest.append(houseId)
                antennaPos = middlePoint(nearest)
                rayon = self.getRayon(nearest, antennaPos)
                if(rayon < currentMinimalCost):
                    currentMinimalCost = rayon
            costList.append(currentMinimalCost)

        return costList

    ''' Use in the heuristic cost, the shortestDistanceList represent the lowest radius to connect
        xi elements (0, 1, ...) and the repartitionOfAntenna (y1, y2,...) says there is y1 antenna of radius x1,... '''
    def calculateCostByRepartition(self, shortestRadiusList, repartitionOfAntenna):
        totalCost = 0
        for index in xrange(repartitionOfAntenna):
            totalCost += repartitionOfAntenna[index] * self.calculateCost(shortestRadiusList[index])
        return totalCost

    def calculateCost(self, squaredRadius):
        global K, C
        cost = K + C*squaredRadius
        return cost




housesMap = {}

K = 0
C = 0
Nearest = {}

combinaisonRepartition = {}

def search(Positions, k, c):
    init(Positions, k, c)
    initialState = AntennaSearch(range(0, len(Positions)))
    solution = astar_search(initialState)
    print(solution)

def init(Positions, k, c):
    initNearest(Positions, k, c)
    initRepartitionCombinaison(len(Positions))

def initNearest(Position, k, c):
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

def initRepartitionCombinaison(nbOfHouse):

    for k in range(0, nbOfHouse):
        firstList = [0] * nbOfHouse
        firstList[k] = 1
        combinaisonRepartition[k] = []
        combinaisonRepartition[k].append(firstList)

        for r in range(k-1, -1, -1):
            diff = k - r
            for previousList in combinaisonRepartition[r]:
                copyList = copy.deepcopy(previousList)
                copyList[diff-1] += 1
                combinaisonRepartition[k].append(copyList)
                b_set = set(map(tuple, combinaisonRepartition[k]))
                combinaisonRepartition[k] = map(list, b_set)




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
#search([(30,0),(10,10),(20,20),(30,40),(50,40)],200,1)