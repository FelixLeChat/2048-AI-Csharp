#!/usr/bin/python
#-*- coding: latin-1 -*-

# Joueur du jeu de Catane
#
# Date: 29 f�vrier 2012
#
# Auteur: Michel Gagnon

# Chaque joueur est impl�ment� par une classe qui d�rive de la classe Joueur
# Chaque classe de joueur sp�cifique doit �tre ajout�e dans la fabrique de joueurs et doit 
# red�finir les quatre m�thodes suivantes:
#
#  premierTour():  choix de l'emplacement de la premi�re colonie et de la premi�re route
#  deuxiemeTour(): choix de l'emplacement de la deuxi�me colonie et de la deuxi�me route
#
#        Ces deux m�thodes retournent une paire (C,R) o� C est le num�ro de l'intersection 
#        o� doit �tre plac� la colonie et R le num�ro de l'intersection correspondant � l'autre
#        extr�mit� de la route qui part de la colonie
#
# chosirAction(mappe,infoJoueurs,paquetVide): c'est cette m�thode qui d�termine la prochaine action effectu�e
#        par le joueur. On lui passe la mappe et l'information publique sur les joueur (voir plus loin)
#        Elle retourne une paire (A, I) o� A identifie l'action et I
#        est une liste qui fournit les �l�ments d'information n�cessaires pour r�aliser
#        l'action. Les possibilit�s sont les suivantes:
#
#        (Action.ACHETER_CARTE, []) - Pour acheter une carte de d�veloppement
#        (Action.ECHANGER_RESSOURCES, [Quantit�, RessourceOfferte, RessourceDemand�e])
#        (Action.AJOUTER_COLONIE,[Position])
#        (Action.AJOUTER_VILLE,[Position])
#        (Action.AJOUTER_ROUTE,[I1,I2]) - Pour ajouter une route liant les intersections I1 et I2
#        (Action.JOUER_CARTE_CHEVALIER, [V,J] - o� V est le num�ro du territoire o� seront d�plac�s les voleurs
#                                                  J est le num�ro du joueur � qui on volera une carte
#        Action.TERMINER - Pour indiquer que le joueur a termin� son tour
#
#        Le param�tre infoJoueurs est une liste contenant les infos publique sur chaque joueur:
#            infoJoueurs[i] = infos sur joueur i = (pv, cr, ch) 
#                   o� pv = nombre de points de victoire (excluant les cartes de points de victoire cach�es)
#                      cr = nombre de cartes ressources que le joueur a en sa possession
#                      ch = nombre de cartes chevalier jou�es
#
#
# joueVoleurs(mappe): retourne une paire (T,J), o� T est le num�ro du territore o� on doit d�placer les
#        voleur, et J le num�ro du joueur � qui on volera une carte
#
#
# volerRessources(quantit�): cette fonction doit �liminer de la r�serve des ressources la quantit�
#        fournie en argument. C'est au joueur d'identifier les ressources qu'il d�sire �liminer


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

        # Les cartes ressources que le joueur poss�de dans sa main
        self._ressources = {}
        self._ressources[Ressource.BLE] = 0
        self._ressources[Ressource.ARGILE] = 0
        self._ressources[Ressource.BOIS] = 0
        self._ressources[Ressource.MINERAL] = 0
        self._ressources[Ressource.LAINE] = 0

        # Le total des points de victoire du joueur
        self._pointsVictoire = 2

        # Les cartes chvalier ne peuvent pas �tre jou�ees durant le tour o� elles ont �t� achet�es
        # Une carte achet�e dans le tour courant est une carte "re�ue". Au tour suivant
        # elle devient une carte "acitv�e", c'est-�-dire qu'elle peut �tre jou�e.
        self._cartesChevalierRecues = 0
        self._cartesChevalierActivees = 0
        self._cartesChevalierJouees = 0

        # On m�morise les points de victoires obtenus par le biais de cartes de d�veloppement
        self._cartesPointsVictoire = 0

        # La liste des types de ressources que le joueur peut �changer au taux 2:1
        # (ceci d�pend des ports sp�cialis�s qu'il poss�de)
        self._peutEchanger = []

        # Indique si le joueur peut �changer des ressources au taux 3:1
        self._possedePortGenerique = False


    # M�thodes devant �tre red�finies par la sous-classe:
    
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


     
    # M�thodes d�finies seulement dans la classe de base

    # Retourne le num�ro d'identification du joueur
    def id(self):
        return self._id

    # Nombre de chevaliers qui ont �t� jou�s par le joueur
    def nombreChevaliers(self):
        return self._cartesChevalierJouees

    # Cette m�thode est appel�e par le contr�leur pour activer les cartes chevalier
    # achet�es au tour pr�c�dent
    def activerChevaliers(self):
        self._cartesChevalierActivees += self._cartesChevalierRecues
        self._cartesChevalierRecues = 0

    # Cette m�thode est appel�e par le contr�ler pour une carte chevalier de l'�tat
    # "activ�e" � l'�tat "jou�e"
    def jouerCarteChevalier(self):
        if self.peutJouerCarteChevalier():
            self._cartesChevalierActivees -= 1
            self._cartesChevalierJouees += 1
        else:
            raise RuntimeError("Tentative de jouer une carte chevalier qui n'existe pas")

    def peutJouerCarteChevalier(self):
        return self._cartesChevalierActivees > 0


    # Cette m�thode est appel�e lorque le joueur vient de placer une colonie sur un port, 
    # afin de d�terminer la capacit� d'�change que permet ce port
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

    # Met � jour l'�tat du joueur selon la carte de d�veloppement qu'il vient d'acheter
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

    # Choisit al�atoire une des ressource du joueur et la retire de sa pile
    # La fonction retourne le type de la ressource retir�e
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
            
    # Retire un nombre de ressources �gal � la quantit� fournie en param�tre
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
        print "BL�:", self._ressources[Ressource.BLE] 
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









 
