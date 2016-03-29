#!/usr/bin/python
#-*- coding: latin-1 -*-

# Représentation de la mappe du jeu de Catane
#
# Date: 29 février 2012
#
# Auteur: Michel Gagnon



import sys
import getopt
import random


def incrementer(dictionnaire,cle,increment):
    if cle in dictionnaire:
        dictionnaire[cle] += increment
    else:
        dictionnaire[cle] = increment


def melangerListe(liste):
    random.shuffle(liste)



# Données du graphe 

# Ce dictionnaire indique, pour chaque intersection (de 1 à 54), quelles sont ses intersections voisines
voisinageIntersections = \
  { 1:[2,9], 2:[1,3], 3:[2,4,11], 4:[3,5], 5:[4,6,13], 6:[5,7], 7:[6,15], 
    8:[9,18], 9:[1,8,10], 10:[9,11,20], 11:[3,10,12], 12:[11,13,22], 13:[5,12,14], 14:[13,15,24], 15:[7,14,16], 16:[15,26],
    17:[18,28], 18:[8,17,19], 19:[18,20,30], 20:[10,19,21], 21:[20,22,32], 22:[12,21,23], 23:[22,24,34], 24:[14,23,25], 25:[24,26,36], 26:[16,25,27], 27:[26,38],
    28:[17,29], 29:[28,30,39], 30:[19,29,31], 31:[30,32,41], 32:[21,31,33], 33:[32,34,43], 34:[23,33,35], 35:[34,36,45], 36:[25,35,37], 37:[36,38,47], 38:[27,37],
    39:[29,40], 40:[39,41,48], 41:[31,40,42], 42:[41,43,50], 43:[33,42,44], 44:[43,45,52], 45:[35,44,46], 46:[45,47,54], 47:[37,46],
    48:[40,49], 49:[48,50], 50:[42,49,51], 51:[50,52], 52:[44,51,53], 53:[52,54], 54:[46,53] }
    
# Ce dictionnaire indique, pour chaque intersection (de 1 à 54), quelles sont les territoires voisins
voisinageIntTer= \
  { 1:[1], 2:[1], 3:[1,2], 4:[2], 5:[2,3], 6:[3], 7:[3], 
    8:[4], 9:[1,4], 10:[1,4,5], 11:[1,2,5], 12:[2,5,6], 13:[2,3,6], 14:[3,6,7], 15:[3,7], 16:[7],
    17:[8], 18:[4,8], 19:[4,8,9], 20:[4,5,9], 21:[5,9,10], 22:[5,6,10], 23:[6,10,11], 24:[6,7,11], 25:[7,11,12], 26:[7,12], 27:[12],
    28:[8], 29:[8,13], 30:[8,9,13], 31:[9,13,14], 32:[9,10,14], 33:[10,14,15], 34:[10,11,15], 35:[11,15,16], 36:[11,12,16], 37:[12,16], 38:[12],
    39:[13], 40:[13,17], 41:[13,14,17], 42:[14,17,18], 43:[14,15,18], 44:[15,18,19], 45:[15,16,19], 46:[16,19], 47:[16],
    48:[17], 49:[17], 50:[17,18], 51:[18], 52:[18,19], 53:[19], 54:[19] }


# Ce dictionnaire indique pour chaque territoire, les 6 intersections qui le bordent
voisinageTerritoires = \
  { 1:[1,2,3,9,10,11],
    2:[3,4,5,11,12,13],
    3:[5,6,7,13,14,15],
    4:[8,9,10,18,19,20],
    5:[10,11,12,20,21,22],
    6:[12,13,14,22,23,24],
    7:[14,15,16,24,25,26],
    8:[17,18,19,28,29,30],
    9:[19,20,21,30,31,32],
    10:[21,22,23,32,33,34],
    11:[23,24,25,34,35,36],
    12:[25,26,27,36,37,38],
    13:[29,30,31,39,40,41],
    14:[31,32,33,41,42,43],
    15:[33,34,35,43,44,45],
    16:[35,36,37,45,46,47],
    17:[40,41,42,48,49,50],
    18:[42,43,44,50,51,52],
    19:[44,45,46,52,53,54] }


