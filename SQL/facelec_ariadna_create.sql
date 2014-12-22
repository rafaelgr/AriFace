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

/*Table structure for table `administrador` */

DROP TABLE IF EXISTS `administrador`;

CREATE TABLE `administrador` (
  `administrador_id` int(11) NOT NULL AUTO_INCREMENT,
  `login` varchar(255) NOT NULL,
  `password` varchar(255) NOT NULL,
  `nombre` varchar(255) NOT NULL,
  `email` varchar(255) DEFAULT NULL,
  `certsn` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`administrador_id`),
  UNIQUE KEY `idx_login` (`login`)
) ENGINE=InnoDB AUTO_INCREMENT=470 DEFAULT CHARSET=utf8;

/*Table structure for table `cliente` */

DROP TABLE IF EXISTS `cliente`;

CREATE TABLE `cliente` (
  `nombre` varchar(255) DEFAULT NULL,
  `login` varchar(255) DEFAULT NULL,
  `i_d` int(11) NOT NULL AUTO_INCREMENT,
  `f_nueva` tinyint(4) DEFAULT NULL,
  `id_empresa` int(11) DEFAULT NULL,
  `email` varchar(255) DEFAULT NULL,
  `contrasena` varchar(255) DEFAULT NULL,
  `codclien_ariges` int(11) DEFAULT NULL,
  `cif` varchar(255) DEFAULT NULL,
  `codclien__arigasol` int(11) NOT NULL,
  `codclien_ariagro` int(11) NOT NULL,
  `tiene_factura_p_r_o_v` tinyint(4) NOT NULL,
  `cod_socio_ariagro` int(11) NOT NULL,
  `cod_clien_ariges2` int(11) NOT NULL,
  `cod_teletaxi` int(11) NOT NULL,
  `cod_gessoc` int(11) DEFAULT NULL,
  `cod_socio_aritaxi` int(11) DEFAULT NULL,
  `organoGestorCodigo` varchar(255) DEFAULT NULL,
  `unidadTramitadoraCodigo` varchar(255) DEFAULT NULL,
  `oficinaContableCodigo` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`i_d`),
  KEY `idx_cliente_id_empresa` (`id_empresa`),
  CONSTRAINT `ref_cliente_empresa` FOREIGN KEY (`id_empresa`) REFERENCES `empresa` (`id_empresa`)
) ENGINE=InnoDB AUTO_INCREMENT=1484 DEFAULT CHARSET=latin1;

/*Table structure for table `departamento` */

DROP TABLE IF EXISTS `departamento`;

