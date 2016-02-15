# Plot the solution of the antenna problem
#

import math
import matplotlib.pyplot as plt


def draw_plot(positions, antennas):

    x = []
    y = []
    colors = []
    area = []
    for position in positions:
        x.append(position[0])
        y.append(position[1])
        colors.append(1)
        area.append(10)

    fig = plt.gcf()
    for antenna in antennas:
        circle1 = plt.Circle((antenna.Position[0], antenna.Position[1]), math.sqrt(antenna.Radius), color='r', alpha=0.2)
        fig.gca().add_artist(circle1)

    plt.scatter(x, y, s=area, c=colors, alpha=1)
    ax = plt.gca()
    ax.set_aspect(1)
    ax.grid()

    plt.show()
    return