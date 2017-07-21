-- --------------------------------------------------------
-- Host:                         127.0.0.1
-- Server version:               5.6.28-log - MySQL Community Server (GPL)
-- Server OS:                    Win64
-- HeidiSQL Version:             9.3.0.4984
-- --------------------------------------------------------

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8mb4 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;

-- Dumping structure for table maelstrom_datacentre.character
CREATE TABLE IF NOT EXISTS `character` (
  `id` bigint(20) unsigned NOT NULL DEFAULT '0',
  `serviceAccountId` int(10) unsigned NOT NULL DEFAULT '0',
  `actorId` int(10) unsigned NOT NULL DEFAULT '0',
  `realmId` smallint(5) unsigned NOT NULL DEFAULT '0',
  `name` varchar(32) NOT NULL DEFAULT '',
  `birthMonth` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `birthDay` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `guardian` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `voice` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `classJobId` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `flags` int(10) unsigned NOT NULL DEFAULT '0',
  `sessionSource` varchar(32) NOT NULL DEFAULT '',
  `sessionExpiration` int(10) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  UNIQUE KEY `name` (`actorId`,`name`),
  KEY `__FK_character_realmId__realm_list_id` (`realmId`),
  CONSTRAINT `__FK_character_realmId__realm_list_id` FOREIGN KEY (`realmId`) REFERENCES `realm_list` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- Dumping data for table maelstrom_datacentre.character: ~1 rows (approximately)
/*!40000 ALTER TABLE `character` DISABLE KEYS */;
/*!40000 ALTER TABLE `character` ENABLE KEYS */;


-- Dumping structure for table maelstrom_datacentre.character_appearance
CREATE TABLE IF NOT EXISTS `character_appearance` (
  `id` bigint(20) unsigned NOT NULL DEFAULT '0',
  `race` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `sex` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `height` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `clan` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `bustSize` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `skinColour` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `tailShape` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `tailLength` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `hairStyle` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `hairColour` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `hairColourHighlights` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `face` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `jaw` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `eye` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `eyeColour` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `eyeColourOdd` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `eyebrows` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `nose` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `mouth` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `lipColour` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `facialFeatures` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `tattooColour` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `facePaint` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `facePaintColour` tinyint(3) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  CONSTRAINT `__FK_character_appearance__character_id` FOREIGN KEY (`id`) REFERENCES `character` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- Dumping data for table maelstrom_datacentre.character_appearance: ~1 rows (approximately)
/*!40000 ALTER TABLE `character_appearance` DISABLE KEYS */;
/*!40000 ALTER TABLE `character_appearance` ENABLE KEYS */;


-- Dumping structure for table maelstrom_datacentre.character_class
CREATE TABLE IF NOT EXISTS `character_class` (
  `id` bigint(20) unsigned NOT NULL DEFAULT '0',
  `classId` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `level` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `xp` int(10) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`,`classId`),
  CONSTRAINT `__FK_character_class_id__character_id` FOREIGN KEY (`id`) REFERENCES `character` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- Dumping data for table maelstrom_datacentre.character_class: ~1 rows (approximately)
/*!40000 ALTER TABLE `character_class` DISABLE KEYS */;
/*!40000 ALTER TABLE `character_class` ENABLE KEYS */;


-- Dumping structure for table maelstrom_datacentre.character_item
CREATE TABLE IF NOT EXISTS `character_item` (
  `id` bigint(20) unsigned NOT NULL DEFAULT '0',
  `guid` bigint(20) unsigned NOT NULL DEFAULT '0',
  `itemId` int(10) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  CONSTRAINT `__FK_character_item_id__character_id` FOREIGN KEY (`id`) REFERENCES `character` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- Dumping data for table maelstrom_datacentre.character_item: ~0 rows (approximately)
/*!40000 ALTER TABLE `character_item` DISABLE KEYS */;
/*!40000 ALTER TABLE `character_item` ENABLE KEYS */;


-- Dumping structure for table maelstrom_datacentre.character_position
CREATE TABLE IF NOT EXISTS `character_position` (
  `id` bigint(20) unsigned NOT NULL DEFAULT '0',
  `territoryId` smallint(5) unsigned NOT NULL DEFAULT '0',
  `x` float NOT NULL DEFAULT '0',
  `y` float NOT NULL DEFAULT '0',
  `z` float NOT NULL DEFAULT '0',
  `o` float NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  CONSTRAINT `__FK_character_position_id__character_id` FOREIGN KEY (`id`) REFERENCES `character` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- Dumping data for table maelstrom_datacentre.character_position: ~1 rows (approximately)
/*!40000 ALTER TABLE `character_position` DISABLE KEYS */;
/*!40000 ALTER TABLE `character_position` ENABLE KEYS */;


-- Dumping structure for table maelstrom_datacentre.realm_list
CREATE TABLE IF NOT EXISTS `realm_list` (
  `id` smallint(5) unsigned NOT NULL DEFAULT '0',
  `name` varchar(32) NOT NULL DEFAULT '',
  `flags` int(10) unsigned NOT NULL DEFAULT '0',
  `host` varchar(48) NOT NULL DEFAULT '',
  `internalHost` varchar(48) NOT NULL DEFAULT '',
  `port` int(10) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- Dumping data for table maelstrom_datacentre.realm_list: ~2 rows (approximately)
/*!40000 ALTER TABLE `realm_list` DISABLE KEYS */;
INSERT INTO `realm_list` (`id`, `name`, `flags`, `host`, `internalHost`, `port`) VALUES
	(1000, 'Maelstrom', 0, '127.0.0.1', '127.0.0.1', 55023);
/*!40000 ALTER TABLE `realm_list` ENABLE KEYS */;
/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IF(@OLD_FOREIGN_KEY_CHECKS IS NULL, 1, @OLD_FOREIGN_KEY_CHECKS) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
