/*!40101 SET NAMES utf8 */;

/*!40101 SET SQL_MODE=''*/;

/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;
USE `facelec_ariadna`;
DROP TABLE IF EXISTS `administrador`;

CREATE TABLE `administrador` (
  `administrador_id` INT(11) NOT NULL AUTO_INCREMENT,
  `login` VARCHAR(255) NOT NULL,
  `password` VARCHAR(255) NOT NULL,
  `nombre` VARCHAR(255) NOT NULL,
  `email` VARCHAR(255) DEFAULT NULL,
  `certsn` VARCHAR(255) DEFAULT NULL,
  PRIMARY KEY (`administrador_id`),
  UNIQUE KEY `idx_login` (`login`)
) ENGINE=INNODB AUTO_INCREMENT=470 DEFAULT CHARSET=utf8;

/*Data for the table `administrador` */

INSERT  INTO `administrador`(`administrador_id`,`login`,`password`,`nombre`,`email`,`certsn`) VALUES (1,'admin','admin','Administrador','adm@gmail.com','09C042D451F3F466');

/* Incluir las columnas de la tabla clientes */
ALTER TABLE `facelec_ariadna`.`cliente` ADD COLUMN `organoGestorCodigo` VARCHAR(255) NULL AFTER `cod_socio_aritaxi`, ADD COLUMN `unidadTramitadoraCodigo` VARCHAR(255) NULL AFTER `organoGestorCodigo`, ADD COLUMN `oficinaContableCodigo` VARCHAR(255) NULL AFTER `unidadTramitadoraCodigo`;

/* Incluir la columna para registro de factura. */
ALTER TABLE `facelec_ariadna`.`factura` ADD COLUMN `registroFace` VARCHAR(255) NULL AFTER `v_codtipom1`;

/* Crear la tabla de usuarios */
CREATE TABLE `facelec_ariadna`.`usuario`( `usuario_id` INT(11) NOT NULL, `nombre` VARCHAR(255), `login` VARCHAR(255), `password` VARCHAR(255), `email` VARCHAR(255), `nif` VARCHAR(255), `cliente_id` INT(11), `departamento_id` INT(11), PRIMARY KEY (`usuario_id`) );

/* Se me ha olvidado el autoinc */
ALTER TABLE `facelec_ariadna`.`usuario` CHANGE `usuario_id` `usuario_id` INT(11) NOT NULL AUTO_INCREMENT;

/* Creación de la tabla de departamentos */
CREATE TABLE `facelec_ariadna`.`departamento`( `departamento_id` INT(11) NOT NULL AUTO_INCREMENT, `codclien` INT(11), `coddirec` INT(11), `nombre` VARCHAR(255), PRIMARY KEY (`departamento_id`) );

/* Creación de la base de nif */
CREATE TABLE `facelec_ariadna`.`nifbase`( `nif` VARCHAR(255) NOT NULL, `nombre` VARCHAR(255), PRIMARY KEY (`nif`) );

/* Agregar una columna para los motivos de rechazo en las facturas. */
ALTER TABLE `facelec_ariadna`.`factura` ADD COLUMN `motivoFace` VARCHAR(255) NULL AFTER `registroFace`;

/* Hay que crear un campo adicional para observaciones en la plantilla y sirva de guía */
ALTER TABLE `facelec_ariadna`.`plantilla` ADD COLUMN `observaciones` TEXT NULL AFTER `plantilla_id`

/*Table structure for table `estado` */

DROP TABLE IF EXISTS `estado`;

CREATE TABLE `estado` (
  `codigo` VARCHAR(255) NOT NULL,
  `nombre` VARCHAR(255) DEFAULT NULL,
  `descripcion` TEXT,
  PRIMARY KEY (`codigo`)
) ENGINE=INNODB DEFAULT CHARSET=utf8;

/*Table structure for table `unidad` */

DROP TABLE IF EXISTS `unidad`;

CREATE TABLE `unidad` (
  `organoGestorCodigo` VARCHAR(255) NOT NULL,
  `unidadTramitadoraCodigo` VARCHAR(255) NOT NULL,
  `oficinaContableCodigo` VARCHAR(255) NOT NULL,
  `organoGestorNombre` VARCHAR(255) NOT NULL,
  `unidadTramitadoraNombre` VARCHAR(255) NOT NULL,
  `oficinaContableNombre` VARCHAR(255) NOT NULL,
  PRIMARY KEY (`organoGestorCodigo`,`unidadTramitadoraCodigo`,`oficinaContableCodigo`)
) ENGINE=INNODB DEFAULT CHARSET=utf8;

/*Table structure for table `nifbase` */

DROP TABLE IF EXISTS `nifbase`;

CREATE TABLE `nifbase` (
  `nif` VARCHAR(255) NOT NULL,
  `nombre` VARCHAR(255) DEFAULT NULL,
  PRIMARY KEY (`nif`)
) ENGINE=INNODB DEFAULT CHARSET=utf8;

INSERT INTO nifbase
SELECT DISTINCT cif AS nif, nombre FROM cliente GROUP BY 1;


