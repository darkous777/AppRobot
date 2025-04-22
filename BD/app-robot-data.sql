-- MySQL dump 10.13  Distrib 8.0.38, for Win64 (x86_64)
--
-- Host: localhost    Database: app-robot-data
-- ------------------------------------------------------
-- Server version	8.0.39

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

CREATE DATABASE IF NOT EXISTS `app-robot-data`;
USE `app-robot-data`;


--
-- Table structure for table `fonctionnalite`
--

DROP TABLE IF EXISTS `fonctionnalite`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `fonctionnalite` (
  `id` int NOT NULL AUTO_INCREMENT,
  `nom` varchar(75) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=12 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `fonctionnalite`
--

LOCK TABLES `fonctionnalite` WRITE;
/*!40000 ALTER TABLE `fonctionnalite` DISABLE KEYS */;
INSERT INTO `fonctionnalite` VALUES (1,'Avancer'),(2,'Avancer/Droite'),(3,'Avancer/Gauche'),(4,'Reculer'),(5,'Reculer/Droite'),(6,'Reculer/Gauche'),(7,'Rotation/Droite'),(8,'Rotation/Gauche'),(9,'Musique'),(10,'Camera'),(11,'Suivre/Ligne');
/*!40000 ALTER TABLE `fonctionnalite` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `gestion_fonctionnalite_user`
--

DROP TABLE IF EXISTS `gestion_fonctionnalite_user`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `gestion_fonctionnalite_user` (
  `UserId` int NOT NULL,
  `FonctionnaliteId` int NOT NULL,
  `Acces` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`UserId`,`FonctionnaliteId`),
  KEY `FonctionnaliteId` (`FonctionnaliteId`),
  CONSTRAINT `gestion_fonctionnalite_user_ibfk_1` FOREIGN KEY (`UserId`) REFERENCES `user` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `gestion_fonctionnalite_user_ibfk_2` FOREIGN KEY (`FonctionnaliteId`) REFERENCES `fonctionnalite` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `gestion_fonctionnalite_user`
--

LOCK TABLES `gestion_fonctionnalite_user` WRITE;
/*!40000 ALTER TABLE `gestion_fonctionnalite_user` DISABLE KEYS */;
INSERT INTO `gestion_fonctionnalite_user` VALUES (1,1,0),(1,2,1),(1,3,1),(1,4,1),(1,5,1),(1,6,1),(1,7,1),(1,8,1),(1,9,1),(1,10,1),(1,11,1),(2,1,0),(2,2,1),(2,3,1),(2,4,1),(2,5,1),(2,6,1),(2,7,1),(2,8,1),(2,9,1),(2,10,1),(2,11,1),(3,1,0),(3,2,0),(3,3,0),(3,4,0),(3,5,0),(3,6,0),(3,7,0),(3,8,0),(3,9,0),(3,10,0),(3,11,0),(5,1,1),(5,2,1),(5,3,1),(5,4,1),(5,5,1),(5,6,1),(5,7,1),(5,8,1),(5,9,1),(5,10,1),(5,11,1);
/*!40000 ALTER TABLE `gestion_fonctionnalite_user` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `user`
--

DROP TABLE IF EXISTS `user`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `user` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Username` varchar(45) NOT NULL,
  `Password` varchar(256) NOT NULL,
  `Age_date` date NOT NULL,
  `TypeUser` enum('User','Moderator','Admin') NOT NULL,
  `Image` varchar(255) NOT NULL,
  `Acces` tinyint NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `user`
--

LOCK TABLES `user` WRITE;
/*!40000 ALTER TABLE `user` DISABLE KEYS */;
INSERT INTO `user` VALUES (1,'Admin00000','1f9b53294c3a031120258552f415e471e865b344c9d820ef8c81bb7a19a04473','2000-01-01','Admin','sourire-user-icon.jpg',1),(2,'User0000','c3d545d8e89ad3314559ee2309d9c734bdc03ad83573ad16b83a18699abc3ed4','2001-01-01','User','sourire-user-icon.jpg',1),(3,'Moderator000','1f9b53294c3a031120258552f415e471e865b344c9d820ef8c81bb7a19a04473','1965-01-01','Moderator','singe-user-icon.jpg',1),(5,'Moderator001','c3d545d8e89ad3314559ee2309d9c734bdc03ad83573ad16b83a18699abc3ed4','1988-07-18','Moderator','user-icon.png',1);
/*!40000 ALTER TABLE `user` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2025-04-10 21:50:12
