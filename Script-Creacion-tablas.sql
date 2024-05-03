--Nombre de la BD (BDAssistsADSO)

--Creación de las tablas



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

create table Instructor(
Id_instructor int identity (1,1) not null,
Documento_instructor int not null,
Tipo_Documento_instructor varchar (200) not null,
Nombre_instructor varchar (55) not null,
Apellido_instructor varchar (55) not null,
Telefono_instructor int not null,
Correo_instructor varchar (100) not null,
Tipo_instructor varchar (55) not null,
Id_ficha int references Ficha(Id_ficha),
primary key (Id_instructor))

create table Competencia(
ID_competencia int identity (10000,1) not null,
tipo_competencia varchar (100) not null,
Numero_ficha int references Ficha(Id_ficha),
Id_programa int references Programa_formacion(Id_programa),
primary key (ID_competencia))

create table Aprendiz(
Id_aprendiz int identity (1,1) not null,
Documento_aprendiz int not null,
Tipo_Documento_aprendiz varchar (200) not null,
Nombre_aprendiz varchar (45) not null,
Apellido_aprendiz varchar (45) not null, 
Telefono_aprendiz int not null,
Correo_aprendiz varchar (100) not null,
Numero_ficha int references Ficha(Id_ficha),
Primary Key (Id_aprendiz))

create table Asistencia(
Id_asistencia int identity (100000,1) not null,
Tipo_asistencia varchar (45) not null,
fecha_asistencia varchar (45) not null,
Hora_asistencia varchar (45) not null,
Id_aprendiz int references Aprendiz(Id_aprendiz),
Primary Key (Id_asistencia))

create table Soporte(
Id_soporte int identity (1000000,1) not null,
Nombre_soporte varchar (45) not null,
Descripcion_soporte varchar (500) not null,
Id_asistencia int references Asistencia(Id_asistencia),
primary key (Id_soporte))

create table Reporte(
Id_reporte int identity (10000000,1)not null,
Nombre_reporte varchar (45) not null,
Descripcion_reporte varchar (500) not null,
Tipo_reporte varchar (45) not null,
Id_asistencia int references Asistencia(Id_asistencia),
primary key (Id_reporte))