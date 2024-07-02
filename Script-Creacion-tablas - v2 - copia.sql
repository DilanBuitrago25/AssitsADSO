--Nombre de la BD (BDAssistsADSOv4)

--Creación de las tablas
go
--Base de datos se modifico y se tiene que verificar que los datos impuestos en el modelo y en el controlador esten de la misma secuencia
Create database BDAssistsADSOv4

go

use BDAssistsADSOv4

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
primary key (Id_ficha));

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
Id_ficha int references Ficha(Id_ficha),
Estado_Usuario bit default (1),
primary key (Id_usuario))

go

Insert into Usuario (Tipo_Documento_usuario, Documento_usuario, Nombre_usuario, Apellido_usuario, Telefono_usuario, Correo_usuario, Contrasena_usuario, Tipo_usuario, Estado_Usuario) values
('C.C', 11111111, 'User', 'Admin', 0000000000, 'admin@soy.sena.edu.co', 'admin123', 'InstructorAdmin', 1)


go

Alter table Ficha
add Id_Instructor int references Usuario(Id_Usuario);

go


create table Competencia(
Id_competencia int identity (10000,1) not null,
Nombre_competencia varchar (500) not null,
tipo_competencia varchar (100) not null,
Estado_Competencia bit default (1),
primary key (ID_competencia))

go

CREATE TABLE Programas_has_Competencia (
    Id_programa INT  ,
    Id_competencia INT  ,
    CONSTRAINT PK_Programas_has_Competencia PRIMARY KEY (Id_programa, Id_competencia),
    CONSTRAINT FK_Programas_has_Competencia_Programa FOREIGN KEY (Id_programa) REFERENCES Programa_formacion(Id_programa),
    CONSTRAINT FK_Programas_has_Competencia_Competencia FOREIGN KEY (Id_competencia) REFERENCES Competencia(Id_competencia)
);


go

CREATE TABLE Ficha_has_Aprendices (
    Id_ficha INT  ,
    Id_Aprendiz INT  ,
    CONSTRAINT PK_Ficha_has_Usuario PRIMARY KEY (Id_ficha, Id_Aprendiz),
    CONSTRAINT FK_Ficha_has_Usuario_Ficha FOREIGN KEY (Id_ficha) REFERENCES Ficha(Id_ficha),
    CONSTRAINT FK_Ficha_has_Usuario_Usuario FOREIGN KEY (Id_Aprendiz) REFERENCES Usuario(Id_usuario)
);


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
Estado_Asistencia bit default (1),
QrCode varchar(max),
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


