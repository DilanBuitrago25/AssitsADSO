--Procedimientos de la BD 'BDAssistsADSO'

create proc ValidarUsuarios(
@Correo varchar(100),
@Contraseña varchar(100)
)

as

begin

if(exists(select * from Usuario where Correo_usuario = @Correo and Contrasena_usuario = @Contraseña))
select Tipo_usuario from Usuario where Correo_usuario = @Correo and Contrasena_Usuario = @Contraseña
else
select '0'
end

