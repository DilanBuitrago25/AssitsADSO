--Procedimientos de la BD 'BDAssistsADSO'

create proc ValidarUsuarios(
@Correo varchar(100),
@Contrase�a varchar(100)
)

as

begin

if(exists(select * from Usuario where Correo_usuario = @Correo and Contrasena_usuario = @Contrase�a))
select Tipo_usuario from Usuario where Correo_usuario = @Correo and Contrasena_Usuario = @Contrase�a
else
select '0'
end

