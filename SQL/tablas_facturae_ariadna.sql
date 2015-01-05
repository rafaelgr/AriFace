/*
SQLyog Community v12.03 (64 bit)
MySQL - 5.6.16 : Database - facelec_ariadna
*********************************************************************
*/

/*!40101 SET NAMES utf8 */;

/*!40101 SET SQL_MODE=''*/;

/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;
USE `facelec_ariadna`;

/*Table structure for table `plantilla` */

DROP TABLE IF EXISTS `plantilla`;

CREATE TABLE `plantilla` (
  `contenido` text,
  `nombre` varchar(255) DEFAULT NULL,
  `plantilla_id` int(11) NOT NULL,
  `observaciones` text,
  PRIMARY KEY (`plantilla_id`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;

/*Data for the table `plantilla` */

insert  into `plantilla`(`contenido`,`nombre`,`plantilla_id`,`observaciones`) values ('<p><img alt=\"Teinsa S.L.\" src=\"http://www.teinsa.es/s/cc_images/cache_12097161.jpg?t=1360606162\" style=\"height:57px; width:210px\" /></p>\n\n<p>Estimado cliente, {0}</p>\n\n<p>Ya se encuentran disponibles las siguientes facturas:</p>\n\n<p>{1}</p>\n\n<p>Que le adjuntamos en este correo. Le recordamos que en nuestra web de facturaci&oacute;n electr&oacute;nica&nbsp;<a href=\"http://facturae.teinsa.es\">http://facturae.teinsa.es</a>&nbsp;siempre podr&aacute; consultar tanto estas como otras emitidas.</p>\n\n<p>Atentamente.</p>\n\n<p>Teinsa S.L.</p>\n','Envio email fact. electronica',1,'{0} Cliente </br>\r\n{1} Detalles de factura </br>\r\n');

/*Table structure for table `repositorio` */

DROP TABLE IF EXISTS `repositorio`;

CREATE TABLE `repositorio` (
  `path` varchar(255) DEFAULT NULL,
  `i_d` int(11) NOT NULL AUTO_INCREMENT,
  PRIMARY KEY (`i_d`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=latin1;

/*Data for the table `repositorio` */

insert  into `repositorio`(`path`,`i_d`) values ('C:\\Intercambio\\Repositorio\\',1);

/*Table structure for table `sistema` */

DROP TABLE IF EXISTS `sistema`;

CREATE TABLE `sistema` (
  `sistema_id` int(11) NOT NULL AUTO_INCREMENT,
  `nombre` varchar(255) DEFAULT NULL,
  `base_datos` varchar(255) DEFAULT NULL,
  `descripcion` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`sistema_id`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=latin1;

/*Data for the table `sistema` */

insert  into `sistema`(`sistema_id`,`nombre`,`base_datos`,`descripcion`) values (1,'AriGes','ariges1','ARIGES'),(2,'AriGasol','arigasol','ARIGASOL'),(3,'AriAgro','ariagro8','ARIAGRO'),(4,'AriGesDos','ariges6','TELEFONIA'),(5,'AriTaxi','aritaxi','TELETAXI'),(6,'AriGes','ariges4','ARIGESCON');

/*Table structure for table `superusuario` */

DROP TABLE IF EXISTS `superusuario`;

CREATE TABLE `superusuario` (
  `Login` varchar(255) DEFAULT NULL,
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Id_empresa` int(11) DEFAULT NULL,
  `Email` varchar(255) DEFAULT NULL,
  `Contrasena` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `idx_superusuario_Id_empresa` (`Id_empresa`),
  CONSTRAINT `ref_superusuario_empresa` FOREIGN KEY (`Id_empresa`) REFERENCES `empresa` (`id_empresa`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=latin1;

/*Data for the table `superusuario` */

insert  into `superusuario`(`Login`,`Id`,`Id_empresa`,`Email`,`Contrasena`) values ('admin',1,1,'rafa@myariadna.com','admin');

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;
