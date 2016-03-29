#!/usr/bin/python
#-*- coding: latin-1 -*-

# Représentation du paquet de cartes
#
# Date: 29 février 2012
#
# Auteur: Michel Gagnon



import sys
import getopt
import random
import math

from Mappe import *



class Carte:
    POINT_VICTOIRE = 0
    CHEVALIER = 1
    BIDON = 2


class Cartes(object):
    def __init__(self,debug=False):
        self._paquetCartes = \
             [ Carte.POINT_VICTOIRE for i in range(5) ] + \
             [ Carte.CHEVALIER for i in range(16) ] + \
             [ Carte.BIDON for i in range(6) ]
        if debug:
            self._paquetCartes = [Carte.CHEVALIER,Carte.CHEVALIER,Carte.CHEVALIER,Carte.CHEVALIER,Carte.CHEVALIER,Carte.CHEVALIER,Carte.CHEVALIER,Carte.CHEVALIER,Carte.POINT_VICTOIRE,Carte.BIDON]
        else:
            random.shuffle(self._paquetCartes)

    def vide(self):
        return len(self._paquetCartes) == 0

    def pigerCarte(self):
        if len(self._paquetCartes) > 0:
            return self._paquetCartes.pop()
        else:
            return None

    def afficher(self):
        for c in self._paquetCartes:
            if c == Carte.POINT_VICTOIRE:
                print "P",
            elif c == Carte.CHEVALIER:                
                print "C",
            elif c == Carte.BIDON:
                print "*",
            else:
                raise RuntimeError("Carte invalide")
        print

