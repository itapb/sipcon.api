GO
/****** Object:  StoredProcedure [dbo].[USP_CHECK_IMPORTPARTS]    Script Date: 18/08/2025 8:46:26 ******/
SET ANSI_NULLS ON

GO
SET QUOTED_IDENTIFIER ON
GO
IF  NOT EXISTS (SELECT * FROM SYS.OBJECTS

                WHERE OBJECT_ID = OBJECT_ID(N'[DBO].[USP_CHECK_IMPORTPARTS]')

                AND TYPE IN (N'P', N'PC', N'TF', N'FN'))
EXEC('CREATE PROCEDURE [DBO].[USP_CHECK_IMPORTPARTS] AS BEGIN SET NOCOUNT ON  END')
GO
ALTER PROCEDURE [dbo].[USP_CHECK_IMPORTPARTS]  
 @IDUSER INT,
 @DATA NVARCHAR(MAX),
 @MODELS NVARCHAR(MAX) = NULL
AS
/* '===============================================================          
  '   NOMBRE                : 
  '   FECHA CREACIÓN        : 
  '   CREADO POR            : JUAN GUARECUCO
  '   CREADO PARA           : 
  '   FUNCIÓN               :  
  '   VERSIÓN               : 
  '   MODIFICADO EN         : 
  '   MODIFICADO POR        :  
  '   RAZÓN DE MODIFICACIÓN : 
  '===============================================================*/

