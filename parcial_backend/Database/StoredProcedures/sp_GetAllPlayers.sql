CREATE OR REPLACE PROCEDURE sp_getallplayers(INOUT result refcursor DEFAULT 'mycursor')
LANGUAGE plpgsql
AS $$
BEGIN
    OPEN result FOR
    SELECT id, "Nombre", "Posicion", "Edad", "EquipoId"
    FROM "players"
    ORDER BY "Nombre";
END;
$$;