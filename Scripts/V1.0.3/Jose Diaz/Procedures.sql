-- INSPECTIONS

CREATE PROCEDURE USP_GET_INSPECTIONS
	@IDAREA INT = NULL
	-- FALTA LA FASE
AS
/* '===============================================================          
  '   NOMBRE                : USP_GET_INSPECTIONS
  '   FECHA CREACIÓN        : 29/03/2026
  '   CREADO POR            : JOSÉ DÍAZ
  '   CREADO PARA           : OBTENER LISTADO DE INSPECCIONES
  '   FUNCIÓN               : SELECCIÓN GENERAL CON JOINS A USUARIO, VEHÍCULO, MODELO Y ÁREA
  '   VERSIÓN               : 1.0.0
  '===============================================================*/
BEGIN
    SET XACT_ABORT ON;               
    SET NOCOUNT ON;
    SET LOCK_TIMEOUT 180000;
	SELECT 
	    I.ID,
	    I.ICREATEDBY,
	    CONCAT(C.VFIRSTNAME, ' ', C.VLASTNAME) AS USERNAME,
	    I.DCREATED,
	    I.IDVEHICLE,
	    V.VPLATE AS PLATE,
	    V.VLOTE AS LOTE,
	    V.VVIN AS VIN,
	    M.VNAME AS MODEL,
	    I.IDAREA,
	    A.VNAME AS AREA,
	    I.DDATEINIT,
	    CONCAT(C_INIT.VFIRSTNAME, ' ', C_INIT.VLASTNAME) AS INITBYNAME,	 
	    I.DDATECLOSE,
	    CONCAT(C_CLOSED.VFIRSTNAME, ' ', C_CLOSED.VLASTNAME) AS CLOSEDBYNAME,
	   	I.DRECEPTION,	   
	    CONCAT(C_RECEP.VFIRSTNAME, ' ', C_RECEP.VLASTNAME) AS RECEPBYNAME,
	    CONCAT(C_TRANS.VFIRSTNAME, ' ', C_TRANS.VLASTNAME) AS TRANSPORTER_NAME,
	    CASE 
	        WHEN FASES.PENDIENTES > 0 THEN 0
	        ELSE 1
	    END AS ISCOMPLETED
	FROM INSPECTION I WITH (NOLOCK)
	    INNER JOIN CONTACT C WITH (NOLOCK) ON I.ICREATEDBY = C.ID
	    INNER JOIN VEHICLE V WITH (NOLOCK) ON I.IDVEHICLE = V.ID
	    INNER JOIN MODEL M WITH (NOLOCK) ON V.IDMODEL = M.ID
	    INNER JOIN AREA A WITH (NOLOCK) ON I.IDAREA = A.ID
	    -- Joins para obtener nombres de los contactos adicionales
	    LEFT JOIN CONTACT C_INIT WITH (NOLOCK) ON I.IDINITBY = C_INIT.ID
	    LEFT JOIN CONTACT C_CLOSED WITH (NOLOCK) ON I.IDCLOSEDBY = C_CLOSED.ID
	    LEFT JOIN CONTACT C_TRANS WITH (NOLOCK) ON I.IDTRANSPORTER = C_TRANS.ID
	    LEFT JOIN CONTACT C_RECEP WITH (NOLOCK) ON I.IDRECEPBY = C_RECEP.ID
	    LEFT JOIN (
	        SELECT 
	            IDINSPECTION, 
	            COUNT(CASE WHEN DCOMPLETED IS NULL THEN 1 END) AS PENDIENTES
	        FROM INSPECTIONFASE WITH (NOLOCK)
	        GROUP BY IDINSPECTION
	    ) FASES ON FASES.IDINSPECTION = I.ID
	WHERE I.BACTIVE = 1
    AND (@IDAREA IS NULL OR A.ID = @IDAREA)
END;

CREATE PROCEDURE USP_GET_INSPECTION_BY_ID
    @ID INT
