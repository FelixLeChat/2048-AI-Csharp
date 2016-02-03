import copy
from lineProfiler import *
import pickle

combination_to_load = 20

instance = None
filename_20 = "precalculated_combination_20"
filename_30 = "precalculated_combination_30"
is_30_load = False


def get_combination_repartition(count):
    global instance, is_30_load

    if not instance:
        if count < 20:
            instance = CombinationRepartition(filename_20)
        else:
            instance = CombinationRepartition(filename_30)
            is_30_load = True

    elif not is_30_load and count >= 20:
        instance = CombinationRepartition(filename_30)
        is_30_load = True

    return instance


class CombinationRepartition:

    def __init__(self, file_name):
        self.CombinationRepartition = {}
        self.try_load(file_name)

    def try_load(self, file_name):
        try:
            with open(file_name,'rb') as f:
                 self.CombinationRepartition = pickle.load(f)
        except Exception:
            self.init_repartition_combination(combination_to_load, file_name)


    ''' Will create all the combination where the sum of the groups will give a number of house '''
    @do_profile()
    def init_repartition_combination(self, count, file_name):

        for k in range(0, count):
            print("Doing {0}".format(k))
            firstList = [0] * count
            firstList[k] = 1
            self.CombinationRepartition[k] = []
            self.CombinationRepartition[k].append(firstList)

            for r in range(k-1, -1, -1):
                diff = k - r
                for previousList in self.CombinationRepartition[r]:
                    copyList = copy.deepcopy(previousList)
                    copyList[diff-1] += 1
                    self.CombinationRepartition[k].append(copyList)
                    b_set = set(map(tuple, self.CombinationRepartition[k]))
                    self.CombinationRepartition[k] = map(list, b_set)

        #Save
        with open(file_name, 'wb') as f:
            pickle.dump(self.CombinationRepartition, f)

    def get_combination(self, houses_count_left):
        maxIndex = len(self.CombinationRepartition)
        if houses_count_left <= maxIndex:
            return self.CombinationRepartition[houses_count_left-1]
        else:
            print("Issue with heuristic, will use random not admissible heuristic")
            falseCombinaison = [0] * houses_count_left
            for index in xrange(maxIndex):
                falseCombinaison[index] = self.CombinationRepartition[maxIndex-1][0][index]
            return [falseCombinaison]
