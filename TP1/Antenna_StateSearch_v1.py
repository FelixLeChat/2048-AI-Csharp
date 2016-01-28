# Antenna problem with state search


from node import *
from state import *
from astar_search import *
import math
import matplotlib.pyplot as plt
import cProfile



import numpy as np
import copy

class AntennaSearch(State):

    def __init__(self, houses):
        self.housesLeft = houses
        self.antennas = []
        self.totalCost = 0

    def equals(self,state):
        if set(self.housesLeft) == set(state.housesLeft) and self.totalCost== state.totalCost:
            return True
        return False

    def executeAction(self,(x, y, squaredRayon, housesInRange)):
        self.antennas.append((x, y, squaredRayon))
        self.housesLeft = [x for x in self.housesLeft if x not in housesInRange]

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
                    squaredRayon = getSquaredRayon(nearest, antennaPos)
                    actions.append((antennaPos[0], antennaPos[1], squaredRayon, nearest))
                # If nearest is empty -> return himself
                else:
                    housePos = housesMap[houseId]
                    actions.append((housePos[0], housePos[1], 1, [houseId]))

        return actions


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

        for k in range(1, houseLeft):
            currentMinimalCost = sys.maxint

            for houseId in self.housesLeft:

                nearest = self.getKNearest(houseId, k)
                nearest.append(houseId)
                antennaPos = middlePoint(nearest)
                rayon = getSquaredRayon(nearest, antennaPos)
                if(rayon < currentMinimalCost):
                    currentMinimalCost = rayon
            costList.append(currentMinimalCost)

        return costList

    ''' Use in the heuristic cost, the shortestDistanceList represent the lowest radius to connect
        xi elements (0, 1, ...) and the repartitionOfAntenna (y1, y2,...) says there is y1 antenna of radius x1,... '''
    def calculateCostByRepartition(self, shortestRadiusList, repartitionOfAntenna):
        totalCost = 0
        for index in xrange(len(shortestRadiusList)):
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

    drawPlot(Positions, solution)
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
    currentMiddlePoint = (sumX/size, sumY/size)

    ### Fine tuning here ###
    #return currentMiddlePoint
    return improveMiddlePoint(pointsId, currentMiddlePoint, getSquaredRayon(pointsId, currentMiddlePoint))

def improveMiddlePoint(pointsId, currentMiddle, radiusToImprove):
    possibleDirection = [(1,0), (-1,0), (0, 1), (0, -1)]
    currentBest = currentMiddle
    for direction in possibleDirection:
        newX = currentMiddle[0] + direction[0]
        newY = currentMiddle[1] + direction[1]
        newPossibleMiddle = (newX, newY)
        radius = getSquaredRayon(pointsId, newPossibleMiddle)
        if(radius < radiusToImprove):
            ## Option 1 : more search (much slower but not better)
            #currentBest = improveMiddlePoint(pointsId, newPossibleMiddle,  radius)
            #radiusToImprove = getSquaredRayon(pointsId, currentBest)
            ## Option 2 : quick search
            return improveMiddlePoint(pointsId, newPossibleMiddle,  radius)
    return currentBest



def getSquaredRayon(housesId, antenna):
    maxRayonSquared = 1
    for house in housesId:
        housePos = housesMap[house]
        squaredRayon = SquaredDistance(housePos, antenna)
        if(squaredRayon > maxRayonSquared):
            maxRayonSquared = squaredRayon
    return maxRayonSquared
    #intRadius = pow(math.ceil(math.sqrt(maxRayonSquared)), 2)
    #return intRadius


def drawPlot(Positions, Antennas):

    x = []
    y = []
    colors = []
    area = []
    for position in Positions:
        x.append(position[0])
        y.append(position[1])
        colors.append(1)
        area.append(10)

    fig = plt.gcf()
    for antenna in Antennas.state.antennas:
        x.append(position[0])
        y.append(position[1])
        colors.append(1)
        area.append(10)

        circle1=plt.Circle((antenna[0],antenna[1]),math.sqrt(antenna[2]),color='r', alpha=0.2)
        fig.gca().add_artist(circle1)

    plt.scatter(x, y, s=area, c=colors, alpha=1)
    ax = plt.gca()
    ax.set_aspect(1)
    ax.grid()

    plt.show()
    return


def startSearch():


    # 14 points, Cost:1641, Step:7242, Time: 741,143 sec
    #search([(30,0),(10,10),(20,20),(30,40),(50,40),(60,10),(70,80),(80,80),(50,50),(40,10),(60,50),(40,80),(50,70),(30,60)],200,1)

    # 13 points, Cost:1615, Step:8142, Time: 536,099 sec
    #search([(30,0),(10,10),(20,20),(30,40),(50,40),(60,10),(70,80),(80,80),(50,50),(40,10),(60,50),(40,80),(50,70)],200,1)

    # 12 points, Cost:1591, Step:3540, Time: 105,837 sec
    search([(30,0),(10,10),(20,20),(30,40),(50,40),(60,10),(70,80),(80,80),(50,50),(40,10),(60,50),(40,80)],200,1)

    # 11 points, Cost:1365, Step:696, Time: 4,782 sec
    # search([(30,0),(10,10),(20,20),(30,40),(50,40),(60,10),(70,80),(80,80),(50,50),(40,10),(60,50)],200,1)

    # 5 points, Cost:700, Step:7, Time: 2 ms
    #search([(30,0),(10,10),(20,20),(30,40),(50,40)],200,1)


cProfile.run('startSearch()')#.sortStat('tottime')

# Profiling
#cProfile.run('search([(30,0),(10,10),(20,20),(30,40),(50,40),(60,10),(70,80),(80,80),(50,50),(40,10),(60,50),(40,80)],200,1)').sortStat('tottime')
# Total Time : 132,950 sec

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