AS
/* '===============================================================          
  '   NOMBRE                : USP_GET_INSPECTION_BY_ID
  '   FECHA CREACIÓN        : 29/03/2026
  '   CREADO POR            : JOSÉ DÍAZ
  '   CREADO PARA           : OBTENER UNA INSPECCIÓN POR ID
  '   FUNCIÓN               : SELECCIÓN FILTRADA POR LLAVE PRIMARIA
  '   VERSIÓN               : 1.0.0
  '===============================================================*/
BEGIN
    SET XACT_ABORT ON;               
    SET NOCOUNT ON;
    SET LOCK_TIMEOUT 180000;
SELECT 
	    I.ID,
	    I.ICREATEDBY,
	    CONCAT(C.VFIRSTNAME, ' ', C.VLASTNAME) AS USERNAME,
	    I.DCREATED,
	    I.IDVEHICLE,
	    V.VPLATE AS PLATE,
	    V.VLOTE AS LOTE,
	    V.VVIN AS VIN,
	    M.VNAME AS MODEL,
	    I.IDAREA,
	    A.VNAME AS AREA,
	    I.DDATEINIT,
	    CONCAT(C_INIT.VFIRSTNAME, ' ', C_INIT.VLASTNAME) AS INITBYNAME,	 
	    I.DDATECLOSE,
	    CONCAT(C_CLOSED.VFIRSTNAME, ' ', C_CLOSED.VLASTNAME) AS CLOSEDBYNAME,
	   	I.DRECEPTION,	   
	    CONCAT(C_RECEP.VFIRSTNAME, ' ', C_RECEP.VLASTNAME) AS RECEPBYNAME,
	    CONCAT(C_TRANS.VFIRSTNAME, ' ', C_TRANS.VLASTNAME) AS TRANSPORTER_NAME,
	    CASE 
	        WHEN FASES.PENDIENTES > 0 THEN 0
	        ELSE 1
	    END AS ISCOMPLETED
	FROM INSPECTION I WITH (NOLOCK)
	    INNER JOIN CONTACT C WITH (NOLOCK) ON I.ICREATEDBY = C.ID
	    INNER JOIN VEHICLE V WITH (NOLOCK) ON I.IDVEHICLE = V.ID
	    INNER JOIN MODEL M WITH (NOLOCK) ON V.IDMODEL = M.ID
	    INNER JOIN AREA A WITH (NOLOCK) ON I.IDAREA = A.ID
	    -- Joins para obtener nombres de los contactos adicionales
	    LEFT JOIN CONTACT C_INIT WITH (NOLOCK) ON I.IDINITBY = C_INIT.ID
	    LEFT JOIN CONTACT C_CLOSED WITH (NOLOCK) ON I.IDCLOSEDBY = C_CLOSED.ID
	    LEFT JOIN CONTACT C_TRANS WITH (NOLOCK) ON I.IDTRANSPORTER = C_TRANS.ID
	    LEFT JOIN CONTACT C_RECEP WITH (NOLOCK) ON I.IDRECEPBY = C_RECEP.ID
	    LEFT JOIN (
	        SELECT 
	            IDINSPECTION, 
	            COUNT(CASE WHEN DCOMPLETED IS NULL THEN 1 END) AS PENDIENTES
	        FROM INSPECTIONFASE WITH (NOLOCK)
	        GROUP BY IDINSPECTION
	    ) FASES ON FASES.IDINSPECTION = I.ID
    WHERE I.ID = @ID
END;

CREATE PROCEDURE USP_POST_INSPECTION
 @DATA NVARCHAR(MAX)
AS
/* '===============================================================          
 '   NOMBRE                : USP_POST_INSPECTION
 '   FECHA CREACIÓN        : 06/04/2026
 '   CREADO POR            : JOSÉ DÍAZ
 '   CREADO PARA           : INSERTAR O ACTUALIZAR INSPECCIONES (UPSERT)
 '   FUNCIÓN               : PROCESAR JSON PARA CARGA O EDICIÓN DE INSPECCIONES
 '   VERSIÓN               : 1.0.4
 '===============================================================*/
