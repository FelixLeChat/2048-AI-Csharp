import numpy as np
from sklearn.cluster import MeanShift, estimate_bandwidth, AffinityPropagation
from sklearn.datasets.samples_generator import make_blobs
import matplotlib.pyplot as plt
from itertools import cycle

# Issue for library http://stackoverflow.com/questions/28190534/windows-scipy-install-no-lapack-blas-resources-found
# response of drewid


def find_cluster(initial_positions, current_quantile, use_kmean):

    #centers = [[1, 1], [-1, -1], [1, -1]]
    #X, _ = make_blobs(n_samples=10000, centers=centers, cluster_std=0.6)

    size = len(initial_positions)

    sample = np.reshape(initial_positions, [size, 2])

    try:
        #Mean Shift
        if use_kmean:
            bandwidth = estimate_bandwidth(sample, quantile=current_quantile, n_samples=size)
            ms = MeanShift(bandwidth=bandwidth, bin_seeding=True)
            ms.fit(initial_positions)
            cluster_centers = ms.cluster_centers_

        else:
            # Affinity
            ms = AffinityPropagation().fit(sample)
            cluster_centers = ms.cluster_centers_indices_


        labels = ms.labels_

        labels_unique = np.unique(labels)
        n_clusters_ = len(labels_unique)

        '''
        if use_kmean:
            plot_mean(n_clusters_, labels, cluster_centers, sample)
        else:
            plot_affinity(n_clusters_, labels, cluster_centers, sample)
        '''


        separated_cluster = [[] for i in range(n_clusters_)]

        for index in xrange(size):
            separated_cluster[labels[index]].append(initial_positions[index])

        return separated_cluster

    except Exception:
            return [initial_positions]


def plot_mean(n_clusters_, labels, cluster_centers, sample):

    print("number of estimated clusters : %d" % n_clusters_)

    plt.figure(1)
    plt.clf()

    colors = cycle('bgrcmykbgrcmykbgrcmykbgrcmyk')
    for k, col in zip(range(n_clusters_), colors):
        my_members = labels == k
        cluster_center = cluster_centers[k]
        plt.plot(sample[my_members, 0], sample[my_members, 1], col + '.')
        plt.plot(cluster_center[0], cluster_center[1], 'o', markerfacecolor=col,
                 markeredgecolor='k', markersize=14)
    plt.title('Estimated number of clusters: %d' % n_clusters_)
    plt.show()

def plot_affinity(n_clusters_, labels, cluster_centers, sample):
    plt.close('all')
    plt.figure(1)
    plt.clf()

    colors = cycle('bgrcmykbgrcmykbgrcmykbgrcmyk')
    for k, col in zip(range(n_clusters_), colors):
        class_members = labels == k
        cluster_center =sample[cluster_centers[k]]
        plt.plot(sample[class_members, 0], sample[class_members, 1], col + '.')
        plt.plot(cluster_center[0], cluster_center[1], 'o', markerfacecolor=col,
                 markeredgecolor='k', markersize=14)
        for x in sample[class_members]:
            plt.plot([cluster_center[0], x[0]], [cluster_center[1], x[1]], col)

    plt.title('Estimated number of clusters: %d' % n_clusters_)
    plt.show()