CREATE TABLE `departamento` (
  `departamento_id` int(11) NOT NULL AUTO_INCREMENT,
  `codclien` int(11) DEFAULT NULL,
  `coddirec` int(11) DEFAULT NULL,
  `nombre` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`departamento_id`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8;

/*Table structure for table `empresa` */

DROP TABLE IF EXISTS `empresa`;

CREATE TABLE `empresa` (
  `nombre` varchar(255) DEFAULT NULL,
  `id_empresa` int(11) NOT NULL AUTO_INCREMENT,
  `cif` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`id_empresa`)
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=latin1;

/*Table structure for table `estado` */

DROP TABLE IF EXISTS `estado`;

CREATE TABLE `estado` (
  `codigo` varchar(255) NOT NULL,
  `nombre` varchar(255) DEFAULT NULL,
  `descripcion` text,
  PRIMARY KEY (`codigo`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/*Table structure for table `factura` */

DROP TABLE IF EXISTS `factura`;

CREATE TABLE `factura` (
  `ttal` decimal(20,10) DEFAULT NULL,
  `sistema_id` int(11) NOT NULL,
  `num_serie` varchar(255) DEFAULT NULL,
  `num_factura` int(11) DEFAULT NULL,
  `nueva` tinyint(4) DEFAULT NULL,
  `id_factura` int(11) NOT NULL AUTO_INCREMENT,
  `fecha` datetime DEFAULT NULL,
  `cuota_total` decimal(20,10) DEFAULT NULL,
  `coddirec_ariges` int(11) DEFAULT NULL,
  `id_cliente` int(11) DEFAULT NULL,
  `base_total` decimal(20,10) DEFAULT NULL,
  `es_fra_cliente` bit(1) NOT NULL,
  `imp_retencion` decimal(20,10) NOT NULL,
  `letra_id_fra_prove` varchar(255) DEFAULT NULL,
  `imp_gastos_a_fo` decimal(20,10) NOT NULL,
  `v_codtipom1` varchar(255) DEFAULT NULL,
  `registroFace` varchar(255) DEFAULT NULL,
  `motivoFace` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`id_factura`),
  KEY `idx_factura_id_cliente` (`id_cliente`),
  KEY `idx_factura_sistema_id` (`sistema_id`),
  CONSTRAINT `FK_factura` FOREIGN KEY (`id_cliente`) REFERENCES `cliente` (`i_d`),
  CONSTRAINT `ref_factura_sistema` FOREIGN KEY (`sistema_id`) REFERENCES `sistema` (`sistema_id`)
) ENGINE=InnoDB AUTO_INCREMENT=1656 DEFAULT CHARSET=latin1;

/*Table structure for table `firma` */

DROP TABLE IF EXISTS `firma`;

CREATE TABLE `firma` (
  `tsa_user` varchar(255) DEFAULT NULL,
  `tsa_url` varchar(255) DEFAULT NULL,
  `tsa_pass` varchar(255) DEFAULT NULL,
  `posicion_y0` int(11) DEFAULT NULL,
  `posicion_y_y` int(11) DEFAULT NULL,
  `posicion_x0` int(11) DEFAULT NULL,
  `posicion_x_x` int(11) DEFAULT NULL,
  `motivo` varchar(255) DEFAULT NULL,
  `lugar` varchar(255) DEFAULT NULL,
  `logo_path` varchar(255) DEFAULT NULL,
  `id_firma` int(11) NOT NULL AUTO_INCREMENT,
  `certificado_path` varchar(255) DEFAULT NULL,
  `certificado_pass` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`id_firma`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

/*Table structure for table `nifbase` */

DROP TABLE IF EXISTS `nifbase`;

CREATE TABLE `nifbase` (
  `nif` varchar(255) NOT NULL,
  `nombre` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`nif`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/*Table structure for table `plantilla` */

DROP TABLE IF EXISTS `plantilla`;

CREATE TABLE `plantilla` (
  `contenido` text,
  `nombre` varchar(255) DEFAULT NULL,
  `plantilla_id` int(11) NOT NULL,
  `observaciones` text,
  PRIMARY KEY (`plantilla_id`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;

/*Table structure for table `repositorio` */

DROP TABLE IF EXISTS `repositorio`;

CREATE TABLE `repositorio` (
  `path` varchar(255) DEFAULT NULL,
  `i_d` int(11) NOT NULL AUTO_INCREMENT,
  PRIMARY KEY (`i_d`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=latin1;

/*Table structure for table `sistema` */

DROP TABLE IF EXISTS `sistema`;

CREATE TABLE `sistema` (
  `sistema_id` int(11) NOT NULL AUTO_INCREMENT,
  `nombre` varchar(255) DEFAULT NULL,
  `base_datos` varchar(255) DEFAULT NULL,
  `descripcion` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`sistema_id`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=latin1;

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

/*Table structure for table `unidad` */

DROP TABLE IF EXISTS `unidad`;

CREATE TABLE `unidad` (
  `organoGestorCodigo` varchar(255) NOT NULL,
  `unidadTramitadoraCodigo` varchar(255) NOT NULL,
  `oficinaContableCodigo` varchar(255) NOT NULL,
  `organoGestorNombre` varchar(255) NOT NULL,
  `unidadTramitadoraNombre` varchar(255) NOT NULL,
  `oficinaContableNombre` varchar(255) NOT NULL,
  PRIMARY KEY (`organoGestorCodigo`,`unidadTramitadoraCodigo`,`oficinaContableCodigo`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/*Table structure for table `usuario` */

DROP TABLE IF EXISTS `usuario`;

CREATE TABLE `usuario` (
  `usuario_id` int(11) NOT NULL AUTO_INCREMENT,
  `nombre` varchar(255) DEFAULT NULL,
  `login` varchar(255) DEFAULT NULL,
  `password` varchar(255) DEFAULT NULL,
  `email` varchar(255) DEFAULT NULL,
  `nif` varchar(255) DEFAULT NULL,
  `cliente_id` int(11) DEFAULT NULL,
  `departamento_id` int(11) DEFAULT NULL,
  PRIMARY KEY (`usuario_id`)
) ENGINE=InnoDB AUTO_INCREMENT=18 DEFAULT CHARSET=utf8;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;