SET XACT_ABORT ON               
SET NOCOUNT ON
SET LOCK_TIMEOUT 180000
BEGIN
    BEGIN TRY
        BEGIN TRAN 
            -- 1. Declaración de tabla temporal incluyendo el ID y nuevos campos
            DECLARE @TDATA AS TABLE
            (
                ID INT,
                ICREATEDBY INT,
                IDVEHICLE INT,
                IDAREA INT,
                IDINITBY INT,
                IDCLOSEDBY INT,
                IDTRANSPORTER INT,
                IDRECEPBY INT,
                DDATEINIT DATETIME,
                DDATECLOSE DATETIME,
                DRECEPTION DATETIME
            );

            -- 2. Inserción de datos desde el JSON
            INSERT INTO @TDATA
            SELECT 
                ISNULL(M.Id, 0),
                M.CreatedBy,
                M.VehicleId,
                M.AreaId,
                M.InitBy,
                M.ClosedBy,
                M.TransporterId,
                M.RecepBy,
                M.DInit,
                M.DClose,
                M.DReception
            FROM OPENJSON(@DATA)
            WITH (
                Id INT,
                CreatedBy INT,
                VehicleId INT,
                AreaId INT,
                InitBy INT,
                ClosedBy INT,
                TransporterId INT,
                RecepBy INT,
                DInit DATETIME,
                DClose DATETIME,
                DReception DATETIME
            ) AS M;
		
            -- 3. Declaración de variables de control
            DECLARE @IID INT
            DECLARE @IINSERTED INT = 0
            DECLARE @IUPDATED INT = 0

            -------- 4. Lógica de ACTUALIZACIÓN (UPDATE) ----------------
            UPDATE I
            SET 
                I.ICREATEDBY = T.ICREATEDBY,
                I.IDVEHICLE = T.IDVEHICLE,
                I.IDAREA = T.IDAREA,
                I.IDINITBY = T.IDINITBY,
                I.IDCLOSEDBY = T.IDCLOSEDBY,
                I.IDTRANSPORTER = T.IDTRANSPORTER,
                I.IDRECEPBY = T.IDRECEPBY,
                I.DUPDATED = GETDATE(),
                I.DDATEINIT = T.DDATEINIT,
                I.DDATECLOSE = T.DDATECLOSE,
                I.DRECEPTION = T.DRECEPTION
            FROM INSPECTION I
            INNER JOIN @TDATA T ON I.ID = T.ID
            WHERE T.ID > 0;

            SET @IUPDATED = @@ROWCOUNT;

            -------- 5. Lógica de INSERCIÓN (INSERT) ----------------
            INSERT INTO INSPECTION (
                ICREATEDBY, 
                IDVEHICLE, 
                IDAREA, 
                IDINITBY, 
                IDCLOSEDBY, 
                IDTRANSPORTER, 
                IDRECEPBY,
                DDATEINIT, 
                DDATECLOSE, 
                DRECEPTION
            )
            SELECT 
                ICREATEDBY, 
                IDVEHICLE, 
                IDAREA,
                IDINITBY,
                IDCLOSEDBY,
                IDTRANSPORTER,
                IDRECEPBY,
                DDATEINIT,
                DDATECLOSE,
                DRECEPTION
            FROM @TDATA D
            WHERE D.ID = 0; 

            SET @IINSERTED = @@ROWCOUNT;
            SET @IID = SCOPE_IDENTITY();
			
            -- 6. Retorno de información final
            SELECT
                ISNULL(NULLIF(@IID, 0), (SELECT TOP 1 ID FROM @TDATA WHERE ID > 0)) AS IID, 
                ISNULL(@IINSERTED, 0) AS IINSERTED,
                ISNULL(@IUPDATED, 0) AS IUPDATED
        COMMIT TRAN
    END TRY
    BEGIN CATCH
        -- 7. Manejo de errores
        IF XACT_STATE() <> 0
            ROLLBACK TRAN

        DECLARE @ErrorMessage NVARCHAR(4000);
        SELECT @ErrorMessage = ERROR_PROCEDURE() + ' : ' + ERROR_MESSAGE();
        RAISERROR (@ErrorMessage, 16, 1);
    END CATCH
END;

-- INSPECTIONDETAIL

CREATE PROCEDURE USP_GET_INSPECTIONS_DETAIL
    @IDINSPECTION INT = NULL,
    @IDAREA INT = NULL,
    @IDFASE INT = NULL,
    @IDFEATURETYPE INT = NULL