SET XACT_ABORT ON               
SET NOCOUNT ON
SET LOCK_TIMEOUT 180000


			--DECLARA UNA VARIABLE @TDATA
				DECLARE @PARTMODEL AS TABLE 
				(
				IDPART INT,
				IDMODEL INT,
				BRELATED BIT
				)
			

				DECLARE @TDATA AS TABLE
				(
					ID INT,
					VINNERCODE VARCHAR(20),
					VMASTERCODE VARCHAR(20),
					VALTERCODE VARCHAR(20),
					VREPLACEMENTCODE VARCHAR(20),
					VBARCODE VARCHAR(20),
					VDESCRIPTION VARCHAR(100),
					IDTYPE INT,
					IDFAMILY INT,
					IDSUBFAMILY INT,
					NPRICE NUMERIC(18,2),
					NCOST NUMERIC(18,2),
					NDISCOUNT NUMERIC(18,2),
					NWEIGHT NUMERIC(18,2),
					VSIZE VARCHAR(1),
					IMINSALE INT,
					IPACKING INT,
					BSELL BIT,
					BPURCHASE BIT,
					BWARRANTY BIT,
					BLICENSE BIT,
					BORIGINAL BIT,
					BSERIALIZABLE BIT,
					IDSUPPLIER INT,
					IDBRAND INT,
					IDUM INT,
					IDTAX INT,
					VRATING VARCHAR(1),
					BACTIVE BIT,
					VALTERDESCRIPTION VARCHAR(40),
					VROWREFERENCE VARCHAR(5)
				);




				INSERT INTO @TDATA
				SELECT	Id ,
						InnerCode ,
						MasterCode ,
						AlterCode ,
						ReplacementCode,
						BarCode ,
						Description,
						TypeId,
						FamilyId ,
						SubFamilyId ,
						Price ,
						Cost ,
						Discount ,
						Weight ,
						Size ,
						MinSale ,
						Packing ,
						Sell ,
						Purchase ,
						Warranty ,
						License ,
						Original ,
						Serializable ,
						SupplierId ,
						BrandId ,
						UmId ,
						TaxId ,
						Rating ,
						IsActive,
						AlterDescription,
						RowReference
				FROM OPENJSON(@DATA)
				WITH (
						Id int,
						InnerCode varchar(20), 
						MasterCode varchar(20), 
						AlterCode varchar(20), 
						ReplacementCode varchar(20),
						BarCode varchar(20),
						Description varchar(100),
						TypeId int,
						FamilyId int,
						SubFamilyId int,
						Price numeric(18,2),
						Cost numeric(18,2),
						Discount numeric(18,2),
						Weight numeric(18,2),
						Size varchar(1),
						MinSale int,
						Packing int,
						Sell bit,
						Purchase bit,
						Warranty bit,
						License bit,
						Original bit,
						Serializable bit,
						SupplierId Int,
						BrandId int,
						UmId int,
						TaxId int,
						Rating varchar(1),
						IsActive bit,
						AlterDescription varchar(40),
						RowReference Varchar(5)
					  ) AS L

			IF @MODELS IS NOT NULL
			BEGIN
				INSERT INTO @PARTMODEL
					SELECT	PartId ,
							ModelId ,
							IsRelated 
					FROM OPENJSON(@MODELS)
					WITH (
							PartId int,
							ModelId int,
							IsRelated bit
						  ) AS L
			END

			--VALIDACIONES

			DECLARE @MENSAJE VARCHAR(100)
			SET @MENSAJE=NULL
            DECLARE @SERIAL VARCHAR(20)=NULL
			DECLARE @VROWREF VARCHAR(5)=NULL
			DECLARE @Duplicados NVARCHAR(MAX) = ''


	 
				------------------------------VIINERCODE DUPLICADO EN EXCEL------------------------------------------------------
				IF @MENSAJE IS NULL
					BEGIN
					SELECT TOP 1 @Duplicados = VINNERCODE
					FROM (
						SELECT VINNERCODE
						FROM @TDATA
						GROUP BY VINNERCODE
						HAVING COUNT(*) > 1
					) AS Duplicados
					SELECT TOP 1 @VROWREF= VROWREFERENCE FROM @TDATA WHERE VINNERCODE=@Duplicados
					IF @Duplicados <> ''
					BEGIN
						SET @MENSAJE = 'DOCUMENTO CONTIENE REGISTRO DUPLICADO: ' + @Duplicados
					END
					END

					------------------------------SERIAL DUPLICADO EN EXCEL------------------------------------------------------
					IF @MENSAJE IS NULL
					BEGIN
					SELECT TOP 1 @Duplicados = VENGINESERIAL
					FROM (
						SELECT VENGINESERIAL
						FROM @TDATA
						GROUP BY VENGINESERIAL
						HAVING COUNT(*) > 1
					) AS Duplicados

					SELECT TOP 1 @VROWREF= VROWREFERENCE FROM @TDATA WHERE VENGINESERIAL=@Duplicados
					IF @Duplicados <> ''
					BEGIN
					
						SET @MENSAJE = 'DOCUMENTO CONTIENE REGISTRO DUPLICADO: ' + @Duplicados
					END
					END

					------------------------------PLACA DUPLICADO EN EXCEL------------------------------------------------------
					IF @MENSAJE IS NULL
					BEGIN
					SELECT TOP 1 @Duplicados = VPLATE
					FROM (
						SELECT VPLATE
						FROM @TDATA
						GROUP BY VPLATE
						HAVING COUNT(*) > 1
					) AS Duplicados
					SELECT TOP 1 @VROWREF= VROWREFERENCE FROM @TDATA WHERE VPLATE=@Duplicados
					IF @Duplicados <> ''
					BEGIN
						SET @MENSAJE = 'DOCUMENTO CONTIENE REGISTRO DUPLICADO: ' + @Duplicados
					END
					END


					 -----------------------------VALIDACION MARCA PLANTA-------------------------------------------
			     IF @MENSAJE IS NULL AND EXISTS ( SELECT * FROM @TDATA A
							INNER JOIN MODEL M ON M.ID=A.IDMODEL
							INNER JOIN V_SUPPLIERS S ON S.IDSUPPLIER=A.IDSUPPLIER
							WHERE S.IDBRAND <> M.IDBRAND
							)
				BEGIN
				SELECT TOP 1 @SERIAL=A.VVIN,@VROWREF=A.VROWREFERENCE FROM @TDATA A
							INNER JOIN MODEL M ON M.ID=A.IDMODEL
							INNER JOIN V_SUPPLIERS S ON S.IDSUPPLIER=A.IDSUPPLIER
							WHERE S.IDBRAND <> M.IDBRAND
					SET @MENSAJE= 'EXISTEN VEHICULOS CON MODELOS NO ACORDE A LA MARCA DE LA PLANTA: '+ @SERIAL; 
				END
				-----------------------------VALIDACION MARCA-CONCESIONARIO-------------------------------------------
				IF  @MENSAJE IS NULL AND EXISTS (SELECT * FROM @TDATA D
							INNER JOIN V_SUPPLIERS S ON S.IDSUPPLIER=D.IDSUPPLIER
							WHERE S.IDBRAND <> @IDBRAND)
				BEGIN
				    SELECT TOP 1 @SERIAL=D.VVIN,@VROWREF=D.VROWREFERENCE FROM @TDATA D
							INNER JOIN V_SUPPLIERS S ON S.IDSUPPLIER=D.IDSUPPLIER
							WHERE S.IDBRAND <> @IDBRAND
					SET @MENSAJE='VEHICULO QUE NO CORRESPONDEN A LA MARCA DEL CONCESIONARIO ASIGNADO: '+@SERIAL
				END
				-----------------------------VALIDACION PARA VIN-------------------------------------------
				IF @MENSAJE IS NULL AND EXISTS ( SELECT * FROM @TDATA A
							INNER JOIN VEHICLE V WITH (NOLOCK) ON A.VVIN = V.VVIN 
							)
				BEGIN 
				   SELECT TOP 1 @SERIAL=A.VVIN,@VROWREF=A.VROWREFERENCE FROM @TDATA A
							INNER JOIN VEHICLE V WITH (NOLOCK) ON A.VVIN = V.VVIN 
					SET @MENSAJE= 'EL REGISTRO QUE INTENTAS CREAR YA EXISTE: '+ @SERIAL;
				END
				-----------------------------VALIDACION PARA PLACA-------------------------------------------
				
				IF @MENSAJE IS NULL AND EXISTS ( SELECT * FROM @TDATA A
							INNER JOIN VEHICLE V WITH (NOLOCK) ON LTRIM(RTRIM(A.VPLATE)) = LTRIM(RTRIM(V.VPLATE)) 
							)
				BEGIN
				     SELECT TOP 1 @SERIAL=A.VPLATE,@VROWREF=A.VROWREFERENCE FROM @TDATA A
					 INNER JOIN VEHICLE V WITH (NOLOCK) ON LTRIM(RTRIM(A.VPLATE)) = LTRIM(RTRIM(V.VPLATE))
					SET @MENSAJE= 'EL REGISTRO QUE INTENTAS CREAR YA EXISTE: '+ @SERIAL;
				END

				-----------------------------VALIDACION PARA SERIAL DE MOTOR-------------------------------------------
				IF @MENSAJE IS NULL AND EXISTS ( SELECT * FROM @TDATA A
							INNER JOIN VEHICLE V WITH (NOLOCK) ON LTRIM(RTRIM(A.VENGINESERIAL)) = LTRIM(RTRIM(V.VENGINESERIAL))  
							)
				BEGIN
				     SELECT TOP 1 @SERIAL=A.VENGINESERIAL,@VROWREF=A.VROWREFERENCE FROM @TDATA A
					 INNER JOIN VEHICLE V WITH (NOLOCK) ON LTRIM(RTRIM(A.VENGINESERIAL)) = LTRIM(RTRIM(V.VENGINESERIAL))
					SET @MENSAJE= 'EL REGISTRO QUE INTENTAS CREAR YA EXISTE: '+ @SERIAL; 
				END

			-- Lanzar error si se detectó alguno
				IF @MENSAJE IS NOT NULL
				BEGIN
					IF @VROWREF IS NOT NULL AND LTRIM(RTRIM(@VROWREF)) <> ''
						SET @MENSAJE = @MENSAJE + ' -FILA: ' + @VROWREF

					RAISERROR(@MENSAJE, 16, 1)
					RETURN
				END