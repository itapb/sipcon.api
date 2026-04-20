USE SODA;

-- INSPECTIONS
EXEC USP_GET_INSPECTIONS @IDAREA = 1; -- IDAREA es opcional
EXEC USP_GET_INSPECTION_BY_ID @ID = 1;
EXEC USP_POST_INSPECTION @DATA = '
[
    {
        "Id": 0,
        "CreatedBy": 1,
        "VehicleId": 2524,
        "AreaId": 1,
        "InitBy": 1,
        "TransporterId": 1,
        "DInit": "2026-04-06T08:00:00",
        "DReception": "2026-04-06T07:30:00"
    }
]';

-- INSPECTIONDETAILS
EXEC USP_GET_INSPECTIONS_DETAIL @IDFEATURETYPE = 1; --@IDFASE = 1; @IDAREA=1;
EXEC USP_GET_INSPECTION_DETAIL_BY_ID @ID = 5;
EXEC USP_POST_INSPECTION_DETAIL @DATA = '[
    { 
		"Value": true, 
		"Observation": "Todo en perfectas condiciones", 
        "InspectionId": 1, 
		"FeatureId": 1
    }
]';

-- INSPECTIONFASE
EXEC USP_GET_INPECTIONFASE; --@IDAREA = 1, @IDFASE = 1, @IDINSPECTION = 1, @ISCOMPLETED = 1         
EXEC USP_POST_INSPECTION_FASE @DATA = '[
    {
        "FaseId": 1, 
        "InspectionId": 1, 
        "CompletedDate": "2026-04-05T15:30:00"
    },
	{
        "FaseId": 2, 
        "InspectionId": 1
    }
]';

-- Generar FullInspección
EXEC USP_POST_GENERATE_FULL_INSPECTION @IDUSER = 1, @DATA = '[
    { "VehicleId": 2523, "AreaId": 1 },
    { "VehicleId": 2524, "AreaId": 2 },
    { "VehicleId": 2525, "AreaId": 1 }
]';