AS
/* '===============================================================          
  '   NOMBRE                : USP_GET_INSPECTIONS_DETAIL
  '   FECHA CREACIÓN        : 30/03/2026
  '   CREADO POR            : JOSÉ DÍAZ
  '   CREADO PARA           : OBTENER EL DETALLE DE LAS INSPECCIONES
  '   FUNCIÓN               : SELECCIÓN CON JOINS A FEATURE Y FEATURE_TYPE
  '   VERSIÓN               : 1.0.0
  '===============================================================*/
BEGIN
    SET XACT_ABORT ON;               
    SET NOCOUNT ON;
    SET LOCK_TIMEOUT 180000;
    SELECT 
        D.ID,
        D.BVALUE,
        D.VOBSERVATION,
        D.VFILEURL,
        D.BACTIVE,
        I.ID AS IDINSPECTION,
        V.VVIN,
        V.VPLATE,
        C.VNAME VCOLOR,
        M.VNAME VMODEL,
        F.ID IDFEATURE,
        F.VNAME AS VFEATURE,
        FT.ID AS IDFEATURETYPE,
        FT.VNAME AS VFEATURETYPE,
        FA.ID AS IDFASE,
        FA.VNAME AS VFASE,
        A.ID AS IDAREA,
        A.VNAME AS VAREA
    FROM INSPECTIONDETAIL D WITH (NOLOCK)
	    INNER JOIN INSPECTION I WITH (NOLOCK) ON D.IDINSPECTION = I.ID
	    INNER JOIN VEHICLE V WITH (NOLOCK) ON I.IDVEHICLE = V.ID
	    INNER JOIN COLOR C WITH (NOLOCK) ON C.ID = V.IDCOLOR
	    INNER JOIN MODEL M WITH (NOLOCK) ON V.IDMODEL = M.ID
	    INNER JOIN FEATURE F WITH (NOLOCK) ON D.IDFEATURE = F.ID
	    INNER JOIN FEATURETYPE FT WITH (NOLOCK) ON F.IDFEATURETYPE = FT.ID
	    INNER JOIN FASE FA WITH (NOLOCK) ON FT.IDFASE = FA.ID
	    INNER JOIN AREA A WITH (NOLOCK) ON FA.IDAREA = A.ID
    WHERE D.BACTIVE = 1
    AND (@IDINSPECTION IS NULL OR I.ID = @IDINSPECTION)
    AND (@IDAREA IS NULL OR A.ID = @IDAREA)
    AND (@IDFASE IS NULL OR FA.ID = @IDFASE)
    AND (@IDFEATURETYPE IS NULL OR FT.ID = @IDFEATURETYPE);
END;

CREATE PROCEDURE USP_GET_INSPECTION_DETAIL_BY_ID
    @ID INT
AS
/* '===============================================================          
  '   NOMBRE                : USP_GET_DETAIL_INSPECTION_BY_ID
  '   FECHA CREACIÓN        : 30/03/2026
  '   CREADO POR            : JOSÉ DÍAZ
  '   CREADO PARA           : OBTENER UN DETALLE ESPECÍFICO POR ID
  '   FUNCIÓN               : SELECCIÓN FILTRADA POR LLAVE PRIMARIA
  '   VERSIÓN               : 1.0.0
  '===============================================================*/