# Cette liste contient tous les liens (qui relient deux intersections)
liens =  [(1,2), (2,3), (3,4), (4,5), (5,6), (6,7), 
           (1,9), (3,11), (5,13), (7,15), 
           (8,9), (9,10), (10,11), (11,12), (12,13), (13,14), (14,15), (15,16), 
           (8,18), (10,20), (12,22), (14,24), (16,26), 
           (17,18), (18,19), (19,20), (20,21), (21,22), (22,23), (23,24), (24,25), (25,26), (26,27), 
           (17,28), (19,30), (21,32), (23,34), (25,36), (27,38),
           (28,29), (29,30), (30,31), (31,32), (32,33), (33,34), (34,35), (35,36), (36,37), (37,38), 
           (29,39), (31,41), (33,43), (35,45), (37,47), 
           (39,40), (40,41), (41,42), (42,43), (43,44), (44,45), (45,46), (46,47), 
           (40,48), (42,50), (44,52), (46,54),
           (48,49), (49,50), (50,51), (51,52), (52,53), (53,54) ]
          

# Cette liste contient une séquence de valeurs de dés qui seront associés aux territoires
sequenceValeurs = [5,2,6,3,8,10,9,12,11,4,8,10,9,4,5,6,3,11]
# Cette liste indique la séquence de territoires rencontrés si on fait un parcours en spirale à partir du territoire 1
sequenceTerritoires = [1,2,3,7,12,16,19,18,17,13,8,4,5,6,11,15,14,9,10]



# Les 5 types de ressources 
class Ressource:
    BLE = 0
    ARGILE = 1
    BOIS = 2
    MINERAL = 3
    LAINE = 4

# Une occupation, qu'on peut construire sur une intersection
class Occupation:
    COLONIE = 1
    VILLE = 2






#######################################################################################################################
# Classes pour représenter les territoires de la mappe:
#
#     Classe de base:    Territoire
#     Classe dérivées:   Montagne, Pre, Foret, Colline, Champ
#                          
#######################################################################################################################
class Territoire(object):
    # Variable de classe pour la génération d'identificateur
    __compteur__ = 0
 
    def __init__(self):
        # On génère un id
        Territoire.__compteur__ += 1
        self._id = Territoire.__compteur__

        # Initialement un territoire n'a pas de valeur de dé associée et les intersections qui le bordent (voisins)
        # ne sont pas connues
        self._valeur = 0
        self._voisins = []

        # Initialement un territoire n'est pas bloqué par les voleurs
        self._bloqueParVoleurs = False

    def ressource(self):
        return self._ressource
    
    def _placerVoleurs(self):
        self._bloqueParVoleurs = True

    def _retirerVoleurs(self):
        self._bloqueParVoleurs = False

    def bloqueParVoleurs(self):
        return self._bloqueParVoleurs

    # Retourne l'identificateur d'un territoire
    def id(self):
        return self._id

    # Pour fixer la valeur de dés associée au territoire
    def _fixerValeur(self,valeur):
        self._valeur = valeur
  
    # Retourne la valeur de dés associée au territoire
    def valeur(self):
        return self._valeur

    # Retourne une chaîne d'exactement deux caractères pour l'affichage de la valeur dans la mappe
    # (le premier est un espace si valeur < 10)
    def afficherValeur(self):
        if self.bloqueParVoleurs():
            return "XX"
        else:
            return "{:0>2}".format(self._valeur)

    # Ajoute une intersection qui borde le territoire
    def _ajouterVoisin(self, intersection):
        self._voisins.append(intersection)

    # Pour obtenir la liste des intersections qui bordent le territoire
    def obtenirVoisins(self):
        return self._voisins

        

class Montagne(Territoire):
    _ressource = Ressource.MINERAL

    def __init__(self):
        super(Montagne,self).__init__()

    # Pour l'affichage de la mappe
    def afficher(self):
        return 'MN'


class Pre(Territoire):
    _ressource = Ressource.LAINE

    def __init__(self):
        super(Pre,self).__init__()

    # Pour l'affichage de la mappe
    def afficher(self):
        return 'PR'


class Foret(Territoire):
    _ressource = Ressource.BOIS

    def __init__(self):
        super(Foret,self).__init__()

    # Pour l'affichage de la mappe
    def afficher(self):
        return 'FO'


class Champ(Territoire):
    _ressource = Ressource.BLE

    def __init__(self):
        super(Champ,self).__init__()

    # Pour l'affichage de la mappe
    def afficher(self):
        return 'CH'

class Colline(Territoire):
    _ressource = Ressource.ARGILE

    def __init__(self):
        super(Colline,self).__init__()

    # Pour l'affichage de la mappe
    def afficher(self):
        return 'CO'

