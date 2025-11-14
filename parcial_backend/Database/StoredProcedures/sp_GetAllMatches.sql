CREATE OR REPLACE PROCEDURE sp_getallmatches(INOUT result refcursor DEFAULT 'mycursor')
LANGUAGE plpgsql
AS $$
BEGIN
    OPEN result FOR
    SELECT id, "Fecha", "EquipoLocalId", "EquipoVisitanteId", "GolesLocal", "GolesVisitante"
    FROM "matches"
    ORDER BY "Fecha" DESC;
END;
$$;