BEGIN
    SET XACT_ABORT ON;               
    SET NOCOUNT ON;
    SET LOCK_TIMEOUT 180000;
    SELECT 
        D.ID,
        D.BVALUE,
        D.VOBSERVATION,
        D.VFILEURL,
        D.BACTIVE,
        I.ID AS IDINSPECTION,
        V.VVIN,
        V.VPLATE,
        C.VNAME VCOLOR,
        M.VNAME VMODEL,
        F.ID IDFEATURE,
        F.VNAME AS VFEATURE,
        FT.ID AS IDFEATURETYPE,
        FT.VNAME AS VFEATURETYPE,
        FA.ID AS IDFASE,
        FA.VNAME AS VFASE,
        A.ID AS IDAREA,
        A.VNAME AS VAREA
    FROM INSPECTIONDETAIL D WITH (NOLOCK)
	    INNER JOIN INSPECTION I WITH (NOLOCK) ON D.IDINSPECTION = I.ID
	    INNER JOIN VEHICLE V WITH (NOLOCK) ON I.IDVEHICLE = V.ID
	    INNER JOIN COLOR C WITH (NOLOCK) ON C.ID = V.IDCOLOR
	    INNER JOIN MODEL M WITH (NOLOCK) ON V.IDMODEL = M.ID
	    INNER JOIN FEATURE F WITH (NOLOCK) ON D.IDFEATURE = F.ID
	    INNER JOIN FEATURETYPE FT WITH (NOLOCK) ON F.IDFEATURETYPE = FT.ID
	    INNER JOIN FASE FA WITH (NOLOCK) ON FT.IDFASE = FA.ID
	    INNER JOIN AREA A WITH (NOLOCK) ON FA.IDAREA = A.ID
    WHERE D.ID = @ID;
END;

CREATE PROCEDURE USP_POST_INSPECTION_DETAIL
 @DATA NVARCHAR(MAX)
AS
/* '===============================================================          
  '   NOMBRE                : USP_POST_INSPECTION_DETAIL
  '   FECHA CREACIÓN        : 30/03/2026
  '   CREADO POR            : JOSÉ DÍAZ
  '   CREADO PARA           : INSERTAR O ACTUALIZAR DETALLES (UPSERT)
  '   FUNCIÓN               : PROCESAR JSON PARA CARGA MASIVA DE RESULTADOS
  '   VERSIÓN               : 1.0.0
  '===============================================================*/
SET XACT_ABORT ON               
SET NOCOUNT ON
SET LOCK_TIMEOUT 180000
BEGIN
    BEGIN TRY
        BEGIN TRAN 
            -- 1. Tabla temporal
            DECLARE @TDATA AS TABLE
            (
                ID INT,
                BVALUE INT,
                VOBSERVATION VARCHAR(500),
                VFILEURL VARCHAR(500),
                IDINSPECTION INT,
                IDFEATURE INT
            );

            -- 2. Carga desde JSON
            INSERT INTO @TDATA
            SELECT 
                ISNULL(M.Id, 0),
                M.Value,
                ISNULL(M.Observation, 'S/O'),
                M.FileUrl,
                M.InspectionId,
                M.FeatureId
            FROM OPENJSON(@DATA)
            WITH (
                Id INT,
                Value INT,
                Observation VARCHAR(500),
                FileUrl VARCHAR(500),
                InspectionId INT,
                FeatureId INT
            ) AS M;
		
            DECLARE @IID INT
            DECLARE @IINSERTED INT = 0
            DECLARE @IUPDATED INT = 0

            -------- 3. UPDATE ----------------
            UPDATE D
            SET 
                D.BVALUE = T.BVALUE,
                D.VOBSERVATION = T.VOBSERVATION,
                D.VFILEURL = T.VFILEURL,
                D.IDINSPECTION = T.IDINSPECTION,
                D.IDFEATURE = T.IDFEATURE
            FROM INSPECTIONDETAIL D
            INNER JOIN @TDATA T ON D.ID = T.ID
            WHERE T.ID > 0;

            SET @IUPDATED = @@ROWCOUNT;

            -------- 4. INSERT ----------------
            INSERT INTO INSPECTIONDETAIL (BVALUE, VOBSERVATION, VFILEURL, IDINSPECTION, IDFEATURE)
            SELECT 
                BVALUE, 
                VOBSERVATION, 
                VFILEURL,
                IDINSPECTION,
                IDFEATURE
            FROM @TDATA D
            WHERE D.ID = 0;

            SET @IINSERTED = @@ROWCOUNT;
            SET @IID = SCOPE_IDENTITY();
			
            -- 5. Resultado
            SELECT
                ISNULL(NULLIF(@IID, 0), (SELECT TOP 1 ID FROM @TDATA WHERE ID > 0)) AS IID, 
                ISNULL(@IINSERTED, 0) AS IINSERTED,
                ISNULL(@IUPDATED, 0) AS IUPDATED
        COMMIT TRAN
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0
            ROLLBACK TRAN

        DECLARE @ErrorMessage NVARCHAR(4000);
        SELECT @ErrorMessage = ERROR_PROCEDURE() + ' : ' + ERROR_MESSAGE();
        RAISERROR (@ErrorMessage, 16, 1);
    END CATCH
