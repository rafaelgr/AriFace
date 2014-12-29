SELECT 
DISTINCT YEAR(f.fecha)
FROM factura AS f
LEFT JOIN sistema AS s ON s.sistema_id = f.sistema_id
LEFT JOIN cliente AS c ON c.i_d = f.id_cliente
LEFT JOIN departamento AS d ON d.codclien = c.codclien_ariges AND d.coddirec = f.coddirec_ariges