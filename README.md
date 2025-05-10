# 420-08C-FX PROJET D'APPROFONDISSEMENT EN PROGRAMMATION - Vincent Rompré et Mohammed Amine Boumediene Blal

# AppRobot!


# Nature et objectif du projet (type de projet, clientèle cible, etc.)
* Le projet AppRobot consiste à créer un petit robot mobile, surnommé Vezou, contrôlé à distance par une interface WPF en C#. Le "cerveau" de Vezou est en fait un Raspberry Pi sous un système d'exploitation Ubuntu et ses pièces proviennent du kit Picar-X de la compagnie SunFounder. Il est capable de se déplacer, détecter un obstacle devant lui et s'arrêter automatiquement, suivre un parcour de ruban noir placé au sol, jouer de la musique, diffuser une caméra en direct et réagir à différentes commandes envoyées par TCP via un réseau local. L’objectif principal était d’explorer concrètement la communication entre un logiciel d'interface et un système embarqué, en combinant programmation réseau, contrôle moteur, et interaction avec des capteurs physiques. Le projet s’adresse à un public étudiant ou simplement à ceux qui s’intéressent à la robotique, et qui veulent mettre en place un jouet concret, simple à contrôler, mais qui possède assez d'option et de potentiel pour toucher à plusieurs aspects du code et du matériel en même temps. Le projet permet de plonger à la fois dans de la théorie, du virtuelle et du matériel, en les reliant de façon concrète. Il ouvre aussi la porte à une foule de possibilités, autant pour apprendre que pour pousser plus loin selon l’imagination de ceux qui le développent.

# Technologies utilisées :
* Python 3.12 – pour le code qui controle tout le robot (fichier sockets.py)

* C# avec WPF (.NET 8) – pour l'interface graphique de contrôle sur PC

* Socket TCP/IP – pour la communication entre l’interface WPF et le Raspberry Pi

* Pygame – pour la lecture de musique (via pygame.mixer) sur le robot

* Flask – pour diffuser un flux vidéo depuis la caméra du robot pour ensuite aller le récupérer et l'afficher sur l'interface de controle WPF

* OpenCV – utilisé indirectement pour la caméra (puisque picamera2 est incompatible avec Ubuntu)

* SunFounder Picar-X SDK – bibliothèque Python utilisée pour contrôler les moteurs, servos, capteurs et autre

* Ubuntu pour Raspberry Pi – système d’exploitation du robot (au lieu de Raspberry Pi OS pour compatibilité)

* GPIOZero + PiGPIO – pour l’accès aux broches GPIO via réseau, les broches sont les points d'accès physique aux modules du robot.


# Fonctionnalités servies par le projet :
* Contrôle directionnel complet du robot via des boutons (avancer, reculer, rotationner, tourner en diagonale dans tous les sens)

* Suivi automatisé de ligne noir placé par terre avec arrêt lorsqu’elle est perdue

* Diffusion d’un flux vidéo en direct à partir de la caméra embarquée

* Joue de la musique préalablement télécharger sur le robot

* Détection d’obstacle en temps réel avec évitement et envoi d’un message d’avertissement à l’interface

* Création, connexion, modification et suppression de comptes pour les utilisateurs

* Connexion au robot et retour de statut de communication actif

* Système de rôles (utilisateur, modérateur, administrateur) avec interface de gestion dédiée pour l'administrateur

* Interface d’administration pour :

    * Octroyer ou retirer le statut de modérateur

    * Bloquer, débloquer, supprimer des comptes utilisateurs ou modérateurs

    * Activer ou suspendre des fonctionnalités du robot à des utilisateurs à distance


# Degré de complétion :
* Tous les fonctionnalités initiales planifiées ont été complété et fonctionnent.


# Bogues persistants :
* Lenteur de chargement de la caméra dans l'interface parce que le serveur Flask prend un certain temps à s'initialisé

* Le module de niveau de gris repose sur la détection de contraste entre le noir et le blanc et il peut mal réagir sur des surfaces non uniformes ou trop foncées comme des planchers de bois franc ou de la céramique texturée. C’est pourquoi on recommande fortement de placer une ligne de ruban électrique noir sur une surface entièrement blanche, afin d’assurer un suivi fiable et stable.



# Possibles amélioration :
* La qualité de l'image et le niveaux de rafraichissment de l'image peuvent toujours être améliorer. La caméra embarquée fournie dans le kit n’offre pas une très haute définition ni une fluidité optimale, mais il serait tout à fait possible d’obtenir de meilleurs résultats en remplaçant le module par une caméra plus performante.


# Procédure d'installation client :

## A) - Procédure pour démarrer le robot :
* Étape 1 : Allumer l’alimentation principale
    Mettre le robot sous tension en basculant l’interrupteur principal vers la position de marche. Ce bouton se retrouve à l'avant sur le dessus du Robot Hat, la plaquette blanche de contrôle montée sur le Raspberry Pi.
    Une lumière verte devrait s’allumer sur le dessus du robot pour confirmer que le courant passe.

    Si aucun voyant ne s’allume, il est probable que la batterie soit déchargée. Dans ce cas :
    * Brancher le port USB-C situé sur le Robot Hat à une source d’alimentation (comme un chargeur mural ou un ordinateur).
    * Attendre environ 5 à 10 minutes pour laisser la batterie se recharger.
    * Recommencer l’opération d’allumage.

    Une fois l’indicateur vert allumé, patienter environ 2 à 5 minutes, le temps que le système du robot démarre complètement.

