select * from Asistencia

select * from usuario

select * from Soporte

ALTER TABLE Asistencia 
ADD Estado_Asistencia bit default (1) ;

ALTER TABLE Competencia 
ADD Estado_Competencia bit default (1) ;

ALTER TABLE Ficha 
ADD Estado_Ficha bit default (1) ;

ALTER TABLE Programa_formacion 
ADD Estado_Programa_formacion bit default (1) ;

ALTER TABLE Usuario 
ADD Estado_Usuario bit default (1) ;

ALTER TABLE Soporte 
ADD Estado_Soporte bit default (1) ;

ALTER TABLE RegistroAsistencia
ADD Estado_RegistroAsitencia bit default (1) ;

select * from RegistroAsistencia