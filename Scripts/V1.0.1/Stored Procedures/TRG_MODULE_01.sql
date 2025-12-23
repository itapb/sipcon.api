ALTER TRIGGER [dbo].[TRG_MODULE_01] ON [dbo].[MODULE] AFTER UPDATE,INSERT                            
 AS                               
/* '===============================================================                                                     
 '   FECHA CREACIÓN     :                                 
 '   CREADO POR         :                                          
 '   FINALIDAD          :                            
 '   ENTRADA            :                                   
 '   SALIDA             :                                     
 '   VERSIÓN            :                                                                      
 '   COMENTARIOS        :                                                    
 '   MODIFICADO EL      :                                          
 '   MODIFICADO POR     :                                                 
 '   RAZÓN MODIFICACIÓN :                                                 
 '===============================================================*/                                   
                            
BEGIN                   
          
 DECLARE @IDACTION INT          
            
-- 1           
  IF UPDATE (BGET)                   
  BEGIN                 
    SELECT @IDACTION=ID FROM DBO.ACTION WITH (NOLOCK) WHERE VNAME='GET'            
           
    DELETE ACTIONMODULE          
    WHERE IDACTION=@IDACTION          
    AND IDMODULE IN (SELECT ID FROM inserted A WHERE BGET=0)          
          
    INSERT INTO ACTIONMODULE(IDACTION,IDMODULE)          
    SELECT @IDACTION,ID FROM inserted A WHERE BGET=1          
    AND NOT EXISTS (SELECT * FROM ACTIONMODULE B WITH (NOLOCK) WHERE B.IDACTION=@IDACTION AND B.IDMODULE=A.ID)          
          
  END                    
           
          
 -- 2          
 IF UPDATE (BCREATE)                   
  BEGIN                 
   SELECT @IDACTION=ID FROM DBO.ACTION WITH (NOLOCK) WHERE VNAME='CREATE'            
            
   DELETE ACTIONMODULE          
   WHERE IDACTION=@IDACTION          
   AND IDMODULE IN (SELECT ID FROM inserted A where BCREATE=0)          
          
   INSERT INTO ACTIONMODULE(IDACTION,IDMODULE)          
   SELECT @IDACTION,ID FROM inserted A where BCREATE=1          
   and NOT EXISTS (SELECT * FROM ACTIONMODULE B WITH (NOLOCK) WHERE B.IDACTION=@IDACTION AND B.IDMODULE=A.ID)          
              
  END             
          
-- 3          
 IF UPDATE (BEDIT)                   
  BEGIN                 
   SELECT @IDACTION=ID FROM DBO.ACTION WITH (NOLOCK) WHERE VNAME='EDIT'            
          
   DELETE ACTIONMODULE          
   WHERE IDACTION=@IDACTION          
   AND IDMODULE IN (SELECT ID FROM inserted A where BEDIT=0)          
          
   INSERT INTO ACTIONMODULE(IDACTION,IDMODULE)          
   SELECT @IDACTION,ID FROM inserted A where BEDIT=1          
   and NOT EXISTS (SELECT * FROM ACTIONMODULE B WITH (NOLOCK) WHERE B.IDACTION=@IDACTION AND B.IDMODULE=A.ID)          
            
             
  END             
          
