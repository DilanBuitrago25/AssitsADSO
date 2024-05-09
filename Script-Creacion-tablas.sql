--Nombre de la BD (BDAssistsADSO)

--Creación de las tablas

--Base de datos se modifico y se tiene que verificar que los datos impuestos en el modelo y en el controlador esten de la misma secuencia


create table Programa_formacion(
Id_programa int identity (1000,1)not null,
Nombre_programa varchar (100) not null,
Tipo_programa varchar (100) not null,
Duracion_programa varchar (100) not null
primary key (Id_programa))

create table Ficha(
Id_ficha int identity (100000,1) not null,
Jornada_ficha varchar(100) not null,
Modalidad_ficha varchar(100) not null,
tipo_ficha varchar (100) not null,
Id_programa int references Programa_formacion(Id_programa),
primary key (Id_ficha))

--alter table Ficha add Nombre_ficha varchar (200) not null 

create table Usuario(
Id_usuario int identity (1,1) not null,
Tipo_Documento_usuario varchar (200) not null,
Documento_usuario int not null,
Nombre_usuario varchar (55) not null,
Apellido_usuario varchar (55) not null,
Telefono_usuario int not null,
Correo_usuario varchar (100) not null,
Contrasena_usuario varchar (100) not null,
Tipo_usuario Varchar (100),
Tipo_instructor varchar (55),  --quitado el not null
Esinstructormaster_usuario bit default (0),
Id_ficha int references Ficha(Id_ficha),
primary key (Id_usuario))



--alter table Usuario add Esinstructormaster_usuario bit default (0)

--create table Aprendiz(
--Id_aprendiz int identity (1,1) not null,
--Documento_aprendiz int not null,
--Tipo_Documento_aprendiz varchar (200) not null,
--Nombre_aprendiz varchar (45) not null,
--Apellido_aprendiz varchar (45) not null, 
--Telefono_aprendiz int not null,
--Correo_aprendiz varchar (100) not null,
--Numero_ficha int references Ficha(Id_ficha),
--Primary Key (Id_aprendiz))  No aplica

create table Competencia(
Id_competencia int identity (10000,1) not null,
Nombre_competencia varchar (500) not null,
tipo_competencia varchar (100) not null,
Numero_ficha int references Ficha(Id_ficha),
Id_programa int references Programa_formacion(Id_programa),
primary key (ID_competencia))

create table Asistencia(
Id_asistencia int identity (100000,1) not null,
Tipo_asistencia varchar (45) not null,
fecha_asistencia varchar (45) not null,
Hora_asistencia varchar (45) not null,
Id_usuario int references Usuario(Id_usuario),
Primary Key (Id_asistencia))

create table Soporte(
Id_soporte int identity (1000000,1) not null,
Nombre_soporte varchar (45) not null,
Descripcion_soporte varchar (500) not null,
Tipo_soporte varchar (100) not null,
Id_asistencia int references Asistencia(Id_asistencia),
primary key (Id_soporte))

create table Reporte(
Id_reporte int identity (10000000,1)not null,
Nombre_reporte varchar (45) not null,
Descripcion_reporte varchar (500) not null,
Tipo_reporte varchar (45) not null,
Id_asistencia int references Asistencia(Id_asistencia),
primary key (Id_reporte))