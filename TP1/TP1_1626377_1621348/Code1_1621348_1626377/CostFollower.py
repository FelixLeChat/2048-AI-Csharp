instance = None


def get_cost_follower():
    global instance

    if not instance:
        instance = CostFollower()

    return instance


class CostFollower:

    def __init__(self):
        self.Steps = 0
        self.Time = 0
        self.Cost = 0

    def update(self, steps, time, cost):
        self.Steps += steps
        self.Time += time
        self.Cost += cost

    def show(self):
        print '------- Cumulative Result --------'
        print 'Cumulative Cost:', self.Cost
        print 'Cumulative Steps:',  self.Steps
        print 'Cumulative Time:', self.Time