-- 4          
 IF UPDATE (BIMPORT)                   
  BEGIN                 
   SELECT @IDACTION=ID FROM DBO.ACTION WITH (NOLOCK) WHERE VNAME='IMPORT'            
             
   DELETE ACTIONMODULE          
   WHERE IDACTION=@IDACTION          
   AND IDMODULE IN (SELECT ID FROM inserted A where BIMPORT=0)          
          
   INSERT INTO ACTIONMODULE(IDACTION,IDMODULE)          
   SELECT @IDACTION,ID FROM inserted A where BIMPORT=1          
   and NOT EXISTS (SELECT * FROM ACTIONMODULE B WITH (NOLOCK) WHERE B.IDACTION=@IDACTION AND B.IDMODULE=A.ID)          
  END           
          
  -- 5          
 IF UPDATE (BEXPORT)                   
  BEGIN                 
   SELECT @IDACTION=ID FROM DBO.ACTION WITH (NOLOCK) WHERE VNAME='EXPORT'          
             
   DELETE ACTIONMODULE          
   WHERE IDACTION=@IDACTION          
   AND IDMODULE IN (SELECT ID FROM inserted A where BEXPORT=0)          
          
   INSERT INTO ACTIONMODULE(IDACTION,IDMODULE)          
   SELECT @IDACTION,ID FROM inserted A where BEXPORT=1          
   and NOT EXISTS (SELECT * FROM ACTIONMODULE B WITH (NOLOCK) WHERE B.IDACTION=@IDACTION AND B.IDMODULE=A.ID)          
  END           
          
  -- 6          
 IF UPDATE (BACTIVATE)                   
  BEGIN                 
   SELECT @IDACTION=ID FROM DBO.ACTION WITH (NOLOCK) WHERE VNAME='ACTIVATE'            
             
   DELETE ACTIONMODULE          
   WHERE IDACTION=@IDACTION          
   AND IDMODULE IN (SELECT ID FROM inserted A where BACTIVATE=0)          
          
   INSERT INTO ACTIONMODULE(IDACTION,IDMODULE)          
   SELECT @IDACTION,ID FROM inserted A where BACTIVATE=1          
   and NOT EXISTS (SELECT * FROM ACTIONMODULE B WITH (NOLOCK) WHERE B.IDACTION=@IDACTION AND B.IDMODULE=A.ID)          
  END           
          
  -- 7          
 IF UPDATE (BDEACTIVATE)                   
  BEGIN                 
   SELECT @IDACTION=ID FROM DBO.ACTION WITH (NOLOCK) WHERE VNAME='DEACTIVATE'            
          
   DELETE ACTIONMODULE          
   WHERE IDACTION=@IDACTION          
   AND IDMODULE IN (SELECT ID FROM inserted A where BDEACTIVATE=0)          
          
   INSERT INTO ACTIONMODULE(IDACTION,IDMODULE)          
   SELECT @IDACTION,ID FROM inserted A where BDEACTIVATE=1          
   and NOT EXISTS (SELECT * FROM ACTIONMODULE B WITH (NOLOCK) WHERE B.IDACTION=@IDACTION AND B.IDMODULE=A.ID)          
  END          
           
 -- 8          
  IF UPDATE (BASSIGN)                   
  BEGIN                 
   SELECT @IDACTION=ID FROM DBO.ACTION WITH (NOLOCK) WHERE VNAME='ASSIGN'            
             
   DELETE ACTIONMODULE          
   WHERE IDACTION=@IDACTION          
   AND IDMODULE IN (SELECT ID FROM inserted A where BASSIGN=0)          
          
   INSERT INTO ACTIONMODULE(IDACTION,IDMODULE)          
   SELECT @IDACTION,ID FROM inserted A where BASSIGN=1          
   and NOT EXISTS (SELECT * FROM ACTIONMODULE B WITH (NOLOCK) WHERE B.IDACTION=@IDACTION AND B.IDMODULE=A.ID)          
  END          
          
  -- 9          
IF UPDATE (BUNASSIGN)                   
  BEGIN                 
   SELECT @IDACTION=ID FROM DBO.ACTION WITH (NOLOCK) WHERE VNAME='UNASSIGN'            
             
   DELETE ACTIONMODULE          
   WHERE IDACTION=@IDACTION          
   AND IDMODULE IN (SELECT ID FROM inserted A where BUNASSIGN=0)          
          
   INSERT INTO ACTIONMODULE(IDACTION,IDMODULE)          
   SELECT @IDACTION,ID FROM inserted A where BUNASSIGN=1          
   and NOT EXISTS (SELECT * FROM ACTIONMODULE B WITH (NOLOCK) WHERE B.IDACTION=@IDACTION AND B.IDMODULE=A.ID)          
          
  END          
          
  -- 10          
