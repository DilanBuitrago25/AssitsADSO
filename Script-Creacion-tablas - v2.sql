--Nombre de la BD (BDAssistsADSOv2)

--Creaci�n de las tablas
go
--Base de datos se modifico y se tiene que verificar que los datos impuestos en el modelo y en el controlador esten de la misma secuencia
Create database BDAssistsADSOv2

go

use BDAssistsADSOv2

go

create table Programa_formacion(
Id_programa int identity (1000,1)not null,
Nombre_programa varchar (100) not null,
Tipo_programa varchar (100) not null,
Duracion_programa varchar (100) not null,
Estado_Programa_formacion bit default (1),
primary key (Id_programa))

go

create table Ficha(
Id_ficha int identity (100000,1) not null,
Codigo_ficha int not null,
Jornada_ficha varchar(100) not null,
Modalidad_ficha varchar(100) not null,
tipo_ficha varchar (100) not null,
Fecha_inicio varchar (200) not null,
Fecha_fin varchar (200) not null,
Estado_ficha bit default (1),
Id_programa int references Programa_formacion(Id_programa),
primary key (Id_ficha))

go

create table Fichas_has_Programa_Formacion(
Id_ficha int references Ficha(Id_ficha),
Id_programa int references Programa_formacion(Id_programa))

go

create table Usuario(
Id_usuario int identity (1,1) not null,
Tipo_Documento_usuario varchar (200) not null,
Documento_usuario int not null,
Nombre_usuario varchar (55) not null,
Apellido_usuario varchar (55) not null,
Telefono_usuario numeric (18, 0),
Correo_usuario varchar (100) not null,
Contrasena_usuario varchar (100) not null,
Tipo_usuario Varchar (100),
Tipo_instructor varchar (55),  --quitado el not null
Id_ficha int references Ficha(Id_ficha),
Estado_Usuario bit default (1),
primary key (Id_usuario))

go

Alter table Programa_formacion
add Id_Usuario int references Usuario(Id_Usuario);

go

Alter table Ficha
add Id_Aprendiz int references Usuario(Id_Usuario),
Id_Instructor int references Usuario(Id_Usuario);

go

create table Competencia(
Id_competencia int identity (10000,1) not null,
Nombre_competencia varchar (500) not null,
tipo_competencia varchar (100) not null,
Id_ficha int references Ficha(Id_ficha),
Id_programa int references Programa_formacion(Id_programa),
Id_Usuario int references Usuario(Id_Usuario),
Estado_Competencia bit default (1),
primary key (ID_competencia))

go

create table Ficha_has_Competencia(
Id_ficha int references Ficha(Id_ficha),
Id_compentencia int references Competencia(Id_competencia))

go

create table Programas_has_Competencia(
Id_programa int references Programa_formacion(Id_programa),
Id_compentencia int references Competencia(Id_competencia))

go

create table Ficha_has_Usuario(
Id_ficha int references Ficha(Id_ficha),
Id_usuario int references Usuario(Id_usuario))

go

create table Asistencia(
Id_asistencia int identity (100000,1) not null,
Fecha_inicio_asistencia varchar(200) not null,
Fecha_fin_asistencia varchar(200) not null,
Hora_inicio_asistencia varchar(200) not null,
Hora_fin_asistencia varchar(200) not null,
Detalles_asistencia varchar (500) not null,
Id_usuario int references Usuario(Id_usuario),
Id_ficha int references Ficha(Id_ficha),
Id_competencia int references Competencia(Id_competencia),
Estado_Asistencia bit default (1),
Primary Key (Id_asistencia))

go

create table RegistroAsistencia(
Id_Registroasistencia int identity (10000000,1) not null,
Fecha_registro varchar(200) not null,
Hora_registro varchar(200) not null,
Asistio_registro bit default (0),
Id_asistencia int references Asistencia(Id_asistencia),
Id_usuario int references Usuario(Id_usuario),
Estado_RegistroAsitencia bit default (1),
Primary Key (Id_Registroasistencia))

go

create table Soporte(
Id_soporte int identity (1000000,1) not null,
Nombre_soporte varchar (100) not null,
Descripcion_soporte varchar (500) not null,
Fecha_registro varchar(200) not null,
Hora_registro varchar(200) not null,
Id_usuario int references Usuario(Id_usuario),
Id_Instructor int references Usuario(Id_usuario),
Id_asistencia int references Asistencia(Id_asistencia), 
Archivo_soporte varchar(max),
Estado_Soporte bit default (1),
primary key (Id_soporte))


