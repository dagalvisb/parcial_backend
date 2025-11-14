CREATE OR REPLACE PROCEDURE sp_getallteams(INOUT result refcursor DEFAULT 'mycursor')
LANGUAGE plpgsql
AS $$
BEGIN
    OPEN result FOR
    SELECT id, "Nombre", "Ciudad", "Estadio", "Fundacion"
    FROM "teams"
    ORDER BY "Nombre";
END;
$$;
