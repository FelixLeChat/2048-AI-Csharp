#!/usr/bin/python
#-*- coding: latin-1 -*-

# Joueur du jeu de Catane
#
# Date: 29 février 2012
#
# Auteur: Michel Gagnon

# Chaque joueur est implémenté par une classe qui dérive de la classe Joueur
# Chaque classe de joueur spécifique doit être ajoutée dans la fabrique de joueurs et doit 
# redéfinir les quatre méthodes suivantes:
#
#  premierTour():  choix de l'emplacement de la première colonie et de la première route
#  deuxiemeTour(): choix de l'emplacement de la deuxième colonie et de la deuxième route
#
#        Ces deux méthodes retournent une paire (C,R) où C est le numéro de l'intersection 
#        oû doit être placé la colonie et R le numéro de l'intersection correspondant à l'autre
#        extrémité de la route qui part de la colonie
#
# chosirAction(mappe,infoJoueurs,paquetVide): c'est cette méthode qui détermine la prochaine action effectuée
#        par le joueur. On lui passe la mappe et l'information publique sur les joueur (voir plus loin)
#        Elle retourne une paire (A, I) oû A identifie l'action et I
#        est une liste qui fournit les éléments d'information nécessaires pour réaliser
#        l'action. Les possibilités sont les suivantes:
#
#        (Action.ACHETER_CARTE, []) - Pour acheter une carte de développement
#        (Action.ECHANGER_RESSOURCES, [Quantité, RessourceOfferte, RessourceDemandée])
#        (Action.AJOUTER_COLONIE,[Position])
#        (Action.AJOUTER_VILLE,[Position])
#        (Action.AJOUTER_ROUTE,[I1,I2]) - Pour ajouter une route liant les intersections I1 et I2
#        (Action.JOUER_CARTE_CHEVALIER, [V,J] - où V est le numéro du territoire où seront déplacés les voleurs
#                                                  J est le numéro du joueur à qui on volera une carte
#        Action.TERMINER - Pour indiquer que le joueur a terminé son tour
#
#        Le paramètre infoJoueurs est une liste contenant les infos publique sur chaque joueur:
#            infoJoueurs[i] = infos sur joueur i = (pv, cr, ch) 
#                   où pv = nombre de points de victoire (excluant les cartes de points de victoire cachées)
#                      cr = nombre de cartes ressources que le joueur a en sa possession
#                      ch = nombre de cartes chevalier jouées
#
#
# joueVoleurs(mappe): retourne une paire (T,J), où T est le numéro du territore où on doit déplacer les
#        voleur, et J le numéro du joueur à qui on volera une carte
#
#
# volerRessources(quantité): cette fonction doit éliminer de la réserve des ressources la quantité
#        fournie en argument. C'est au joueur d'identifier les ressources qu'il désire éliminer


import sys
import getopt
import random
import math

from Mappe import *
from Action import *
from Cartes import *