IF UPDATE (BLOCK)                   
  BEGIN                 
   SELECT @IDACTION=ID FROM DBO.ACTION WITH (NOLOCK) WHERE VNAME='LOCK'           
             
   DELETE ACTIONMODULE          
   WHERE IDACTION=@IDACTION          
   AND IDMODULE IN (SELECT ID FROM inserted A where BLOCK=0)          
          
   INSERT INTO ACTIONMODULE(IDACTION,IDMODULE)          
   SELECT @IDACTION,ID FROM inserted A where BLOCK=1          
   and NOT EXISTS (SELECT * FROM ACTIONMODULE B WITH (NOLOCK) WHERE B.IDACTION=@IDACTION AND B.IDMODULE=A.ID)          
          
  END          
          
  -- 11          
IF UPDATE (BUNLOCK)                   
  BEGIN                 
   SELECT @IDACTION=ID FROM DBO.ACTION WITH (NOLOCK) WHERE VNAME='UNLOCK'            
             
   DELETE ACTIONMODULE          
   WHERE IDACTION=@IDACTION          
   AND IDMODULE IN (SELECT ID FROM inserted A where BUNLOCK=0)          
          
   INSERT INTO ACTIONMODULE(IDACTION,IDMODULE)          
   SELECT @IDACTION,ID FROM inserted A where BUNLOCK=1          
   and NOT EXISTS (SELECT * FROM ACTIONMODULE B WITH (NOLOCK) WHERE B.IDACTION=@IDACTION AND B.IDMODULE=A.ID)          
          
  END          
          
  -- 12          
IF UPDATE (BRECEIVE)                   
  BEGIN                 
   SELECT @IDACTION=ID FROM DBO.ACTION WITH (NOLOCK) WHERE VNAME='RECEIVE'            
             
   DELETE ACTIONMODULE          
   WHERE IDACTION=@IDACTION          
   AND IDMODULE IN (SELECT ID FROM inserted A where BRECEIVE=0)          
          
   INSERT INTO ACTIONMODULE(IDACTION,IDMODULE)          
   SELECT @IDACTION,ID FROM inserted A where BRECEIVE=1          
   and NOT EXISTS (SELECT * FROM ACTIONMODULE B WITH (NOLOCK) WHERE B.IDACTION=@IDACTION AND B.IDMODULE=A.ID)          
          
  END          
          
          
  -- 13      
IF UPDATE (BGENERATE)                   
  BEGIN                 
   SELECT @IDACTION=ID FROM DBO.ACTION WITH (NOLOCK) WHERE VNAME='GENERATE'            
             
   DELETE ACTIONMODULE          
   WHERE IDACTION=@IDACTION          
   AND IDMODULE IN (SELECT ID FROM inserted A where BGENERATE=0)          
          
   INSERT INTO ACTIONMODULE(IDACTION,IDMODULE)          
   SELECT @IDACTION,ID FROM inserted A where BGENERATE=1          
   and NOT EXISTS (SELECT * FROM ACTIONMODULE B WITH (NOLOCK) WHERE B.IDACTION=@IDACTION AND B.IDMODULE=A.ID)          
          
  END          
          
  -- 14          
IF UPDATE (BTOVALIDATE)                   
  BEGIN                 
   SELECT @IDACTION=ID FROM DBO.ACTION WITH (NOLOCK) WHERE VNAME='TOVALIDATE'            
             
   DELETE ACTIONMODULE          
   WHERE IDACTION=@IDACTION          
   AND IDMODULE IN (SELECT ID FROM inserted A where BTOVALIDATE=0)         
          
   INSERT INTO ACTIONMODULE(IDACTION,IDMODULE)          
   SELECT @IDACTION,ID FROM inserted A where BTOVALIDATE=1          
   and NOT EXISTS (SELECT * FROM ACTIONMODULE B WITH (NOLOCK) WHERE B.IDACTION=@IDACTION AND B.IDMODULE=A.ID)          
          
  END          
          
  -- 15          
