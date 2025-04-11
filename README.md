# 1 - Cloner le projet dans l'endroit de votre choix

## Appuyez sur le bouton vert nommé "<> Code" sur GitHub.

## Copiez le lien HTTPS, ouvrez l’invite de commande sur votre ordinateur, placez-vous dans le dossier où vous souhaitez cloner le projet, puis entrez la commande suivante :

## <code>git clone https://github.com/darkous777/AppRobot.git</code>


# 2 - Créer les dossiers nécessaires au fonctionnement de l'application

## Créez le dossier suivant dans votre disque local <code>C:</code> :

## <code>C:\AppRobot\images</code>

## (Veillez à bien créer le dossier AppRobot, puis le dossier images à l'intérieur.)


# 3 - Ouvrir le projet dans un IDE

## Téléchargez Visual Studio Community 2022 ici :

## https://visualstudio.microsoft.com/fr/downloads/

## Choisissez les paramètres par défaut, et cochez l’option "Développement .NET Desktop" lors de l'installation.

## Cela vous permettra d’ouvrir ce projet développé principalement en C# / WPF.


# 4 - Installer et configurer MySQL Workbench

## Téléchargez MySQL Workbench ici :

## https://dev.mysql.com/downloads/workbench/

## Ouvrez le fichier script_creation_user.sql (dans le dossier BD du projet).

## Modifiez ce fichier avant de l’exécuter :

### - Remplacez new_user par le nom d’utilisateur MySQL de votre choix

### - Remplacez strong_password par un mot de passe sécurisé

## Exécutez le script dans MySQL Workbench

## Ouvrer et changer aussi le fichier appsettings.json dans le dictionnaire : ConnectionStrings:DefaultConnection. Pour que Uid= votre nouveau nom d'utilisateur et que Pwd= votre nouveau mot de passe.

# 5 - Mettre la base de donnée à jour avec le projet

## Dans le dossier BD du projet, importez le fichier app-robot-data.sql dans le schéma app-robot-data que vous avez créé précédemment via MySQL Workbench.

# 6 - Connexion au robot

## Assurez-vous d’être connecté au même réseau Wi-Fi que le robot.

