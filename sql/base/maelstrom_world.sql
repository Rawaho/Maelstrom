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

-- Dumping structure for table maelstrom_world.character_spawn
CREATE TABLE IF NOT EXISTS `character_spawn` (
  `cityStateId` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `territoryId` smallint(5) unsigned NOT NULL DEFAULT '0',
  `x` float NOT NULL DEFAULT '0',
  `y` float NOT NULL DEFAULT '0',
  `z` float NOT NULL DEFAULT '0',
  `o` float NOT NULL DEFAULT '0',
  PRIMARY KEY (`cityStateId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- Dumping data for table maelstrom_world.character_spawn: ~3 rows (approximately)
/*!40000 ALTER TABLE `character_spawn` DISABLE KEYS */;
INSERT INTO `character_spawn` (`cityStateId`, `territoryId`, `x`, `y`, `z`, `o`) VALUES
	(1, 181, -53.7963, 20.2096, 0.000325262, 0),
	(2, 183, 121.563, -12.6964, 144.683, 0),
	(3, 182, 42.7539, 4, -149.24, 0);
/*!40000 ALTER TABLE `character_spawn` ENABLE KEYS */;


-- Dumping structure for table maelstrom_world.teleport_location
CREATE TABLE IF NOT EXISTS `teleport_location` (
  `name` varchar(64) NOT NULL DEFAULT '',
  `territoryId` smallint(5) unsigned NOT NULL DEFAULT '0',
  `x` float NOT NULL DEFAULT '0',
  `y` float NOT NULL DEFAULT '0',
  `z` float NOT NULL DEFAULT '0',
  `o` float NOT NULL DEFAULT '0',
  PRIMARY KEY (`name`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- Dumping data for table maelstrom_world.teleport_location: ~1 rows (approximately)
/*!40000 ALTER TABLE `teleport_location` DISABLE KEYS */;
INSERT INTO `teleport_location` (`name`, `territoryId`, `x`, `y`, `z`, `o`) VALUES
	('Reunion', 723, 555.907, -19.5056, 353.872, 3.12671);
/*!40000 ALTER TABLE `teleport_location` ENABLE KEYS */;
/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IF(@OLD_FOREIGN_KEY_CHECKS IS NULL, 1, @OLD_FOREIGN_KEY_CHECKS) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
