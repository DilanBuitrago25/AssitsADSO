select * from RegistroAsistencia
select * from RegistroAsistencia
select * from Usuario
select * from Asistencia
select * from Soporte

CREATE TRIGGER TR_CrearRegistrosAsistencia
ON Asistencia
AFTER INSERT
AS
BEGIN
    DECLARE @Id_asistencia INT;
    SELECT @Id_asistencia = Id_asistencia FROM inserted;

    INSERT INTO RegistroAsistencia (Fecha_registro, Hora_registro, Id_asistencia, Id_usuario, Asistio_registro)
    SELECT 'N/A', 'N/A', @Id_asistencia, Id_usuario, 0 -- Valores predeterminados y calculados
    FROM Usuario
    WHERE Id_ficha = (SELECT Id_ficha FROM Asistencia WHERE Id_asistencia = @Id_asistencia);
END;

