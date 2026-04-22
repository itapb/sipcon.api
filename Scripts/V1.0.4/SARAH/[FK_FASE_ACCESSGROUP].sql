-- ============================================================
-- 1. ELIMINAR FK y columna IDACCESSGROUP de AREA
-- ============================================================
ALTER TABLE [dbo].[AREA]
    DROP CONSTRAINT [FK_AREA_ACCESSGROUP];
GO

ALTER TABLE [dbo].[AREA]
    DROP COLUMN [IDACCESSGROUP];
GO

-- ============================================================
-- 2. AGREGAR IDACCESSGROUP a FASE con su FK
-- ============================================================
ALTER TABLE [dbo].[FASE]
    ADD [IDACCESSGROUP] [int] NULL;
GO

ALTER TABLE [dbo].[FASE]
    ADD CONSTRAINT [FK_FASE_ACCESSGROUP]
    FOREIGN KEY ([IDACCESSGROUP])
    REFERENCES [dbo].[ACCESSGROUP]([ID]);
GO