* Étape 2 : Connecter le robot au réseau (première utilisation seulement)
    Lors de la toute première utilisation, il est nécessaire de brancher un écran, un clavier et une souris au Raspberry Pi. Pour utiliser le fil HDMI classique de votre écran, il est nécessaire d'utiliser l'adapteur HDMI—micro-HDMI et de brancher le port micro-HDMI dans un des ports prévus à cette effet du RaspberryPie. Un clavier et une souris peuvent être brancher à l'arrière par port USB.

    Cela permet d'avoir une vue du système du robot et de le connecter à un réseau Wi-Fi.
    Une fois connecté :
    * Cocher l’option « Se connecter automatiquement à ce réseau » (ou équivalent) si le réseau est de confiance (exemple : le wifi de  votre maison).
    * Cela évitera d’avoir à refaire cette opération à chaque redémarrage.

* Étape 3 : Activer le haut-parleur
    Le haut-parleur est la petite boule noire placée au centre du robot, entre la caméra et les composants électroniques (le Raspberry Pi et le Robot Hat). Il possède un interrupteur à trois positions situé sur le dessus, qui agit à la fois comme bouton d’alimentation et de volume.

    Voici comment il fonctionne :
    * Position complètement à droite : haut-parleur éteint.
    * Position du milieu : haut-parleur allumé avec un volume moyen.
    * Position complètement à gauche : haut-parleur allumé avec le volume au maximum.

    Dès que le bouton est tourné vers le milieu ou vers la gauche, le haut-parleur s’allume. Le voyant lumineux à sa base passe alors de rouge à bleu-mauve, indiquant qu’il est prêt à diffuser du son.


## B) - Installer l'application à partir du fichier .msi

* Par la suite télécharger le .msi et installer l'application.

* Cliquer sur le fichier ayant l'extension ".exe" et l'application de l'interface devrait démarrer.

* Ensuite vous n'avez qu'à vous créer un compte, suivre le tutoriel et vous amusez!

# Procédure d'installation pour les développeurs :
## 1 - Cloner le projet dans l'endroit de votre choix

* Appuyez sur le bouton vert nommé "<> Code" sur GitHub.

* Copiez le lien HTTPS, ouvrez l’invite de commande sur votre ordinateur, placez-vous dans le dossier où vous souhaitez cloner le projet, puis entrez la commande suivante : <code>git clone https://github.com/darkous777/AppRobot.git</code>


## 2 - Créer les dossiers nécessaires au fonctionnement de l'application

* Créez le dossier suivant dans votre disque local <code>C:</code>

* <code>C:\AppRobot\images</code>

* (Veillez à bien créer le dossier AppRobot, puis le dossier images à l'intérieur.)


## 3 - Ouvrir le projet dans un IDE

* Téléchargez Visual Studio Community 2022 ici : https://visualstudio.microsoft.com/fr/downloads/

* Choisissez les paramètres par défaut, et cochez l’option "Développement .NET Desktop" lors de l'installation.

* Cela vous permettra d’ouvrir ce projet développé principalement en C# / WPF.


## 4 - Installer et configurer MySQL Workbench

* Téléchargez MySQL Workbench ici :

* https://dev.mysql.com/downloads/workbench/

* Créez un schéma nommé : app-robot-data

* Ouvrez le fichier script_creation_user.sql (dans le dossier BD du projet).

* Modifiez ce fichier avant de l’exécuter :

* - Remplacez new_user par le nom d’utilisateur MySQL de votre choix

* - Remplacez strong_password par un mot de passe sécurisé

* Exécutez le script dans MySQL Workbench

* Ouvrer et changer aussi le fichier appsettings.json dans le dictionnaire : ConnectionStrings:DefaultConnection. Pour que Uid= votre nouveau nom d'utilisateur et que Pwd= votre nouveau mot de passe.

## 5 - Mettre la base de donnée à jour avec le projet

* Dans le dossier BD du projet, importez le fichier app-robot-data.sql dans le schéma app-robot-data que vous avez créé précédemment via MySQL Workbench.

## 6 - Préparation avant la connexion avec le robot

* Avant de lancer le projet, il y aura 2 choses à prendre en compte, il faudra rentrer dans votre projet cloner dans le chemin suivant :

* <code>\AppRobot\AppRobot\RobotApp\Ressources\Videos</code>

* Et vous devrez dézipper le fichier nommé : videoExplicative.7z .

* Par la suite pour tester le exe vous aurez le setup.exe à votre disposition dans le fichier Setup ou sinon vous pourrez dézipper le fichier Setup.7z qui vous permettera d'utiliser le .msi

## 7 - Connexion au robot

* Assurez-vous d’être connecté au même réseau Wi-Fi que le robot.

# Compte d'utilisateurs et mots de passes déjà prédéfinies avec leur niveau d'autorisation :

* Utilisateur : Admin00000 | Mot de passe  : 12345QWERT* | Niveaux d'accès : Administrateur