class Joueur(object):
    def __init__(self,id):
        random.seed()
        self._id = id

        # Les cartes ressources que le joueur possède dans sa main
        self._ressources = {}
        self._ressources[Ressource.BLE] = 0
        self._ressources[Ressource.ARGILE] = 0
        self._ressources[Ressource.BOIS] = 0
        self._ressources[Ressource.MINERAL] = 0
        self._ressources[Ressource.LAINE] = 0

        # Le total des points de victoire du joueur
        self._pointsVictoire = 2

        # Les cartes chvalier ne peuvent pas être jouéees durant le tour oû elles ont été achetées
        # Une carte achetée dans le tour courant est une carte "reçue". Au tour suivant
        # elle devient une carte "acitvée", c'est-à-dire qu'elle peut être jouée.
        self._cartesChevalierRecues = 0
        self._cartesChevalierActivees = 0
        self._cartesChevalierJouees = 0

        # On mémorise les points de victoires obtenus par le biais de cartes de développement
        self._cartesPointsVictoire = 0

        # La liste des types de ressources que le joueur peut échanger au taux 2:1
        # (ceci dépend des ports spécialisés qu'il possède)
        self._peutEchanger = []

        # Indique si le joueur peut échanger des ressources au taux 3:1
        self._possedePortGenerique = False


    # Méthodes devant être redéfinies par la sous-classe:
    
    def nom(self):
        return "Joueur " + str(self._id)

    def premierTour(self,mappe):
        pass

    def deuxiemeTour(self,mappe):
        pass

    def jouerVoleurs(self,mappe,infoJoueurs):
        return (0,0)  # Retourne une paire non valide

    def choisirAction(self,mappe,infoJoueurs,paquetCartesVide = True):
        return Action.TERMINER


     
    # Méthodes définies seulement dans la classe de base

    # Retourne le numéro d'identification du joueur
    def id(self):
        return self._id

    # Nombre de chevaliers qui ont été joués par le joueur
    def nombreChevaliers(self):
        return self._cartesChevalierJouees

    # Cette méthode est appelée par le contrôleur pour activer les cartes chevalier
    # achetées au tour précédent
    def activerChevaliers(self):
        self._cartesChevalierActivees += self._cartesChevalierRecues
        self._cartesChevalierRecues = 0

    # Cette méthode est appelée par le contrôler pour une carte chevalier de l'état
    # "activée" à l'état "jouée"
    def jouerCarteChevalier(self):
        if self.peutJouerCarteChevalier():
            self._cartesChevalierActivees -= 1
            self._cartesChevalierJouees += 1
        else:
            raise RuntimeError("Tentative de jouer une carte chevalier qui n'existe pas")

    def peutJouerCarteChevalier(self):
        return self._cartesChevalierActivees > 0


    # Cette méthode est appelée lorque le joueur vient de placer une colonie sur un port, 
    # afin de déterminer la capacité d'échange que permet ce port
    def ajusterCapaciteEchange(self,intersection):
        if isinstance(intersection,PortGenerique):
            self._possedePortGenerique = True
        elif isinstance(intersection,PortSpecialise):
            self._peutEchanger.append(intersection.ressource())
            

    def peutEchangerPortSpecialise(self,ressource):
        return (self._ressources[ressource] >= 2 and ressource in self._peutEchanger)

    def peutEchangerPortGenerique(self,ressource):
        return (self._ressources[ressource] >= 3 and self._possedePortGenerique)

    def peutEchangerBanque(self,ressource):
        return (self._ressources[ressource] >= 4)


    def nombreCartesRessources(self):
        total = 0
        for n in self._ressources.values():
            total += n
        return total

    def nombrePointsVictoire(self):
        return self._pointsVictoire

    def nombrePointsVictoireVisibles(self):
        return self._pointsVictoire - self._cartesPointsVictoire

    def augmenterPointsVictoire(self,quantite):
        self._pointsVictoire += quantite

    def diminuerPointsVictoire(self,quantite):
        self._pointsVictoire -= quantite

    # Met à jour l'état du joueur selon la carte de développement qu'il vient d'acheter
    def ajouterCarte(self,carte):
        if carte == Carte.POINT_VICTOIRE:
            self._cartesPointsVictoire += 1
            self._pointsVictoire += 1
        elif carte == Carte.CHEVALIER:
            self._cartesChevalierRecues += 1            
        else:
            pass

    def ajouterRessources(self,ressource,quantite):
        self._ressources[ressource] += quantite

    def retirerRessources(self,ressource,quantite):
        if self._ressources[ressource] < quantite:
            raise RuntimeError("Nombre de ressources insuffisant")
        self._ressources[ressource] -= quantite

    def quantiteRessources(self,ressource):
        return self._ressources[ressource]

    # Choisit aléatoire une des ressource du joueur et la retire de sa pile
    # La fonction retourne le type de la ressource retirée
    def pigerRessourceAleatoirement(self):
        n = self.nombreRessources()
        if n == 0:
            return None

        x = math.floor(random.random()*n)

        if x < self._ressources[Ressource.BLE]:
            ressourceVolee = Ressource.BLE
            self.retirerRessources(ressourceVolee,1)
            return ressourceVolee
        
        x -= self._ressources[Ressource.BLE]
        if x < self._ressources[Ressource.ARGILE]:
            ressourceVolee = Ressource.ARGILE
            self.retirerRessources(ressourceVolee,1)
            return ressourceVolee

        x -= self._ressources[Ressource.ARGILE]

        if x < self._ressources[Ressource.BOIS]:
            ressourceVolee = Ressource.BOIS
            self.retirerRessources(ressourceVolee,1)
            return ressourceVolee
        
        x -= self._ressources[Ressource.BOIS]

        if x < self._ressources[Ressource.MINERAL]:
            ressourceVolee = Ressource.MINERAL
            self.retirerRessources(ressourceVolee,1)
            return ressourceVolee

        x -= self._ressources[Ressource.MINERAL]

        if x < self._ressources[Ressource.LAINE]:
            ressourceVolee = Ressource.LAINE
            self.retirerRessources(ressourceVolee,1)
            return ressourceVolee

        raise RuntimeError('Erreur') 
 
    def nombreRessources(self):
        n = 0
        for k in self._ressources:
            n += self._ressources[k]
        return n
            
    # Retire un nombre de ressources égal à la quantité fournie en paramètre
    def volerRessources(self,quantite):
        while quantite > 0 and self._ressources[Ressource.BLE] > 0:
            self._ressources[Ressource.BLE] -= 1
            quantite -= 1
        while quantite > 0 and self._ressources[Ressource.ARGILE] > 0:
            self._ressources[Ressource.ARGILE] -= 1
            quantite -= 1
        while quantite > 0 and self._ressources[Ressource.BOIS] > 0:
            self._ressources[Ressource.BOIS] -= 1
            quantite -= 1
        while quantite > 0 and self._ressources[Ressource.MINERAL] > 0:
            self._ressources[Ressource.MINERAL] -= 1
            quantite -= 1
        while quantite > 0 and self._ressources[Ressource.LAINE] > 0:
            self._ressources[Ressource.LAINE] -= 1
            quantite -= 1


    def afficherRessources(self):
        print "BLÉ:", self._ressources[Ressource.BLE] 
        print "ARGILE:", self._ressources[Ressource.ARGILE] 
        print "BOIS:", self._ressources[Ressource.BOIS] 
        print "MINERAL:", self._ressources[Ressource.MINERAL] 
        print "LAINE:", self._ressources[Ressource.LAINE] 


    def afficher(self):
        print self.nom(), '[', self._id, ']'
        print 'Cartes ressources:', 
        print 'BLE', self._ressources[Ressource.BLE],
        print 'ARGILE', self._ressources[Ressource.ARGILE],
        print 'BOIS', self._ressources[Ressource.BOIS],
        print 'MINERAL', self._ressources[Ressource.MINERAL],
        print 'LAINE', self._ressources[Ressource.LAINE]
        print 'Points de victoire:', self._pointsVictoire
        print 'Chevaliers: (',self._cartesChevalierRecues,self._cartesChevalierActivees,self._cartesChevalierJouees,')'
        print '--------------------------------'









 
