#!/usr/bin/python
#-*- coding: latin-1 -*-

from Mappe  import *
import unittest

class TestPrincipal(unittest.TestCase):

    def setUp(self):
        self.mappe = Mappe()


    def tearDown(self):
        pass
        #self.mappe.afficher()
        
    def testAjouterOccupationInitiale1(self):
        self.mappe._ajouterOccupationInitiale(23,Occupation.COLONIE,1)
        self.assertTrue(self.mappe.obtenirIntersection(23).occupe())
        self.assertFalse(self.mappe.joueurPossedeRoute(23,24,1))
        print "Test - Occupation initiale 1"

        
    def testAjouterOccupationInitiale2(self):
        self.mappe._ajouterOccupationInitiale(23,Occupation.COLONIE,1)
        self.mappe._ajouterRoute(23,24,1)
        self.assertTrue(self.mappe.obtenirIntersection(23).occupe())
        self.assertTrue(self.mappe.joueurPossedeRoute(23,24,1))
        print "Test - Occupation initiale 2"

    def testAjouterOccupationInitiale3(self):
        self.mappe._ajouterOccupationInitiale(23,Occupation.COLONIE,1)
        self.mappe._ajouterRoute(23,24,1)
        self.assertRaisesRegexp(RuntimeError,'Une intersection voisine est déjà occupée',self.mappe._ajouterOccupationInitiale,22,Occupation.COLONIE,2)
        print "Test - Occupation initiale 3"


    def testAjoutColonie(self):
        self.mappe._ajouterOccupationInitiale(23,Occupation.COLONIE,1)
        self.assertRaisesRegexp(RuntimeError,'Emplacement déjà occupé',self.mappe._ajouterOccupationInitiale,23,Occupation.COLONIE,1)
        print "Test- Ajout colonie"

    def testAjoutVille1(self):
        self.mappe._ajouterOccupationInitiale(23,Occupation.COLONIE,1)
        self.mappe._ajouterRoute(23,24,1)
        self.mappe._ajouterRoute(14,24,1)
        self.assertRaisesRegexp(RuntimeError,'On ne peut construire une ville que sur l\'emplacement d\'une colonie',self.mappe._ajouterOccupationInitiale,14,Occupation.VILLE,1)
        print "Test - Ajout ville 1"


    def testAjoutVille2(self):
        self.mappe._ajouterOccupationInitiale(23,Occupation.COLONIE,1)
        self.assertRaisesRegexp(RuntimeError,'On ne peut construire une ville sur un emplacement occupé par un autre joueur',self.mappe._ajouterOccupationInitiale,23,Occupation.VILLE,2)
        print "Test - Ajout ville 2"

    def testAjoutVille3(self):
        self.mappe._ajouterOccupationInitiale(23,Occupation.COLONIE,1)
        self.mappe._ajouterOccupationInitiale(23,Occupation.VILLE,1)
        self.assertEqual(self.mappe.obtenirIntersection(23).afficherOccupation(),'V1') 
        print "Test - Ajout ville 3"

    def testAjoutVille4(self):
        self.mappe._ajouterOccupationInitiale(23,Occupation.COLONIE,1)
        self.mappe._ajouterOccupationInitiale(23,Occupation.VILLE,1)
        self.assertRaisesRegexp(RuntimeError,'Il y a déjà une ville à l\'emplacement spécifié',self.mappe._ajouterOccupationInitiale,23,Occupation.VILLE,2)
        print "Test - Ajout ville 4"

    def testAjoutRoute1(self):
        self.mappe._ajouterOccupationInitiale(23,Occupation.COLONIE,1)
        self.mappe._ajouterRoute(23,24,1)
        self.mappe._ajouterRoute(25,24,1)
        self.assertTrue(self.mappe.joueurPossedeRoute(24,25,1))
        print "Test - Ajouter route 1"
        
    def testAjoutRoute2(self):
        self.mappe._ajouterOccupationInitiale(23,Occupation.COLONIE,1)
        self.mappe._ajouterRoute(23,24,1)
        self.assertRaisesRegexp(RuntimeError,'Le joueur 1 ne peut pas construire une route à cet endroit',self.mappe._ajouterRoute,18,19,1)
        print "Test - Ajouter route 2"

    def testAjoutRoute3(self):
        self.mappe._ajouterOccupationInitiale(23,Occupation.COLONIE,1)
        self.mappe._ajouterRoute(23,24,1)
        self.assertRaisesRegexp(RuntimeError,'Le joueur 2 ne peut pas construire une route à cet endroit',self.mappe._ajouterRoute,24,25,2)
        print "Test - Ajouter route 3"

    def testPeutConstruireOccupation1(self):
        self.mappe._ajouterOccupationInitiale(23,Occupation.COLONIE,1)
        self.mappe._ajouterRoute(23,24,1)
        self.assertRaisesRegexp(RuntimeError,'Une intersection voisine est déjà occupée',self.mappe._ajouterOccupation,24,Occupation.COLONIE,1)
        print "Test - Peut contruire occupation 1"

    def testPeutConstruireOccupation2(self):
        self.mappe._ajouterOccupationInitiale(23,Occupation.COLONIE,1)
        self.mappe._ajouterRoute(23,24,1)
        self.assertRaisesRegexp(RuntimeError,'Une intersection voisine est déjà occupée',self.mappe._ajouterOccupation,25,Occupation.COLONIE,1)
        print "Test - Peut contruire occupation 2"

    def testPeutConstruireOccupation3(self):
        self.mappe._ajouterOccupationInitiale(23,Occupation.COLONIE,1)
        self.mappe._ajouterRoute(23,24,1)
        self.assertRaisesRegexp(RuntimeError,'Une intersection voisine est déjà occupée',self.mappe._ajouterOccupation,25,Occupation.COLONIE,2)
        print "Test - Peut contruire occupation 3"

    def testPeutConstruireOccupation4(self):
        self.mappe._ajouterOccupationInitiale(23,Occupation.COLONIE,1)
        self.mappe._ajouterRoute(23,24,1)
        self.mappe._ajouterRoute(24,25,1)
        self.mappe._ajouterOccupation(25,Occupation.COLONIE,1)
        self.assertTrue(self.mappe.obtenirIntersection(25).occupe())
        print "Test - Peut construire occupation 4"

    def testPeutConstruireOccupation5(self):
        self.mappe._ajouterOccupationInitiale(23,Occupation.COLONIE,1)
        self.mappe._ajouterOccupationInitiale(33,Occupation.COLONIE,1)
        self.mappe._ajouterOccupationInitiale(35,Occupation.COLONIE,1)
        self.assertRaisesRegexp(RuntimeError,'Une intersection voisine est déjà occupée',self.mappe._ajouterOccupation,34,Occupation.COLONIE,2)
        print "Test - Peut construire occupation 5"


    def testCheminPlusLong1(self):
        self.mappe._ajouterOccupationInitiale(23,Occupation.COLONIE,1)
        self.mappe._ajouterOccupationInitiale(12,Occupation.COLONIE,2)
        self.mappe._ajouterOccupationInitiale(43,Occupation.COLONIE,2)

        self.mappe._ajouterRoute(23,24,1)
        self.mappe._ajouterRoute(24,25,1)
        self.mappe._ajouterRoute(25,36,1)
        self.mappe._ajouterRoute(36,37,1)
        self.mappe._ajouterRoute(37,38,1)
        self.mappe._ajouterRoute(38,27,1)
        self.mappe._ajouterRoute(37,47,1)
        self.mappe._ajouterRoute(26,27,1)
        self.mappe._ajouterRoute(26,25,1)
        self.mappe._ajouterRoute(16,26,1)
        self.mappe._ajouterRoute(15,16,1)
        
        self.mappe._ajouterRoute(43,44,2)
        self.mappe._ajouterRoute(52,44,2)
        self.mappe._ajouterRoute(12,13,2)
        
        self.mappe._ajouterOccupation(38,Occupation.COLONIE,1)
        self.mappe._ajouterOccupation(43,Occupation.VILLE,2)

        self.assertEqual(self.mappe.cheminPlusLong(1), 9)
        self.assertEqual(self.mappe.cheminPlusLong(2), 2)

        print "Test - Chemin plus long 1"

    def testCheminPlusLong2(self):
        self.mappe._ajouterOccupationInitiale(12,Occupation.COLONIE,2)
        self.mappe._ajouterOccupationInitiale(43,Occupation.COLONIE,2)

        self.mappe._ajouterRoute(43,44,2)
        self.mappe._ajouterRoute(52,44,2)
        self.mappe._ajouterRoute(45,44,2)
        self.mappe._ajouterRoute(45,46,2)
        self.mappe._ajouterRoute(12,13,2)
        
        self.assertEqual(self.mappe.cheminPlusLong(2), 3)

        print "Test - Chemin plus long 2"

    def testCheminPlusLong3(self):
        self.mappe._ajouterOccupationInitiale(12,Occupation.COLONIE,2)
        self.mappe._ajouterOccupationInitiale(43,Occupation.COLONIE,2)

        self.mappe._ajouterRoute(43,44,2)
        self.mappe._ajouterRoute(52,44,2)
        self.mappe._ajouterRoute(45,44,2)
        self.mappe._ajouterRoute(45,46,2)
        self.mappe._ajouterRoute(12,13,2)
        self.mappe._ajouterRoute(12,22,2)
        self.mappe._ajouterRoute(21,22,2)
        
        self.assertEqual(self.mappe.cheminPlusLong(2), 3)

        print "Test - Chemin plus long 3"


    def testCheminPlusLong4(self):
        self.mappe._ajouterOccupationInitiale(12,Occupation.COLONIE,2)
        self.mappe._ajouterOccupationInitiale(43,Occupation.COLONIE,2)

        self.mappe._ajouterRoute(43,44,2)
        self.mappe._ajouterRoute(52,44,2)
        self.mappe._ajouterRoute(45,44,2)
        self.mappe._ajouterRoute(45,46,2)
        self.mappe._ajouterRoute(12,13,2)
        self.mappe._ajouterRoute(12,22,2)
        self.mappe._ajouterRoute(21,22,2)
        self.mappe._ajouterRoute(5,13,2)
        
        self.assertEqual(self.mappe.cheminPlusLong(2), 4)

        print "Test - Chemin plus long 4"


    def testVoleurs1(self):
        t = self.mappe.obtenirTerritoireContenantVoleurs()
        self.assertTrue(isinstance(t,Desert))
        print "Test - Voleurs 1"

    def testVoleurs3(self):
        typesTerritoires = ['champ','champ','champ','champ','foret','foret','foret','foret','pre','pre','pre','pre','montagne','montagne','montagne','colline','colline','colline','desert']
        self.mappe = Mappe(typesTerritoires)
        self.mappe._deplacerVoleurs(7)
        t = self.mappe.obtenirTerritoireContenantVoleurs()
        self.assertEqual(t.id(),7)
        print "Test - Voleurs 3"


    def testVoleurs4(self):
        typesTerritoires = ['champ','champ','champ','champ','foret','foret','foret','foret','pre','pre','pre','pre','montagne','montagne','montagne','colline','colline','colline','desert']
        self.mappe = Mappe(typesTerritoires)
        self.mappe._deplacerVoleurs(10)
        self.assertRaisesRegexp(RuntimeError,'On ne peut pas déplacer les voleurs dans le territoire où ils se trouvent déjà',self.mappe._deplacerVoleurs,10)
        print "Test - Voleurs 4"

    def testVoleurs5(self):
        typesTerritoires = ['champ','champ','champ','champ','foret','foret','foret','foret','pre','pre','pre','pre','montagne','montagne','montagne','colline','colline','colline','desert']
        self.mappe = Mappe(typesTerritoires)
        self.mappe._deplacerVoleurs(5)
        self.mappe._deplacerVoleurs(18)
        t = self.mappe.obtenirTerritoireContenantVoleurs()
        self.assertEqual(t.id(),18)
        print "Test - Voleurs 5"
        #self.mappe.afficher()

        
    def testDistribuerRessources1(self):
        typesTerritoires = ['champ','champ','champ','champ','foret','foret','foret','foret','pre','pre','pre','pre','montagne','montagne','montagne','colline','colline','colline','desert']
        self.mappe = Mappe(typesTerritoires)
        self.mappe._ajouterOccupationInitiale(12,Occupation.COLONIE,1)       
        ressources = self.mappe._distribuerRessources(2)
        self.assertEqual(ressources[(1,Ressource.BLE)],1)
        print "Test - Distribuer ressources 1"

    def testDistribuerRessources2(self):
        typesTerritoires = ['champ','champ','champ','champ','foret','foret','foret','foret','pre','pre','pre','pre','montagne','montagne','montagne','colline','colline','colline','desert']
        self.mappe = Mappe(typesTerritoires)
        self.mappe._ajouterOccupationInitiale(24,Occupation.COLONIE,1)
        self.mappe._deplacerVoleurs(7)
        ressources = self.mappe._distribuerRessources(3)
        self.assertEqual(len(ressources),0)
        print "Test - Distribuer ressources 2"


    def testDistribuerRessources3(self):
        typesTerritoires = ['champ','champ','champ','champ','foret','foret','foret','foret','pre','pre','pre','pre','montagne','montagne','montagne','colline','colline','colline','desert']
        self.mappe = Mappe(typesTerritoires)
        self.mappe._ajouterOccupationInitiale(24,Occupation.COLONIE,1)
        self.mappe._ajouterOccupationInitiale(24,Occupation.VILLE,1)
        ressources = self.mappe._distribuerRessources(3)
        self.assertEqual(ressources[(1,Ressource.BOIS)],2)
        print "Test - Distribuer ressources 3"

    def testDistribuerRessources4(self):
        typesTerritoires = ['champ','champ','champ','champ','foret','foret','foret','foret','pre','pre','pre','pre','montagne','montagne','montagne','colline','colline','colline','desert']
        self.mappe = Mappe(typesTerritoires)
        self.mappe._ajouterOccupationInitiale(24,Occupation.COLONIE,1)
        self.mappe._ajouterOccupationInitiale(24,Occupation.VILLE,1)
        self.mappe._ajouterOccupationInitiale(16,Occupation.COLONIE,1)
        ressources = self.mappe._distribuerRessources(3)
        self.assertEqual(ressources[(1,Ressource.BOIS)],3)
        print "Test - Distribuer ressources 4"

    def testDistribuerRessources5(self):
        typesTerritoires = ['champ','champ','champ','champ','foret','foret','foret','foret','pre','pre','pre','pre','montagne','montagne','montagne','colline','colline','colline','desert']
        self.mappe = Mappe(typesTerritoires)
        self.mappe._ajouterOccupationInitiale(24,Occupation.COLONIE,1)
        self.mappe._ajouterOccupationInitiale(24,Occupation.VILLE,1)
        self.mappe._ajouterOccupationInitiale(16,Occupation.COLONIE,2)
        ressources = self.mappe._distribuerRessources(3)
        self.assertEqual(ressources[(1,Ressource.BOIS)],2)
        self.assertEqual(ressources[(2,Ressource.BOIS)],1)
        print "Test - Distribuer ressources 5"


    def testDistribuerRessources6(self):
        typesTerritoires = ['champ','champ','champ','champ','foret','foret','foret','foret','pre','pre','pre','pre','montagne','montagne','montagne','colline','colline','colline','desert']
        self.mappe = Mappe(typesTerritoires)
        self.mappe._ajouterOccupationInitiale(19,Occupation.COLONIE,1)
        self.mappe._ajouterOccupationInitiale(19,Occupation.VILLE,1)
        self.mappe._ajouterOccupationInitiale(37,Occupation.COLONIE,2)
        ressources = self.mappe._distribuerRessources(8)
        self.assertEqual(ressources[(1,Ressource.BLE)],2)
        self.assertEqual(ressources[(2,Ressource.LAINE)],1)
        print "Test - Distribuer ressources 6"

    def testDistribuerRessources7(self):
        typesTerritoires = ['champ','champ','champ','champ','foret','foret','foret','foret','pre','pre','pre','pre','montagne','montagne','montagne','colline','colline','colline','desert']
        self.mappe = Mappe(typesTerritoires)
        self.mappe._ajouterOccupationInitiale(19,Occupation.COLONIE,1)
        self.mappe._ajouterOccupationInitiale(19,Occupation.VILLE,1)
        self.mappe._ajouterOccupationInitiale(26,Occupation.COLONIE,1)
        self.mappe._ajouterOccupationInitiale(37,Occupation.COLONIE,2)
        ressources = self.mappe._distribuerRessources(8)
        self.assertEqual(ressources[(1,Ressource.BLE)],2)
        self.assertEqual(ressources[(1,Ressource.LAINE)],1)
        self.assertEqual(ressources[(2,Ressource.LAINE)],1)
        print "Test - Distribuer ressources 7"



       

unittest.main()