END;

-- INPECTIONFASE

CREATE PROCEDURE USP_GET_INPECTIONFASE
	@IDAREA INT = NULL,
	@IDFASE INT = NULL,
	@IDINSPECTION INT = NULL,
	@ISCOMPLETED BIT = NULL
AS
/* '===============================================================          
  '   NOMBRE                : USP_GET_INSPECTIONS_DETAIL
  '   FECHA CREACIÓN        : 05/04/2026
  '   CREADO POR            : JOSÉ DÍAZ
  '   CREADO PARA           : OBTENER TODAS LAS FASES PENDIENTES POR INSPECCION
  '   FUNCIÓN               : SELECCIÓN CON JOINS A FEATURE Y FEATURE_TYPE
  '   VERSIÓN               : 1.0.0
  '===============================================================*/
BEGIN
    SET XACT_ABORT ON;               
    SET NOCOUNT ON;
    SET LOCK_TIMEOUT 180000;
   	SELECT 
	    IFA.ID,
	    IFA.IDINSPECTION,
	    IFA.IDFASE,
	    F.VNAME AS FASE,
	    F.IDAREA,
	    A.VNAME AS AREA,
	    IFA.DCOMPLETED,
	    CASE 
	        WHEN IFA.DCOMPLETED IS NOT NULL THEN 1 
	        ELSE 0 
	    END AS ISCOMPLETED
	FROM INSPECTIONFASE IFA WITH (NOLOCK)
	    INNER JOIN FASE F WITH (NOLOCK) ON F.ID = IFA.IDFASE
	    INNER JOIN AREA A WITH (NOLOCK) ON A.ID = F.IDAREA
   	WHERE (@IDAREA IS NULL OR F.IDAREA = @IDAREA)
    AND (@IDFASE IS NULL OR IFA.IDFASE = @IDFASE)
   	AND (@IDINSPECTION IS NULL OR IFA.IDINSPECTION = @IDINSPECTION)
   	AND (
          @ISCOMPLETED IS NULL 
          OR (@ISCOMPLETED = 1 AND IFA.DCOMPLETED IS NOT NULL)
          OR (@ISCOMPLETED = 0 AND IFA.DCOMPLETED IS NULL)
     )
END;

CREATE PROCEDURE USP_POST_INSPECTION_FASE

-- GENERATE INSPECTION 

CREATE PROCEDURE USP_POST_GENERATE_FULL_INSPECTION
    @IDUSER INT,
    @DATA NVARCHAR(MAX)
AS
/* '===============================================================          
  '   NOMBRE                : USP_POST_GENERATE_FULL_INSPECTION
  '   FECHA CREACIÓN        : 06/04/2026
  '   CREADO POR            : JOSÉ DÍAZ
  '   CREADO PARA           : GENERAR MÚLTIPLES INSPECCIONES (FORMATO UPSERT)
  '   VERSIÓN               : 2.2.0
  '===============================================================*/
