SELECT COUNT(*) AS NUMERO, MONTH(fecha) AS MES, YEAR(fecha) AS ANYO
FROM factura
GROUP BY 2,3;