class Desert(Territoire):
    _ressource = None

    def __init__(self):
        super(Desert,self).__init__()

    # Pour l'affichage de la mappe
    def afficher(self):
        return 'DS'






#######################################################################################################################
# Classes pour représenter les intersections de la mappe:
#
#     Classe de base:    Intersection
#     Classe dérivées:   PortSpecialise, PortGenerique
#                          
#######################################################################################################################



class Intersection(object):
    # Variable de classe pour la génération des identificateurs uniques
    __compteur__ = 0

    def __init__(self):
        Intersection.__compteur__ += 1
        self._id = Intersection.__compteur__
        self._intersectionsVoisines = []
        self._joueurOccupant = None
        self._liensVoisins = []
        self._territoiresVoisins = []
        self._occupation = None

    # Retourne le numéro de l'intersection
    def id(self):
        return self._id

    # Vérifie si l'intersection est occupée par un joueur
    def occupe(self):
        return self._occupation != None

    def occupation(self):
        return self._occupation


    # Retourne le numéro du joueur occupant l'intersection. 
    # Retourne None si aucun joueur ne l'occupe
    def obtenirOccupant(self):
        return self._joueurOccupant

    # Ajoute une intersetion dans la liste des voisins
    def _ajouterVoisin(self, intersection):
        self._intersectionsVoisines.append(intersection)
        
    # Ajoute un territoire dans la liste des territoires voisins
    def ajouterTerritoireVoisin(self, territoire):
        self._territoiresVoisins.append(territoire)
        

    # Retourne la liste des toutes les intersections voisines
    def obtenirVoisins(self):
        return self._intersectionsVoisines

    # Ajoute une occupation (colonie ou ville)
    def _ajouterOccupation(self,occupation, joueur):
        # On teste les cas d'erreur
        if occupation == Occupation.VILLE:
            if self._occupation == None:
                raise RuntimeError('On ne peut construire une ville que sur l\'emplacement d\'une colonie')
            if self._occupation == Occupation.COLONIE and self._joueurOccupant != joueur:
                raise RuntimeError('On ne peut construire une ville sur un emplacement occupé par un autre joueur')
            if self._occupation == Occupation.VILLE:
                raise RuntimeError('Il y a déjà une ville à l\'emplacement spécifié')
        elif occupation == Occupation.COLONIE:
            if self._occupation != None:
                raise RuntimeError('Emplacement déjà occupé')
            
        # On ajoute l'occupation
        self._occupation = occupation
        self._joueurOccupant = joueur


    # Pour l'affichage de la mappe
    # Retourne un chaine indiquant le type d'occupation et le numéro du joueur occupant
    def afficherOccupation(self):
        if self._occupation == Occupation.COLONIE:
            return 'C{0}'.format(self._joueurOccupant)
        elif self._occupation == Occupation.VILLE:
            return 'V{0}'.format(self._joueurOccupant)
        else:
            return '  '

    # Retourne les territoires voisins d'une intersection
    def obtenirTerritoiresVoisins(self):
        return self._territoiresVoisins
        
class PortSpecialise(Intersection):
    def __init__(self, ressource):
        super(PortSpecialise,self).__init__()
        self._ressource = ressource

    def ressource(self):
        return self._ressource

class PortGenerique(Intersection):
    def __init__(self):
        super(PortGenerique,self).__init__()






