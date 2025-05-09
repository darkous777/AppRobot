# Nature et objectif du projet (type de projet, clientèle cible, etc.)

* Le projet AppRobot consiste à créer un petit robot mobile, surnommé Vezou, contrôlé à distance par une interface WPF en C#. Le "cerveau" de Vezou est en fait un Raspberry Pi sous un système d'exploitation Ubuntu et ses pièces proviennent du kit Picar-X de la compagnie SunFounder. Il est capable de se déplacer, détecter un obstacle devant lui et s'arrêter automatiquement, suivre un parcour de ruban noir placé au sol, jouer de la musique, diffuser une caméra en direct et réagir à différentes commandes envoyées par TCP via un réseau local. L’objectif principal était d’explorer concrètement la communication entre un logiciel d'interface et un système embarqué, en combinant programmation réseau, contrôle moteur, et interaction avec des capteurs physiques. Le projet s’adresse à un public étudiant ou simplement à ceux qui s’intéressent à la robotique, et qui veulent mettre en place un jouet concret, simple à contrôler, mais qui possède assez d'option et de potentiel pour toucher à plusieurs aspects du code et du matériel en même temps. Le projet permet de plonger à la fois dans de la théorie, du virtuelle et du matériel, en les reliant de façon concrète. Il ouvre aussi la porte à une foule de possibilités, autant pour apprendre que pour pousser plus loin selon l’imagination de ceux qui le développent.


# Technologies utilisées :
*
*
*
*

# Fonctionnalités servies par le projet :
*
*
*
*
*
*
*


# Degré de complétion :

# Bogues persistants :

# Possibles amélioration :

# Procédure d'installation client



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

## 6 - Connexion au robot

* Assurez-vous d’être connecté au même réseau Wi-Fi que le robot.

# Compte d'utilisateurs et mots de passes déjà prédéfinies avec leur niveau d'autorisation :
* Utilisateur : Admin00000 | Mot de passe  : 12345QWERT* | Niveaux d'accès : Administrateur
