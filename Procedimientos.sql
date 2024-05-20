--Procedimientos de la BD 'BDAssistsADSO'

----------

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

exec ValidarUsuarios 'juliobuitrago@gmail.com', 'Julio12345'

select * from Usuario



