#!/usr/bin/python
#-*- coding: latin-1 -*-

# Contrôleur du jeu de Catane
#
# Date: 22 mars  2014
#
# Auteur: Michel Gagnon



import sys
import getopt
import random
import math
from Joueur import *
from Mappe import *
from Cartes import *
from FabriqueJoueur import *
from Action import *

fabrique = FabriqueJoueur()


class Controleur(object):
    def __init__(self,listeJoueurs,debug=False):
        self._debug = debug
        random.seed()
        self._nombreJoueurs = len(listeJoueurs)

        if self._debug:
            self._numeroSequenceDes = 0
            self._mappe = Mappe(['foret','champ','colline','champ','colline','montagne','pre','foret','montagne','desert','foret','colline','pre','champ','montagne','pre','pre','foret','champ'])
            self._paquetCartes = Cartes(True)
        else:
            random.shuffle(listeJoueurs)
            self._mappe = Mappe()
            self._paquetCartes = Cartes()

        self._joueurs = []
        self._longueurCheminPlusLong = 1
        self._joueurAyantCheminPlusLong = None
        self._armeePlusPuissante = 0
        self._joueurAyantArmeePlusPuissante = None

        for i in range(self._nombreJoueurs):
            self._joueurs.append(fabrique.creerJoueur(listeJoueurs[i],i))


    def jouer(self):
        # Premier tour: chaque joueur place une colonie et une route
        for i in range(self._nombreJoueurs):
            print 'Premier tour,', self._joueurs[i].nom()
            try:
                (positionColonie, extremiteRoute) = self._joueurs[i].premierTour(self._mappe)
                self._mappe._ajouterOccupationInitiale(positionColonie,Occupation.COLONIE,i)
                self._joueurs[i].ajusterCapaciteEchange(self._mappe.obtenirIntersection(positionColonie))
                self._mappe._ajouterRoute(positionColonie, extremiteRoute, i)
                print 'Occupation placée avec succès'
            except RuntimeError as e:
                print 'ERREUR:', e

        # Deuxième tour: en ordre inverse, chaque joueur place une seconde colonie et une seconde route
        for i in range(self._nombreJoueurs-1,-1,-1):
            print 'Deuxième  tour,', self._joueurs[i].nom()
            try:
                (positionColonie, extremiteRoute) = self._joueurs[i].deuxiemeTour(self._mappe)
                self._mappe._ajouterOccupationInitiale(positionColonie,Occupation.COLONIE,i)
                self._joueurs[i].ajusterCapaciteEchange(self._mappe.obtenirIntersection(positionColonie))
                self._mappe._ajouterRoute(positionColonie, extremiteRoute, i)
                print 'Occupation placée avec succès'
                
                # Distribuer les ressources initiales
                self._mappe._distribuerRessourcesInitiales(self._joueurs[i],positionColonie)

            except RuntimeError as e:
                print 'ERREUR:', e



        print 'État après le placement initial:'
        self._mappe.afficher()

        self._afficherEtatJoueurs()

        # À partir de maintenant on commence les tours réguliers
        joueurCourant = 0
        numeroTour = 0
        while not self._joueurGagnant() and numeroTour < 1000:
            numeroTour += 1
            valeur = self._lancerDes()

            print 'Tour', numeroTour
            print 'Valeur des dés',valeur

            # Si la valeur des dés est 7, chaque joueur possédant plus de 7 cartes doit en discarter la 
            # moitié et le joueur courant doit déplacer les voleurs
            if (valeur == 7):
                for joueur in self._joueurs:
                    if joueur.nombreRessources() > 7:
                        print joueur.nom(),"se fait voler"
                        quantiteVolee = joueur.nombreRessources() // 2
                        joueur.volerRessources(quantiteVolee)

                if self._debug:
                    (positionVoleurs,joueurVole) = self._joueurs[joueurCourant].jouerVoleurs(self._mappe,self._infoJoueurs())
                else:
                    (positionVoleurs,joueurVole) = self._joueurs[joueurCourant].jouerVoleurs(self._mappe,self._infoJoueurs())

                if 1 <= positionVoleurs and positionVoleurs <= 19 and joueurVole in range(self._nombreJoueurs):
                    self._mappe._deplacerVoleurs(positionVoleurs)
                    print self._joueurs[joueurCourant].nom(), 'vole', self._joueurs[joueurVole].nom()
                    ressourceVolee = self._joueurs[joueurVole].pigerRessourceAleatoirement()
                    if ressourceVolee == None:
                        print "ERREUR : Ce joueur ne possède aucune ressource"
                    else:
                        self._joueurs[joueurCourant].ajouterRessources(ressourceVolee,1)

            # Sinon, chaque joueur reçoit les ressources auxquelles il a droit
            else:
                ressources = self._mappe._distribuerRessources(valeur)
                for (j,r) in ressources:
                    self._joueurs[j].ajouterRessources(r,ressources[(j,r)])

            # On rend jouables les cartes chevalier recues dans le tour précédent
            self._joueurs[joueurCourant].activerChevaliers()

            print 'Etat des joueurs après distribution des ressources:'
            for i in range(self._nombreJoueurs):
                self._joueurs[i].afficher()
            print 
            print self._joueurs[joueurCourant].nom(), '[', joueurCourant, '] joue'

            # On exécute toutes les actions choisies par le joueur courant
            nombreActions = 0
            if self._debug:
                action = self._joueurs[joueurCourant].choisirAction(self._mappe,self._infoJoueurs(),self._paquetCartes.vide(),self._numeroSequenceDes)
            else:
                action = self._joueurs[joueurCourant].choisirAction(self._mappe,self._infoJoueurs(),self._paquetCartes.vide())

            while action != Action.TERMINER and nombreActions < 50:
                self._executer(action,joueurCourant)
                if self._debug:
                    action = self._joueurs[joueurCourant].choisirAction(self._mappe,self._infoJoueurs(),self._paquetCartes.vide(),self._numeroSequenceDes)
                else:
                    action = self._joueurs[joueurCourant].choisirAction(self._mappe,self._infoJoueurs(),self._paquetCartes.vide())
                nombreActions += 1

            # On passe au joueur suivant
            print "État à la fin du tour"
            self._mappe.afficher()
            self._afficherEtatJoueurs()
            joueurCourant = (joueurCourant+1) % self._nombreJoueurs


    # Retourne une liste contenant un triplet (pv, car, ch) pour chaque joueur
    #  où pv = nombre de points de victoire (excluant les cartes de points de victoire cachées)
    #     car = nombre de cartes ressources que le joueur a en sa possession
    #     ch = nombre de cartes chevalier jouées
    def _infoJoueurs(self):
        infoJoueurs = []
        for i in range(self._nombreJoueurs):
            infoJoueurs.append((self._joueurs[i].nombrePointsVictoireVisibles(), 
                                self._joueurs[i].nombreCartesRessources(), 
                                self._joueurs[i].nombreChevaliers()))
        return infoJoueurs

    def _afficherEtatJoueurs(self):
        for i in range(self._nombreJoueurs):
            self._joueurs[i].afficher()


    def _joueurGagnant(self):
        for j in range(self._nombreJoueurs):
            if self._joueurs[j].nombrePointsVictoire() >= 10:
                print self._joueurs[j].nom(),"a gagné la partie"
                return True
        return False


    def _executer(self,(action,donnees),joueurCourant):
        if action == Action.ACHETER_CARTE:
            print "ACHETER CARTE"
            if (self._joueurs[joueurCourant].quantiteRessources(Ressource.LAINE) >= 1 and
                self._joueurs[joueurCourant].quantiteRessources(Ressource.MINERAL) >= 1 and
                self._joueurs[joueurCourant].quantiteRessources(Ressource.BLE) >= 1):
                if self._paquetCartes.vide():
                    print "ERREUR: Paquet de cartes vide"
                else:
                    self._joueurs[joueurCourant].retirerRessources(Ressource.LAINE,1)
                    self._joueurs[joueurCourant].retirerRessources(Ressource.MINERAL,1)
                    self._joueurs[joueurCourant].retirerRessources(Ressource.BLE,1)            
                    self._joueurs[joueurCourant].ajouterCarte(self._paquetCartes.pigerCarte())
            else:
                print "ERREUR: Pas assez de ressources pour acheter une carte"

        elif action == Action.AJOUTER_COLONIE:
            print "AJOUTER COLONIE"
            if (self._joueurs[joueurCourant].quantiteRessources(Ressource.BOIS) >= 1 and
                self._joueurs[joueurCourant].quantiteRessources(Ressource.ARGILE) >= 1 and
                self._joueurs[joueurCourant].quantiteRessources(Ressource.BLE) >= 1 and
                self._joueurs[joueurCourant].quantiteRessources(Ressource.LAINE) >= 1):
                try:
                    self._mappe._ajouterOccupation(donnees[0],Occupation.COLONIE,joueurCourant)
                    self._joueurs[joueurCourant].ajusterCapaciteEchange(self._mappe.obtenirIntersection(donnees[0]))
                    self._joueurs[joueurCourant].retirerRessources(Ressource.BOIS,1)
                    self._joueurs[joueurCourant].retirerRessources(Ressource.ARGILE,1)
                    self._joueurs[joueurCourant].retirerRessources(Ressource.BLE,1)     
                    self._joueurs[joueurCourant].retirerRessources(Ressource.LAINE,1)     
                    self._joueurs[joueurCourant].augmenterPointsVictoire(1)
                except RuntimeError as e:
                        print 'ERREUR:',e
            else:
                print "ERREUR: Pas assez de ressources pour construire une colonie"


        elif action == Action.AJOUTER_VILLE:
            print "AJOUTER VILLE"
            if (self._joueurs[joueurCourant].quantiteRessources(Ressource.BLE) >= 2 and
                self._joueurs[joueurCourant].quantiteRessources(Ressource.MINERAL) >= 3):
                   try:
                       self._mappe._ajouterOccupation(donnees[0],Occupation.VILLE,joueurCourant)
                       self._joueurs[joueurCourant].retirerRessources(Ressource.BLE,2)     
                       self._joueurs[joueurCourant].retirerRessources(Ressource.MINERAL,3)     
                       self._joueurs[joueurCourant].augmenterPointsVictoire(1)
                   except RuntimeError as e:
                       print 'ERREUR:',e
            else:
                print "ERREUR: Pas assez de ressources pour construire une ville"


        elif action == Action.AJOUTER_ROUTE:  
            print "AJOUTER ROUTE"                        
            if (self._joueurs[joueurCourant].quantiteRessources(Ressource.BOIS) >= 1 and
                self._joueurs[joueurCourant].quantiteRessources(Ressource.ARGILE) >= 1):
                   try:
                       self._mappe._ajouterRoute(donnees[0],donnees[1],joueurCourant)
                       self._joueurs[joueurCourant].retirerRessources(Ressource.BOIS,1)
                       self._joueurs[joueurCourant].retirerRessources(Ressource.ARGILE,1)
                       self._attribuerPointsRoutePlusLongue(joueurCourant)
                   except RuntimeError as e:
                       print 'ERREUR:',e
            else:
                print "ERREUR: Pas assez de ressources pour construire une route"

 
        elif action == Action.JOUER_CARTE_CHEVALIER:
            print "JOUER CARTE CHEVALIER"
            try:
                self._joueurs[joueurCourant].jouerCarteChevalier()
                self._attribuerPointsArmeePlusPuissante(joueurCourant)
                self._mappe._deplacerVoleurs(donnees[0])
                joueurVole = donnees[1]
                if joueurVole in  [v.obtenirOccupant() for v in self._mappe.obtenirTerritoire(donnees[0]).obtenirVoisins()]:
                    ressourceVolee = self._joueurs[joueurVole].pigerRessourceAleatoirement()
                    self._joueurs[joueurCourant].ajouterRessources(ressourceVolee,1)
            except Exception as e:
                print 'ERREUR:', e

        elif action == Action.ECHANGER_RESSOURCES:
            print "ECHANGER"
            self._echanger(joueurCourant,donnees[0],donnees[1],donnees[2])

        else:
            pass # Action invalide

    # On vérifie si le joueur courant a maintenant le chemin le plus long 
    # Si c'est le cas, c'est lui qui récupère les 2 points de victoire
    def _attribuerPointsRoutePlusLongue(self,joueurCourant):
        plusLongCheminJoueurCourant = self._mappe.cheminPlusLong(joueurCourant)
        if plusLongCheminJoueurCourant < 5:
            return
        if plusLongCheminJoueurCourant > self._longueurCheminPlusLong:
            ancienJoueur = self._joueurAyantCheminPlusLong
            if ancienJoueur != None:
                self._joueurs[ancienJoueur].diminuerPointsVictoire(2)
            self._joueurAyantCheminPlusLong = joueurCourant
            self._longueurCheminPlusLong = plusLongCheminJoueurCourant
            self._joueurs[joueurCourant].augmenterPointsVictoire(2)

    # On vérifie si le joueur courant a maintenant l'arméee la plus puissante
    # Si c'est le cas, c'est lui qui récupère les 2 points de victoire                
    def _attribuerPointsArmeePlusPuissante(self,joueurCourant):
        if self._joueurs[joueurCourant].nombreChevaliers() < 3:
            return
        if self._joueurs[joueurCourant].nombreChevaliers() > self._armeePlusPuissante:
            ancienJoueur = self._joueurAyantArmeePlusPuissante
            if ancienJoueur != None:
                self._joueurs[ancienJoueur].diminuerPointsVictoire(2)
            self._joueurAyantArmeePlusPuissante = joueurCourant
            self._armeePlusPuissante = self._joueurs[joueurCourant].nombreChevaliers()
            self._joueurs[joueurCourant].augmenterPointsVictoire(2)            


    def _echanger(self,joueurCourant,quantite,offre,demande):
        if quantite == 4 and self._joueurs[joueurCourant].peutEchangerBanque(offre):
            self._joueurs[joueurCourant].retirerRessources(offre,4)    
            self._joueurs[joueurCourant].ajouterRessources(demande,1)    
        elif quantite == 3 and self._joueurs[joueurCourant].peutEchangerPortGenerique(offre):
            self._joueurs[joueurCourant].retirerRessources(offre,3)    
            self._joueurs[joueurCourant].ajouterRessources(demande,1)    
        elif quantite == 2 and self._joueurs[joueurCourant].peutEchangerPortSpecialise(offre):
            self._joueurs[joueurCourant].retirerRessources(offre,2)    
            self._joueurs[joueurCourant].ajouterRessources(demande,1) 
        else:
            print "ERREUR : Échange impossible, offre insufisante" 



    def _lancerDes(self):
        if self._debug:
            sequence = [6,10,9,2,6,5,9,7,2,4,4,11,12,6,12,6,9,8,8,8,10,2,4,8,9,9,11,2,2,2,8,9,8,9,4,6,2,2,3,4,5,6,8,9,10,11,10,10,10,4,8,2,4,2,2,2,2,2,2,2,4,4,4,4,4,11,2,4,2,4,2,3,11,3,11]
            v = sequence[self._numeroSequenceDes]
            self._numeroSequenceDes += 1
			# On reprend la sequence des Des depuis le debut 
            if (self._numeroSequenceDes ==  len(sequence)):
                self._numeroSequenceDes = 0				
            return v
        else:
            d1 = int(math.ceil(random.random()*6))
            d2 = int(math.ceil(random.random()*6))
            return d1+d2


c = Controleur(['AI','AI','AI','AI'])
c.jouer()


