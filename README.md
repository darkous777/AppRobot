# 1 - Cloner le projet dans l'endroit de votre choix

## Appuyer sur le button vert nommer "<> Code". Copier le lien HTTPS et ouvrir l'invite de commande votre ordinateur. Trouver l'endroit dont vous voudriez cloner le projet et entrer la commande suivant <code>git clone https://github.com/darkous777/AppRobot.git<code>.

# 2 - Créer les fichiers suivant pour le fonctionnement du programme de l'application

## <code>C:\AppRobot\images<code> , créer le dossier AppRobot dans votre disque dur et à l'intérieur créer le dossier images.

# 3 - Pour accéder au code source du project, vous aurez besoind'un IDE

## Télécharger Visual Studio Community 2022 sur ce lien : https://visualstudio.microsoft.com/fr/downloads/

## Choisisser les paramètres par default et il important de cocher sur la case : Développement .NET Desktop, lors de l'installation. Cela vous permettera d'ouvrir le project qui est fait majoritairement en C#/WPF

# 4 - Télécharger MYSQL :: WorkBench sur le lien suivant : https://dev.mysql.com/downloads/workbench/

## Dans votre WorkBench Créer d'abord un Schéma nommer : app-robot-data, par la suite:

## Choisisser les parametres par défault et importer le script(dans WorkBench) situer dans le fichier nommer BD dans votre projet cloner : script_creation_user.sql . Changer le script au endroit de new_user(votre nom d'utilisateur) et strong_password(votre mot de passe) FAITES CECI AVANT DE PROCÉDER AVEC L'IMPORT DU FICHIER.

## Changer aussi le fichier appsettings.json dans le dictionnaire : ConnectionStrings:DefaultConnection. Pour que Uid= votre nouveau nom d'utilisateur et que Pwd= votre nouveau mot de passe.

# 5 - Mettre la base de donnée à jour avec le projet

## Dans votre fichier cloner, dirigez-vous dans le fichier nommez : BD. Par la suite importer le fichier nommer : app-robot-data.sql, et mettez le dans le schéma app-robot-data précédament créer.

# 6 - Maintenant il ne vous reste plus qu'à vous connecter au même réseaux WIFI que le robot et voila vous pouvez maintenant tester le robot à 100%.
