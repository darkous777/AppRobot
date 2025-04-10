-- 1. Créer un utilisateur (remplacez 'new_user' et 'strong_password')
CREATE USER 'new_user'@'localhost' IDENTIFIED BY 'strong_password';

-- 2. Donner les privilèges LIMITÉS à un SEUL schéma (remplacez 'new_user' par votre nouveau nom de user)
GRANT SELECT, INSERT, UPDATE, DELETE ON `app-robot-data`.* TO 'new_user'@'localhost';

-- 3. Appliquer les changements
FLUSH PRIVILEGES;