IF UPDATE (BTOAPPROVE)                   
  BEGIN                 
   SELECT @IDACTION=ID FROM DBO.ACTION WITH (NOLOCK) WHERE VNAME='TOAPPROVE'            
             
   DELETE ACTIONMODULE          
   WHERE IDACTION=@IDACTION          
   AND IDMODULE IN (SELECT ID FROM inserted A where BTOAPPROVE=0)          
          
   INSERT INTO ACTIONMODULE(IDACTION,IDMODULE)          
   SELECT @IDACTION,ID FROM inserted A where BTOAPPROVE=1          
   and NOT EXISTS (SELECT * FROM ACTIONMODULE B WITH (NOLOCK) WHERE B.IDACTION=@IDACTION AND B.IDMODULE=A.ID)          
          
  END          
          
  -- 16          
IF UPDATE (BVALIDATE)                   
  BEGIN                 
   SELECT @IDACTION=ID FROM DBO.ACTION WITH (NOLOCK) WHERE VNAME='VALIDATE'            
             
   DELETE ACTIONMODULE          
   WHERE IDACTION=@IDACTION          
   AND IDMODULE IN (SELECT ID FROM inserted A where BVALIDATE=0)          
          
   INSERT INTO ACTIONMODULE(IDACTION,IDMODULE)          
   SELECT @IDACTION,ID FROM inserted A where BVALIDATE=1          
   and NOT EXISTS (SELECT * FROM ACTIONMODULE B WITH (NOLOCK) WHERE B.IDACTION=@IDACTION AND B.IDMODULE=A.ID)          
          
  END          
          
  -- 17          
IF UPDATE (BAPPROVE)                   
  BEGIN                 
             
   SELECT @IDACTION=ID FROM DBO.ACTION WITH (NOLOCK) WHERE VNAME='APPROVE'            
             
   DELETE ACTIONMODULE          
   WHERE IDACTION=@IDACTION          
   AND IDMODULE IN (SELECT ID FROM inserted A where BAPPROVE=0)          
          
   INSERT INTO ACTIONMODULE(IDACTION,IDMODULE)          
   SELECT @IDACTION,ID FROM inserted A where BAPPROVE=1          
   and NOT EXISTS (SELECT * FROM ACTIONMODULE B WITH (NOLOCK) WHERE B.IDACTION=@IDACTION AND B.IDMODULE=A.ID)          
          
  END          
          
  -- 18          
IF UPDATE (BPROCESS)                   
  BEGIN                 
   SELECT @IDACTION=ID FROM DBO.ACTION WITH (NOLOCK) WHERE VNAME='PROCESS'            
             
   DELETE ACTIONMODULE          
   WHERE IDACTION=@IDACTION          
   AND IDMODULE IN (SELECT ID FROM inserted A where BPROCESS=0)          
          
   INSERT INTO ACTIONMODULE(IDACTION,IDMODULE)          
   SELECT @IDACTION,ID FROM inserted A where BPROCESS=1          
   and NOT EXISTS (SELECT * FROM ACTIONMODULE B WITH (NOLOCK) WHERE B.IDACTION=@IDACTION AND B.IDMODULE=A.ID)          
          
  END          
          
  -- 19          
IF UPDATE (BDECLINE)                   
  BEGIN                 
   SELECT @IDACTION=ID FROM DBO.ACTION WITH (NOLOCK) WHERE VNAME='DECLINE'            
             
   DELETE ACTIONMODULE          
   WHERE IDACTION=@IDACTION          
   AND IDMODULE IN (SELECT ID FROM inserted A where BDECLINE=0)          
          
   INSERT INTO ACTIONMODULE(IDACTION,IDMODULE)          
   SELECT @IDACTION,ID FROM inserted A where BDECLINE=1          
   and NOT EXISTS (SELECT * FROM ACTIONMODULE B WITH (NOLOCK) WHERE B.IDACTION=@IDACTION AND B.IDMODULE=A.ID)          
          
  END          
          
IF UPDATE (BAVAILABLE)                   
  BEGIN                 
   SELECT @IDACTION=ID FROM DBO.ACTION WITH (NOLOCK) WHERE VNAME='AVAILABLE'            
             
   DELETE ACTIONMODULE          
   WHERE IDACTION=@IDACTION          
   AND IDMODULE IN (SELECT ID FROM inserted A where BAVAILABLE=0)          
          
   INSERT INTO ACTIONMODULE(IDACTION,IDMODULE)          
   SELECT @IDACTION,ID FROM inserted A where BAVAILABLE=1          
   and NOT EXISTS (SELECT * FROM ACTIONMODULE B WITH (NOLOCK) WHERE B.IDACTION=@IDACTION AND B.IDMODULE=A.ID)          
          
  END          
          
           
