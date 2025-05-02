-- 1. Créer un utilisateur (remplacez 'garneau' et 'strong_password')
CREATE USER 'garneau'@'localhost' IDENTIFIED BY 'qwerty123';

-- 2. Donner les privilèges LIMITÉS à un SEUL schéma (remplacez 'new_user' par votre nouveau nom de user)
GRANT SELECT, INSERT, UPDATE, DELETE ON `app-robot-data`.* TO 'garneau'@'localhost';

-- 3. Appliquer les changements
FLUSH PRIVILEGES;
