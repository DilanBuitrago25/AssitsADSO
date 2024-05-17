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



--Select * from Usuarios

---PROCEDIMIENTOS 
create proc RegistrarUsuario(
@Tipo_Documento_usuario varchar (200),
@Documento_usuario int,
@Nombre_usuario varchar (55),
@Apellido_usuario varchar (55),
@Telefono_usuario int,
@Correo_usuario varchar (100),
@Contrasena_usuario varchar (100),
@Tipo_usuario Varchar (100),
@Tipo_instructor varchar (55),  
@Registrado bit output,
@Mensaje varchar (200) output
)
as
begin 

if (not exists(select* from Usuario where Correo_usuario = @Correo_usuario))
begin 
      insert into Usuario(Tipo_Documento_usuario,Documento_usuario,Nombre_usuario,Apellido_usuario,Telefono_usuario,Correo_usuario,Contrasena_usuario,Tipo_usuario,Tipo_instructor)
	  values(@Tipo_Documento_usuario,@Documento_usuario,@Nombre_usuario,@Apellido_usuario,@Telefono_usuario,@Correo_usuario,@Contrasena_usuario,@Tipo_usuario,@Tipo_instructor)
	  set @Registrado = 1
	  set @Mensaje = 'Usuario registrado'
	  end 
	  else
	  begin
	  set @Registrado = 0
	  set @Mensaje = 'Este Correo ya existe en el sistema'
	  end

end 

--
create proc ValidarUsuarios(
@Correo_usuario varchar(100),
@Contrasena_usuario  varchar(100)
)

as

begin

if(exists(select * from Usuario where Correo_usuario = @Correo_usuario and Contrasena_usuario = @Contrasena_usuario))
select Tipo_usuario from Usuario where Correo_usuario = @Correo_usuario and Contrasena_Usuario = @Contrasena_usuario
else
select '0'
end


declare @Registrado bit, @Mensaje varchar (200)

exec RegistrarUsuario 'Cedula',12,'Sara', 'Angarita',33,'S.angaritab27@gmail.com','Sara12345','Instructor','Tecnico',@Registrado output, @Mensaje output 
select @Registrado
select @Mensaje

exec ValidarUsuarios 'S.angaritab27@gmail.com', 'Sara12345'