IF UPDATE (BUNAVAILABLE)                   
  BEGIN                 
   SELECT @IDACTION=ID FROM DBO.ACTION WITH (NOLOCK) WHERE VNAME='UNAVAILABLE'            
             
   DELETE ACTIONMODULE          
   WHERE IDACTION=@IDACTION          
   AND IDMODULE IN (SELECT ID FROM inserted A where BUNAVAILABLE=0)          
          
   INSERT INTO ACTIONMODULE(IDACTION,IDMODULE)          
   SELECT @IDACTION,ID FROM inserted A where BUNAVAILABLE=1          
   and NOT EXISTS (SELECT * FROM ACTIONMODULE B WITH (NOLOCK) WHERE B.IDACTION=@IDACTION AND B.IDMODULE=A.ID)          
          
  END          
          
IF UPDATE (BINVOICE)              
  BEGIN                 
   SELECT @IDACTION=ID FROM DBO.ACTION WITH (NOLOCK) WHERE VNAME='INVOICE'            
             
   DELETE ACTIONMODULE          
   WHERE IDACTION=@IDACTION          
   AND IDMODULE IN (SELECT ID FROM inserted A where BINVOICE=0)          
          
   INSERT INTO ACTIONMODULE(IDACTION,IDMODULE)          
   SELECT @IDACTION,ID FROM inserted A where BINVOICE=1          
   and NOT EXISTS (SELECT * FROM ACTIONMODULE B WITH (NOLOCK) WHERE B.IDACTION=@IDACTION AND B.IDMODULE=A.ID)          
          
  END          
          
IF UPDATE (BPRINT)              
  BEGIN                 
   SELECT @IDACTION=ID FROM DBO.ACTION WITH (NOLOCK) WHERE VNAME='PRINT'            
             
   DELETE ACTIONMODULE          
   WHERE IDACTION=@IDACTION          
   AND IDMODULE IN (SELECT ID FROM inserted A where BPRINT=0)          
          
   INSERT INTO ACTIONMODULE(IDACTION,IDMODULE)          
   SELECT @IDACTION,ID FROM inserted A where BPRINT=1          
   and NOT EXISTS (SELECT * FROM ACTIONMODULE B WITH (NOLOCK) WHERE B.IDACTION=@IDACTION AND B.IDMODULE=A.ID)          
          
  END         
          
IF UPDATE (BSINC)              
  BEGIN                 
   SELECT @IDACTION=ID FROM DBO.ACTION WITH (NOLOCK) WHERE VNAME='SINC'            
             
   DELETE ACTIONMODULE          
   WHERE IDACTION=@IDACTION          
   AND IDMODULE IN (SELECT ID FROM inserted A where BSINC=0)          
          
   INSERT INTO ACTIONMODULE(IDACTION,IDMODULE)          
   SELECT @IDACTION,ID FROM inserted A where BSINC=1          
   and NOT EXISTS (SELECT * FROM ACTIONMODULE B WITH (NOLOCK) WHERE B.IDACTION=@IDACTION AND B.IDMODULE=A.ID)          
          
  END         
        
IF UPDATE (BPRIORITIZE)              
  BEGIN                 
   SELECT @IDACTION=ID FROM DBO.ACTION WITH (NOLOCK) WHERE VNAME='PRIORITIZE'            
             
   DELETE ACTIONMODULE          
   WHERE IDACTION=@IDACTION          
   AND IDMODULE IN (SELECT ID FROM inserted A where BPRIORITIZE=0)          
          
   INSERT INTO ACTIONMODULE(IDACTION,IDMODULE)          
   SELECT @IDACTION,ID FROM inserted A where BPRIORITIZE=1          
   and NOT EXISTS (SELECT * FROM ACTIONMODULE B WITH (NOLOCK) WHERE B.IDACTION=@IDACTION AND B.IDMODULE=A.ID)          
          
  END   
          
END      