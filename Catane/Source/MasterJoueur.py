# coding=utf-8

from Joueur import *
from Mappe import *

class MasterJoueur(Joueur):

    def __init__(self,id):
        super(MasterJoueur,self).__init__(id)

    #choix de l'emplacement de la première colonie et de la première route
    def premierTour(self,mappe):
        return

    #choix de l'emplacement de la deuxième colonie et de la deuxième route
    def deuxiemeTour(self,mappe):
        return

    #c'est cette méthode qui détermine la prochaine action effectuée par le joueur
    def chosirAction(mappe,infoJoueurs,paquetVide):
        return Action.TERMINER

    def joueVoleurs(mappe):
        return

    def volerRessources(quantité):
        return