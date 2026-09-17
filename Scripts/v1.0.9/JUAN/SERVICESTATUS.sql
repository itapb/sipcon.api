CREATE TABLE SERVICESTATUS
(	
	ID INT IDENTITY (1,1),
	VNAME VARCHAR(150),
	BACTIVE BIT,
	DCREATED DATETIME
)

INSERT INTO SERVICESTATUS(VNAME,BACTIVE,DCREATED)
VALUES ('En Diagnóstico',1,GETDATE()),
       ('Por Asistencia Técnica',1,GETDATE()),
	   ('Por Repuesto',1,GETDATE()),
	   ('En Reparación',1,GETDATE()),
	   ('En Prueba',1,GETDATE()),
	   ('Por Aprobación de Presupuesto',1,GETDATE()),
	   ('En Espera de Técnico',1,GETDATE())

ALTER TABLE MAINTENANCE 
ADD ISERVICESTATUS INT 


