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

-- Dumping structure for table maelstrom_authentication.account
CREATE TABLE IF NOT EXISTS `account` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `name` varchar(32) NOT NULL DEFAULT '',
  `password` varchar(64) NOT NULL DEFAULT '',
  `salt` varchar(64) NOT NULL DEFAULT '',
  `sessionId` varchar(64) NOT NULL DEFAULT '',
  PRIMARY KEY (`id`),
  UNIQUE KEY `name` (`name`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- Data exporting was unselected.


-- Dumping structure for table maelstrom_authentication.account_service
CREATE TABLE IF NOT EXISTS `account_service` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `accountId` int(10) unsigned NOT NULL DEFAULT '0',
  `name` varchar(68) NOT NULL DEFAULT '',
  `expansion` tinyint(3) unsigned NOT NULL DEFAULT '2',
  `realmCharacterLimit` smallint(5) unsigned NOT NULL DEFAULT '8',
  `accountCharacterLimit` smallint(5) unsigned NOT NULL DEFAULT '40',
  `security` smallint(5) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `__FK_account_service_accountId__account_id` (`accountId`),
  CONSTRAINT `__FK_account_service_accountId__account_id` FOREIGN KEY (`accountId`) REFERENCES `account` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- Data exporting was unselected.


-- Dumping structure for table maelstrom_authentication.version
CREATE TABLE IF NOT EXISTS `version` (
  `file` varchar(32) NOT NULL DEFAULT '',
  `version` int(10) unsigned NOT NULL DEFAULT '0',
  `digest` varchar(50) NOT NULL DEFAULT '',
  PRIMARY KEY (`file`,`version`,`digest`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- Data exporting was unselected.
/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IF(@OLD_FOREIGN_KEY_CHECKS IS NULL, 1, @OLD_FOREIGN_KEY_CHECKS) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