class Mappe(object):

    # Si on ne veut pas que les territoires soient distibués aléatoirement, on peut passer la séquence
    # en paramètre au constructeur
    def __init__(self,typesTerritoires=None):
        self._territoires = []
        self._intersections = []
        self._routes = {}
        self._initialiserTerritoires(typesTerritoires)
        self._initialiserIntersections()
        self._initialiserVoisinage()
        self._initialiserRoutes()

    # Retourne toutes les intersections de la map
    def obtenirToutesLesIntersections(self):
        return self._intersections

    def _initialiserTerritoires(self,typesTerritoires=None):
        Territoire.__compteur__ = 0

        # Si la séquence des territoire a été fournie au constructeur, on la garde telle quelle.
        # Sino, distribuer aléatoirement les types de territoires 
        if typesTerritoires == None:
            typesTerritoires = ['champ','champ','champ','champ','foret','foret','foret','foret','pre','pre','pre','pre','montagne','montagne','montagne','colline','colline','colline','desert']
            melangerListe(typesTerritoires)
        for t in typesTerritoires:
            if t == 'champ':
                self._territoires.append(Champ())
            elif t == 'foret':
                self._territoires.append(Foret())
            elif t == 'pre':
                self._territoires.append(Pre())
            elif t == 'montagne':
                self._territoires.append(Montagne())
            elif t == 'colline':
                self._territoires.append(Colline())
            elif t == 'desert':
                desert = Desert()
                desert._placerVoleurs()
                self._territoires.append(desert)
            else:
                raise Exception('Type de territoire non défini')

        # Ajouter les valeurs de dés aux territoires
        indiceValeur = 0
        for id in sequenceTerritoires:
            t = self.obtenirTerritoire(id)
            if isinstance(t,Desert):
                continue
            t._fixerValeur(sequenceValeurs[indiceValeur])
            indiceValeur += 1


    # Créer les intersections (incluant les ports)
    def _initialiserIntersections(self):
        Intersection.__compteur__ = 0
        for i in range(1,55):
            if i in [1,2,27,29,39,38,48,49]:
                self._intersections.append(PortGenerique())
            elif i in [4,5]:
                self._intersections.append(PortSpecialise(Ressource.MINERAL))
            elif i in [15,16]:
                self._intersections.append(PortSpecialise(Ressource.BLE))
            elif i in [46,47]:
                self._intersections.append(PortSpecialise(Ressource.BOIS))
            elif i in [51,52]:
                self._intersections.append(PortSpecialise(Ressource.ARGILE))
            elif i in [8,18]:
                self._intersections.append(PortSpecialise(Ressource.LAINE))
            else:
                self._intersections.append(Intersection())

    def _initialiserVoisinage(self):
        # Ajouter, pour chaque territoire, les intersections qui sont voisines
        for territoire in self._territoires:
            for idIntersection in voisinageTerritoires[territoire.id()]:
                intersection = self.obtenirIntersection(idIntersection)
                territoire._ajouterVoisin(intersection)
                intersection.ajouterTerritoireVoisin(territoire)

        # Ajouter, pour chaque intersection, les intersections qui sont voisines
        for intersection in self._intersections:
            for idVoisine in voisinageIntersections[intersection.id()]:
                voisine = self.obtenirIntersection(idVoisine)
                intersection._ajouterVoisin(voisine)
                
    def _initialiserRoutes(self):
        for l in liens:
            self._routes[l] = ' '


    def _ordonnerIntersections(self,i1,i2):
       if i1 < i2:
           return (i1,i2)
       else:
           return (i2,i1) 
        

    # Placer les voleurs sur le territoire dont le id est fourni en paramèetre
    def _deplacerVoleurs(self,id):
        territoireOrigine = self.obtenirTerritoireContenantVoleurs()
        territoireDestination = self.obtenirTerritoire(id)

        if id == territoireOrigine.id():
            raise RuntimeError("On ne peut pas déplacer les voleurs dans le territoire où ils se trouvent déjà")

        territoireOrigine._retirerVoleurs()
        territoireDestination._placerVoleurs()

    # Retourne le territoire qui contient actuellement les voleurs
    def obtenirTerritoireContenantVoleurs(self):
        for t in self._territoires:
            if t.bloqueParVoleurs():
                return t

        # Si on se rend ici, c'est qu'on n'a pas trouvé le territoire contenant les voleurs
        raise RuntimeError("Aucun territoire ne contient les voleurs")
    
    #Retourne la liste de tous les territoires
    def obtenirTousLesTerritoires(self):
        return self._territoires

    # Retourne l'objet correspondant au territoire dont le numéro est passé en paramètre
    def obtenirTerritoire(self,id):
        for t in self._territoires:
            if t.id() == id:
                return t

    # Retourne l'objet correspondant à l'intersection dont le numéro est passé en paramètre
    def obtenirIntersection(self,id):
        for i in self._intersections:
            if i.id() == id:
                return i

    # Retourne la liste de toutes les intersections occupées par un joueur (seulement les id sont retournés)
    def obtenirNumerosIntersectionsJoueur(self,joueur):
        return [intersection.id() for intersection in self._intersections if intersection.obtenirOccupant() == joueur]

    # Ajout de la première ou deuxième occupation d'un joueur (pas nécessaire de vérifier l'accès par une route)
    def _ajouterOccupationInitiale(self,noIntersection,occupation,joueur):
        if self._verifierVoisinage(noIntersection):
            intersection = self.obtenirIntersection(noIntersection)
            intersection._ajouterOccupation(occupation,joueur)
        else:
            raise RuntimeError('Une intersection voisine est déjà occupée')

    # Ajout d'une occupation à une intersection
    def _ajouterOccupation(self,noIntersection,occupation,joueur):
        if occupation == Occupation.VILLE:
            intersection = self.obtenirIntersection(noIntersection)
            if intersection.obtenirOccupant() == joueur:
                intersection._ajouterOccupation(occupation,joueur)
            else:
                raise RuntimeError('Cette Intersection appartien à un autre Joueur')
        elif self.peutConstruireOccupation(noIntersection,joueur):
            intersection = self.obtenirIntersection(noIntersection)
            intersection._ajouterOccupation(occupation,joueur)
        else:
            raise RuntimeError('Une intersection voisine est déjà occupée')
        
    # Verifie si un joueur possède une route qui joint une intersection 
    def _accesRoute(self,noIntersection,joueur):
        voisins = [v.id() for v in self.obtenirIntersection(noIntersection).obtenirVoisins()]
        for i in voisins:
            if self.joueurPossedeRoute(i,noIntersection,joueur):
                return True
        return False

    # Ajout d'une route entre deux intersections voisines
    def _ajouterRoute(self,origine,destination,joueur):
        if origine == destination:
            raise RuntimeError("Une route doit unir deux intersections différentes")

        # Si origine n'est pas < à destination, on échange les valeurs
        (o,d) = self._ordonnerIntersections(origine,destination)

        # On vérifie qu'il existe bien un lien entre les deux intersections qu'on veut unir par une route
        if (o,d) not in liens:
            raise RuntimeError("Tentative de construire une route entre deux intersections qui ne sont pas voisines")

        # On vérifie qu'il n'y a pas déjà une route à l'endroit demandé
        if self._routes[(o,d)] != ' ':
            raise RuntimeError('Il y a déjà une route à l\'emplacement spécifié')

        # On vérifie que la route joint une occupation appartenant au joueur ou poursuit un chemin déjà existant
        if (self.obtenirIntersection(destination).obtenirOccupant() == joueur or
            self.obtenirIntersection(origine).obtenirOccupant() == joueur or
            self._accesRoute(origine,joueur) or
            self._accesRoute(destination,joueur)):
            self._routes[(o,d)] = joueur
        else:
            raise RuntimeError('Le joueur {0} ne peut pas construire une route à cet endroit: {1}-{2}'.format(joueur,origine,destination))
        
    # Vérifie si le joueur peur construire une route entre les deux intersection
    def peutConstruireRoute(self,origine,destination,joueur):
        if origine == destination:
            return False

        if not self.intersectionPossedeRoute(origine,destination):
            return False

        # On vérifie que la route joint une occupation appartenant au joueur ou poursuit un chemin déjà existant
        if (self.obtenirIntersection(destination).obtenirOccupant() == joueur or
            self.obtenirIntersection(origine).obtenirOccupant() == joueur or
            self._accesRoute(origine,joueur) or
            self._accesRoute(destination,joueur)):
            return True
        else:
            return False

    def intersectionPossedeRoute(self,origine,destination):
        # Si origine n'est pas < à destination, on échange les valeurs
        (o,d) = self._ordonnerIntersections(origine,destination)
        # On vérifie qu'il existe bien un lien entre les deux intersections qu'on veut unir par une route
        if (o,d) not in liens:
            return False
        if self._routes[(o,d)] != ' ':
            return False
        return True

    def peutConstruireOccupationInitial(self,noIntersection):
        # On s'assure qu'aucune intersection voisine est occupée par une colonie
        if (self._verifierVoisinage(noIntersection)):
            return True
        else:
            return False

    def peutConstruireOccupation(self,noIntersection,joueur):
        # On s'assure qu'aucune intersection voisine est occupée par une colonie ou une ville
        # et qu'une route se rend à l'emplacement
        if (self._verifierVoisinage(noIntersection) and
            self._accesRoute(noIntersection,joueur)):
            return True
        else:
            return False

    # Verifie s'il n'y a pas déjà une occupation dans une intersection voisine
    def _verifierVoisinage(self,noIntersection):
        voisinsOccupes = [v for v in self.obtenirIntersection(noIntersection).obtenirVoisins() if v.occupe()]
        if len(voisinsOccupes) > 0:
            return False
        else:
            return True

    # Verifie si un joueur possède une route entre deux intersections
    def joueurPossedeRoute(self,origine,destination,joueur):
        (o,d) = self._ordonnerIntersections(origine,destination)
        return self._routes[(o,d)] == joueur


    # Retourne, pour chaque territoire correspondant à la valeur de dé passé en paramètre, 
    # le nombre de ressources correspondantes retournées à chaque joueur
    #     NoJoueur --> Ressource --> Nombre
    def _distribuerRessources(self,valeur):
        resultat = {}
        territoiresConcernes = [t for t in self._territoires if t.valeur() == valeur]
        for t in territoiresConcernes:
            if not t.bloqueParVoleurs():
                for intersection in t.obtenirVoisins():
                    if intersection.occupation() == Occupation.COLONIE:
                        incrementer(resultat,(intersection.obtenirOccupant(),t.ressource()),1)
                    elif intersection.occupation() == Occupation.VILLE:
                        incrementer(resultat,(intersection.obtenirOccupant(),t.ressource()),2)
        return resultat

    # Distribution initiale des ressources, après le placement des 2 premières colonies
    def _distribuerRessourcesInitiales(self,joueur,idIntersection):
        for t in self._territoires:
            if not isinstance(t,Desert):
                for intersection in t.obtenirVoisins():
                    if intersection.id() == idIntersection:                
                        joueur.ajouterRessources(t.ressource(),1)


    # Retourne la longueur du chemin le plus long pour le joueur spécifié
    def cheminPlusLong(self,joueur):
        # On extrait toutes les intersections couvertes par au moins une route du joueur
        intersections = [i for i in range(1,55) if self._accesRoute(i,joueur)]

        max = 0
        for i in intersections:
            # On calcule la longueur du chemin le plus long à partir de cette intersection
            l = self._plusLong(i,[],joueur)
            # S'il s'ajout du plus long jusqu'à maintenant, on mémorise cette valeur
            if l > max:
                max = l

        return max

    # Retourne la longueur du chemin le plus long à partir d'une intersections donnée
    def _plusLong(self,intersection,routesParcourues,joueur):
        # intersections voisines qu'on accède par une route qu'on n'a encore jamais traversée
        voisins = [v for (i,v) in self._routes if (i == intersection and self._routes[(i,v)] == joueur and (i,v) not in routesParcourues)] + \
                  [v for (v,i) in self._routes if (i == intersection and self._routes[(v,intersection)] == joueur and (v,i) not in routesParcourues)] 
        
        if len(voisins) == 0:
            return 0
        else:
            max = 0
            for v in voisins:
                routesParcouruesUpdate = routesParcourues + [(intersection,v),(v,intersection)]
                l = self._plusLong(v,routesParcouruesUpdate,joueur)
                if l > max:
                    max = l
            return max + 1

    # Si un joueur a construit une route sur le lien en question on retourne le numéro du joueur entre crochet []
    # Sinon on retourne la chaine passée en paramètre 
    def _afficherRoute(self,chaine,origine,destination):
         if origine == destination:
             return chaine

         (o,d) = self._ordonnerIntersections(origine,destination)
       
         if self._routes[(o,d)] != ' ':
             return '[' + str(self._routes[(o,d)]) + ']'
         else:
             return chaine



    # Affichage de l'état courant de la mappe
    def afficher(self):
        occup = {}
        for i in range(1,55):
            occup[i] = self.obtenirIntersection(i).afficherOccupation()

        terr = {}
        for i in range(1,20):
            terr[i] = self.obtenirTerritoire(i).afficher() + '-' + self.obtenirTerritoire(i).afficherValeur()


        print '                                    ({})___{}___({})'.format(occup[17],self._afficherRoute('___',17,28),occup[28])
        print '                                      /           \ '
        print '                                    {}           {} '.format(self._afficherRoute(' / ',17,18),self._afficherRoute(' \ ',28,29))
        print '                                 L  /               \ G '
        print '                   L ({})___{}___({})     {}    ({})___{}___({})  G  '.format(occup[8],self._afficherRoute('___',8,18),occup[18],terr[8],occup[29],self._afficherRoute('___',29,39),occup[39])
        print '                       /           \                 /           \ ' 
        print '                     {}           {}             {}           {} '.format(self._afficherRoute(' / ',8,9),self._afficherRoute(' \ ',18,19),self._afficherRoute(' / ',29,30),self._afficherRoute(' \ ',39,40))
        print '                     /               \             /               \ '
        print '    G  ({})___{}___({})    {}    ({})___{}___({})     {}   ({})___{}___({})  G  '.format(occup[1],self._afficherRoute('___',1,9), occup[9],terr[4],occup[19],self._afficherRoute('___',19,30),occup[30],terr[13],occup[40],self._afficherRoute('___',40,48),occup[48])               
        print '        /           \                 /           \                 /           \  '
        print '      {}           {}             {}           {}             {}           {}  '.format(self._afficherRoute(' / ',1,2),self._afficherRoute(' \ ',9,10),self._afficherRoute(' / ',19,20),self._afficherRoute(' \ ',30,31),self._afficherRoute(' / ',40,41),self._afficherRoute(' \ ',48,49))
        print '      /               \             /               \             /               \  '
        print '  G ({})    {}    ({})___{}___({})    {}    ({})___{}___({})    {}     ({})   G  '.format(occup[2],terr[1],occup[10],self._afficherRoute('___',10,20),occup[20],terr[9],occup[31],self._afficherRoute('___',31,41),occup[41],terr[17],occup[49]) 
        print '     \                 /           \                 /           \                 /   '
        print '     {}             {}           {}             {}           {}             {}  '.format(self._afficherRoute(' \ ',2,3),self._afficherRoute(' / ',10,11),self._afficherRoute(' \ ',20,21),self._afficherRoute(' / ',31,32),self._afficherRoute(' \ ',41,42),self._afficherRoute(' / ',49,50))
        print '       \             /               \             /               \             /     '
        print '       ({})___{}___({})    {}    ({})___{}___({})    {}    ({})___{}___({})     '.format(occup[3],self._afficherRoute('___',3,11),occup[11],terr[5],occup[21],self._afficherRoute('___',21,32),occup[32],terr[14],occup[42],self._afficherRoute('___',42,50),occup[50])               
        print '        /           \                 /           \                 /           \   '   
        print '      {}           {}             {}           {}             {}           {}  '.format(self._afficherRoute(' / ',3,4),self._afficherRoute(' \ ',11,12),self._afficherRoute(' / ',21,22),self._afficherRoute(' \ ',32,33),self._afficherRoute(' / ',42,43),self._afficherRoute(' \ ',50,51))
        print '      /               \             /               \             /               \   ' 
        print ' M  ({})    {}     ({})___{}___({})   {}    ({})___{}___({})    {}     ({})   A '.format(occup[4],terr[2],occup[12],self._afficherRoute('___',12,22),occup[22],terr[10],occup[33],self._afficherRoute('___',33,43),occup[43],terr[18],occup[51])
        print '     \                 /           \                 /           \                 /  '
        print '     {}             {}           {}             {}           {}             {}  '.format(self._afficherRoute(' \ ',4,5),self._afficherRoute(' / ',12,13),self._afficherRoute(' \ ',22,23),self._afficherRoute(' / ',33,34),self._afficherRoute(' \ ',43,44),self._afficherRoute(' / ',51,52))
        print '       \             /               \             /               \             /  '
        print '     M ({})___{}___({})     {}   ({})___{}___({})    {}    ({})___{}___({}) A  '.format(occup[5],self._afficherRoute('___',5,13),occup[13],terr[6],occup[23],self._afficherRoute('___',23,34),occup[34],terr[15],occup[44],self._afficherRoute('___',44,52),occup[52])
        print '        /           \                 /           \                 /           \    '  
        print '      {}           {}             {}           {}             {}           {}  '.format(self._afficherRoute(' / ',5,6),self._afficherRoute(' \ ',13,14),self._afficherRoute(' / ',23,24),self._afficherRoute(' \ ',34,35),self._afficherRoute(' / ',44,45),self._afficherRoute(' \ ',52,53))
        print '      /               \             /               \             /               \    '
        print '    ({})   {}      ({})___{}___({})    {}    ({})___{}___({})     {}   ({})  '.format(occup[6],terr[3],occup[14],self._afficherRoute('___',14,24),occup[24],terr[11],occup[35],self._afficherRoute('___',35,45),occup[45],terr[19],occup[53]) 
        print '     \                 /           \                 /           \                 / ' 
        print '     {}             {}           {}             {}           {}             {}  '.format(self._afficherRoute(' \ ',6,7),self._afficherRoute(' / ',14,15),self._afficherRoute(' \ ',24,25),self._afficherRoute(' / ',35,36),self._afficherRoute(' \ ',45,46),self._afficherRoute(' / ',53,54))
        print '       \             /               \             /               \             /  '
        print '      ({})__{}___({})     {}     ({})___{}___({})    {}    ({})___{}___({}) '.format(occup[7],self._afficherRoute('___',7,15),occup[15],terr[7],occup[25],self._afficherRoute('___',25,36),occup[36],terr[16],occup[46],self._afficherRoute('___',46,54),occup[54])               
        print '                  B \                 /           \                 /  W '
        print '                    {}             {}           {}             {}   '.format(self._afficherRoute(' \ ',15,16),self._afficherRoute(' / ',25,26),self._afficherRoute(' \ ',36,37),self._afficherRoute(' / ',46,47))
        print '                      \             /               \             /    '
        print '                     ({})___{}___({})    {}     ({})__{}___({}) W  '.format(occup[16],self._afficherRoute('___',16,26),occup[26],terr[12],occup[37],self._afficherRoute('___',37,47),occup[47])
        print '                      B            \                 /  '
        print '                                   {}             {}   '.format(self._afficherRoute(' \ ',26,27),self._afficherRoute(' / ',37,38))  
        print '                                     \             /    '
        print '                                  G ({})___{}___({}) G  '.format(occup[27],self._afficherRoute('___',27,38),occup[38])    


    # Afficher la mappe en indiquant seulement les numéros des territoires et des intersections    
    def afficherDebug(self):
        print '                                    ({})_________({})' .format(17,28)
        print '                                      /           \ '
        print '                                     /             \ '
        print '                                 L  /               \ G '
        print '                   L ({})_________({})     {}    ({})_________({})  G  '.format(8,18,'  8  ',29,39)
        print '                       /           \                 /           \ ' 
        print '                      /             \               /             \ '
        print '                     /               \             /               \ '
        print '    G  ({})_________({})    {}    ({})_________({})     {}   ({})_________({})  G  '.format(1, 9,'  4  ',19,30,'  13 ',40,48)               
        print '        /           \                 /           \                 /           \  '
        print '       /             \               /             \               /             \  '
        print '      /               \             /               \             /               \  '
        print '  G ({})    {}    ({})_________({})    {}    ({})_________({})    {}     ({})   G  '.format(2,'  1  ',10,20,'  9  ',31,41,' 17  ',49) 
        print '     \                 /           \                 /           \                 /   '
        print '      \               /             \               /             \               /   '
        print '       \             /               \             /               \             /     '
        print '       ({})_________({})    {}    ({})_________({})    {}    ({})_________({})     '.format(3,11,'  5  ',21,32,' 14 ',42,50)               
        print '        /           \                 /           \                 /           \   '   
        print '       /             \               /             \               /             \   '
        print '      /               \             /               \             /               \   ' 
        print ' M  ({})    {}     ({})_________({})   {}    ({})_________({})    {}     ({})   A '.format(4,'  2  ',12,22,' 10  ',33,43,' 18 ',51)
        print '     \                 /           \                 /           \                 /  '
        print '      \               /             \               /             \               /   '
        print '       \             /               \             /               \             /  '
        print '     M ({})_________({})     {}   ({})_________({})    {}    ({})_________({}) A  '.format(5,13,'  6  ',23,34,' 15  ',44,52)
        print '        /           \                 /           \                 /           \    '  
        print '       /             \               /             \               /             \   '
        print '      /               \             /               \             /               \    '
        print '    ({})   {}      ({})_________({})    {}    ({})_________({})     {}   ({})  '.format(6,'  3  ',14,24,' 11  ',35,45,' 19  ',53) 
        print '     \                 /           \                 /           \                 / ' 
        print '      \               /             \               /             \               /   '
        print '       \             /               \             /               \             /  '
        print '      ({})________({})     {}     ({})_________({})    {}    ({})_________({}) '.format(7,15,'  7  ',25,36,' 16  ',46,54)               
        print '                  B \                 /           \                 /  W '
        print '                     \               /             \               /    '
        print '                      \             /               \             /    '
        print '                     ({})_________({})    {}     ({})________({}) W  '.format(16,26,' 12  ',37,47)
        print '                      B            \                 /  '
        print '                                    \               /    '
        print '                                     \             /    '
        print '                                  G ({})_________({}) G  '.format(27,38)    