SET XACT_ABORT ON               
SET NOCOUNT ON
BEGIN
    BEGIN TRY
        BEGIN TRAN 
            
            -- 1. Tabla temporal para los datos del JSON
            DECLARE @VEHICULOS_A_PROCESAR TABLE (
                VehicleId INT,
                AreaId INT,
                ModelId INT
            );

            INSERT INTO @VEHICULOS_A_PROCESAR (VehicleId, AreaId, ModelId)
            SELECT J.VehicleId, J.AreaId, V.IDMODEL
            FROM OPENJSON(@DATA)
            WITH (VehicleId INT, AreaId INT) AS J
            INNER JOIN VEHICLE V WITH (NOLOCK) ON V.ID = J.VehicleId; -- VIM O PLACA

            -- 2. Variables de control
            DECLARE @CUR_VEHICLEID INT, @CUR_AREAID INT, @CUR_MODELID INT;
            DECLARE @IID INT = 0;
            DECLARE @IINSERTED INT = 0;

            -- 3. Cursor para procesar el lote
            DECLARE VEHICLE_CURSOR CURSOR FOR 
            SELECT VehicleId, AreaId, ModelId FROM @VEHICULOS_A_PROCESAR;

            OPEN VEHICLE_CURSOR;
            FETCH NEXT FROM VEHICLE_CURSOR INTO @CUR_VEHICLEID, @CUR_AREAID, @CUR_MODELID;

            WHILE @@FETCH_STATUS = 0
            BEGIN
                -- Validamos configuración antes de insertar
                IF EXISTS (
                    SELECT 1 FROM FEATURE F WITH (NOLOCK)
                    INNER JOIN FEATURETYPE FT WITH (NOLOCK) ON F.IDFEATURETYPE = FT.ID
                    INNER JOIN FASE FA WITH (NOLOCK) ON FT.IDFASE = FA.ID
                    WHERE FA.IDAREA = @CUR_AREAID AND F.IDMODEL = @CUR_MODELID AND F.BACTIVE = 1
                )
                BEGIN
                    -- A. Insertar Inspección
                    INSERT INTO INSPECTION (ICREATEDBY, IDVEHICLE, IDAREA, BACTIVE, DCREATED, DUPDATED)
                    VALUES (@IDUSER, @CUR_VEHICLEID, @CUR_AREAID, 1, GETDATE(), GETDATE());

                    SET @IID = SCOPE_IDENTITY();
                    SET @IINSERTED = @IINSERTED + 1;

                    -- B. Insertar Detalles (Puntos de inspección)
                    INSERT INTO INSPECTIONDETAIL (BVALUE, VOBSERVATION, VFILEURL, BACTIVE, IDINSPECTION, IDFEATURE)
                    SELECT F.BDEFAULT, 'S/O', NULL, 1, @IID, F.ID
                    FROM FEATURE F WITH (NOLOCK)
                    INNER JOIN FEATURETYPE FT WITH (NOLOCK) ON F.IDFEATURETYPE = FT.ID
                    INNER JOIN FASE FA WITH (NOLOCK) ON FT.IDFASE = FA.ID
                    WHERE FA.IDAREA = @CUR_AREAID AND F.IDMODEL = @CUR_MODELID AND F.BACTIVE = 1;

                    -- C. Insertar Fases
                    INSERT INTO INSPECTIONFASE (IDFASE, IDINSPECTION, DCOMPLETED)
                    SELECT DISTINCT FT.IDFASE, @IID, NULL
                    FROM FEATURE F WITH (NOLOCK)
                    INNER JOIN FEATURETYPE FT WITH (NOLOCK) ON F.IDFEATURETYPE = FT.ID
                    WHERE F.IDMODEL = @CUR_MODELID AND F.BACTIVE = 1 
                      AND FT.IDFASE IN (SELECT ID FROM FASE WITH (NOLOCK) WHERE IDAREA = @CUR_AREAID);
                END

                FETCH NEXT FROM VEHICLE_CURSOR INTO @CUR_VEHICLEID, @CUR_AREAID, @CUR_MODELID;
            END

            CLOSE VEHICLE_CURSOR;
            DEALLOCATE VEHICLE_CURSOR;

            -- 4. RETORNO EN FORMATO ESTÁNDAR
            SELECT
                ISNULL(NULLIF(@IID, 0), 0) AS IID, 
                ISNULL(@IINSERTED, 0) AS IINSERTED,
                0 AS IUPDATED -- En generación de inspecciones no suele haber updates masivos
            
        COMMIT TRAN
    END TRY
    BEGIN CATCH
        IF CURSOR_STATUS('global','VEHICLE_CURSOR') >= 0 
        BEGIN
            CLOSE VEHICLE_CURSOR;
            DEALLOCATE VEHICLE_CURSOR;
        END
        IF XACT_STATE() <> 0 ROLLBACK TRAN;
        
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_PROCEDURE() + ' : ' + ERROR_MESSAGE();
        RAISERROR (@ErrorMessage, 16, 1);
    END CATCH
END;