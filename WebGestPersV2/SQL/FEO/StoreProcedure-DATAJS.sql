USE [DipendentiDB]
GO
/****** Object:  StoredProcedure [dbo].[sp_GetOrganigramma_DATAJS]    Script Date: 02/07/2026 08:25:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/* vers. 1.7.8 (full, corretta) --funziona
   - chart a 4 livelli (Root -> Liv1 -> Liv2 -> Liv3)
   - manager selezionato da Incarichi con filtro: Tipo_incarichi.livello >= 55
   - placeholder incarichi:
       * ID_Uff2 = 119 => stop a Liv1 (Eff_Uff2=0, Eff_Uff3=0)
       * ID_Uff3 = 195 => stop a Liv2 (Eff_Uff3=0)
   - Canon_Uff1 per allineare ID_Uff1 usando Liv2Valid quando Eff_Uff2 > 0
   - people/assignments basati su incarichi mappati ai dipartimenti
   - people.name: Gradi.SiglaGrado (militari) o Titoli.Sigla_titolo (civili) + Cognome Nome
   - photo: PhotoToRank.photo_key (match su grado o titolo)
   - output: var INPUT_DATA = ... ; var UPDATED_ON="dd-MM-yyyy";
   - inserita email
   - inserito phone
*/
ALTER   PROCEDURE [dbo].[sp_GetOrganigramma_DATAJS]
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @json  nvarchar(max);
    DECLARE @today varchar(10) = CONVERT(varchar(10), GETDATE(), 105); -- dd-MM-yyyy

    ;WITH
    Liv2Valid AS (
        SELECT ID_Uff1, ID_Uff2
        FROM Liv2Uff
        WHERE SgUff2 <> 'N/A'
          AND DescUff2 <> 'N/A'
    ),

    /* =====================================================
       1) DIPARTIMENTI NORMALIZZATI (Root + Liv1 + Liv2 + Liv3)
       ===================================================== */
    Departments AS (
        /* ROOT (11) */
        SELECT
            id = CAST(l1.ID_Uff1 AS varchar(50)),
            parent_id = NULL,
            name = l1.DescUff1,
            staff_department = CASE WHEN l1.livello = 0 THEN 'Y' ELSE 'N' END,
            livello = l1.livello,
            sigla = l1.SgUff1,
            ID_Uff1 = l1.ID_Uff1,
            ID_Uff2 = CAST(NULL AS int),
            ID_Uff3 = CAST(NULL AS int)
        FROM Liv1Uff l1
        WHERE l1.ID_Uff1 = 11
          AND l1.SgUff1 <> 'N/A'
          AND l1.DescUff1 <> 'N/A'

        UNION ALL

        /* LIV1 (tutti gli altri Liv1 figli di 11) */
        SELECT
            id = CAST(l1.ID_Uff1 AS varchar(50)),
            parent_id = '11',
            name = l1.DescUff1,
            staff_department = CASE WHEN l1.livello = 0 THEN 'Y' ELSE 'N' END,
            livello = l1.livello,
            sigla = l1.SgUff1,
            ID_Uff1 = l1.ID_Uff1,
            ID_Uff2 = CAST(NULL AS int),
            ID_Uff3 = CAST(NULL AS int)
        FROM Liv1Uff l1
        WHERE l1.ID_Uff1 <> 11
          AND l1.SgUff1 <> 'N/A'
          AND l1.DescUff1 <> 'N/A'

        UNION ALL

        /* LIV2 */
        SELECT
            id = CAST(l2.ID_Uff1 AS varchar(50)) + '_' + CAST(l2.ID_Uff2 AS varchar(50)),
            parent_id = CAST(l2.ID_Uff1 AS varchar(50)),
            name = l2.DescUff2,
            staff_department = CASE WHEN l2.livello = 0 THEN 'Y' ELSE 'N' END,
            livello = l2.livello,
            sigla = l2.SgUff2,
            ID_Uff1 = l2.ID_Uff1,
            ID_Uff2 = l2.ID_Uff2,
            ID_Uff3 = CAST(NULL AS int)
        FROM Liv2Uff l2
        WHERE l2.SgUff2 <> 'N/A'
          AND l2.DescUff2 <> 'N/A'

        UNION ALL

        /* LIV3 (padre risolto su Liv2Valid) */
        SELECT
            id =
                CAST(ISNULL(p.ID_Uff1_padre, l3.ID_Uff1) AS varchar(50)) + '_' +
                CAST(l3.ID_Uff2 AS varchar(50)) + '_' +
                CAST(l3.ID_Uff3 AS varchar(50)),
            parent_id =
                CAST(ISNULL(p.ID_Uff1_padre, l3.ID_Uff1) AS varchar(50)) + '_' +
                CAST(l3.ID_Uff2 AS varchar(50)),
            name = l3.DescUff3,
            staff_department = CASE WHEN l3.livello = 0 THEN 'Y' ELSE 'N' END,
            livello = l3.livello,
            sigla = l3.SgUff3,
            ID_Uff1 = ISNULL(p.ID_Uff1_padre, l3.ID_Uff1),
            ID_Uff2 = l3.ID_Uff2,
            ID_Uff3 = l3.ID_Uff3
        FROM Liv3Uff l3
        OUTER APPLY (
            SELECT TOP 1 ID_Uff1_padre = l2.ID_Uff1
            FROM Liv2Valid l2
            WHERE l2.ID_Uff2 = l3.ID_Uff2
            ORDER BY
                CASE WHEN l2.ID_Uff1 = l3.ID_Uff1 THEN 0 ELSE 1 END,
                l2.ID_Uff1
        ) p
        WHERE l3.SgUff3 <> 'N/A'
          AND l3.DescUff3 <> 'N/A'
    ),

    /* =====================================================
       2) MANAGER PER DIPARTIMENTO (ti.livello >= 55)
          + placeholder + Canon_Uff1
       ===================================================== */
    DeptWithManager AS (
        SELECT
            d.*,
            m.IDPersonale AS manager_id,
            manager_name = ISNULL(mgrname.manager_name, 'sede vacante')
        FROM Departments d
        OUTER APPLY (
            SELECT TOP 1 i.IDPersonale
            FROM Incarichi i
            JOIN Tipo_incarichi ti
              ON ti.id_tipo_incarico = i.id_tipo_incarico

            /* placeholder effettivi */
            OUTER APPLY (
                SELECT
                    Eff_Uff2 =
                        CASE
                            WHEN ISNULL(NULLIF(i.ID_Uff2,0),0) = 119 THEN 0
                            ELSE ISNULL(NULLIF(i.ID_Uff2,0),0)
                        END,
                    Eff_Uff3 =
                        CASE
                            WHEN ISNULL(NULLIF(i.ID_Uff2,0),0) = 119 THEN 0
                            WHEN ISNULL(NULLIF(i.ID_Uff3,0),0) = 195 THEN 0
                            ELSE ISNULL(NULLIF(i.ID_Uff3,0),0)
                        END
            ) eff

            /* Canon_Uff1 (allineo ID_Uff1 al padre reale quando Eff_Uff2>0) */
            OUTER APPLY (
                SELECT
                    Canon_Uff1 =
                        CASE
                            WHEN eff.Eff_Uff2 = 0 THEN i.ID_Uff1
                            ELSE ISNULL(p.ID_Uff1_padre, i.ID_Uff1)
                        END
                FROM (SELECT 1 AS dummy) z
                OUTER APPLY (
                    SELECT TOP 1 ID_Uff1_padre = l2.ID_Uff1
                    FROM Liv2Valid l2
                    WHERE l2.ID_Uff2 = i.ID_Uff2
                    ORDER BY
                        CASE WHEN l2.ID_Uff1 = i.ID_Uff1 THEN 0 ELSE 1 END,
                        l2.ID_Uff1
                ) p
            ) can

            WHERE ti.livello >= 55
              AND can.Canon_Uff1 = d.ID_Uff1
              AND eff.Eff_Uff2   = ISNULL(ISNULL(d.ID_Uff2,0),0)
              AND eff.Eff_Uff3   = ISNULL(ISNULL(d.ID_Uff3,0),0)
            ORDER BY
                ISNULL(i.principale,0) DESC,
                ISNULL(ti.livello,0) DESC,
                ISNULL(i.Data_inizio,'19000101') DESC,
                ISNULL(i.id_incarico,0) DESC
        ) m
        OUTER APPLY (
            SELECT
                manager_name =
                    LTRIM(RTRIM(
                        CASE
                            WHEN epm.IDPersonale IS NULL THEN NULL
                            WHEN epm.Militare = 1
                                THEN ISNULL(gm.SiglaGrado + ' ', '') + epm.Cognome + ' ' + epm.Nome
                            ELSE
                                ISNULL(titm.Sigla_titolo + ' ', '') + epm.Cognome + ' ' + epm.Nome
                        END
                    ))
            FROM ElencoPersonale epm
            LEFT JOIN Profilo_militare pmm ON pmm.IDPersonale = epm.IDPersonale
            LEFT JOIN Gradi gm            ON gm.ID_Grado = pmm.ID_Grado
            LEFT JOIN PersCivile pcm      ON pcm.IDPersonale = epm.IDPersonale
            LEFT JOIN dbo.Titoli titm     ON titm.ID_Titolo = pcm.ID_TitoloAtt
            WHERE epm.IDPersonale = m.IDPersonale
        ) mgrname
    ),

    /* =====================================================
       3) INCARICHI MAPPATI (per people/assignments)
          + placeholder + Canon_Uff1
       ===================================================== */
    IncarichiMapped AS (
        SELECT
            d.id AS department_id,
            i.IDPersonale,
            i.principale,
            i.Data_inizio,
            i.id_incarico,
            ti.livello AS livello_tipo,
            ti.Sigla_incarico,
            ti.Descr_incarico
        FROM Incarichi i
        JOIN Tipo_incarichi ti
          ON ti.id_tipo_incarico = i.id_tipo_incarico
        JOIN ElencoPersonale ep
          ON ep.IDPersonale = i.IDPersonale
         AND ep.Stato_Servizio = 'Attivo'

        OUTER APPLY (
            SELECT
                Eff_Uff2 =
                    CASE
                        WHEN ISNULL(NULLIF(i.ID_Uff2,0),0) = 119 THEN 0
                        ELSE ISNULL(NULLIF(i.ID_Uff2,0),0)
                    END,
                Eff_Uff3 =
                    CASE
                        WHEN ISNULL(NULLIF(i.ID_Uff2,0),0) = 119 THEN 0
                        WHEN ISNULL(NULLIF(i.ID_Uff3,0),0) = 195 THEN 0
                        ELSE ISNULL(NULLIF(i.ID_Uff3,0),0)
                    END
        ) eff

        OUTER APPLY (
            SELECT
                Canon_Uff1 =
                    CASE
                        WHEN eff.Eff_Uff2 = 0 THEN i.ID_Uff1
                        ELSE ISNULL(p.ID_Uff1_padre, i.ID_Uff1)
                    END
            FROM (SELECT 1 AS dummy) z
            OUTER APPLY (
                SELECT TOP 1 ID_Uff1_padre = l2.ID_Uff1
                FROM Liv2Valid l2
                WHERE l2.ID_Uff2 = i.ID_Uff2
                ORDER BY
                    CASE WHEN l2.ID_Uff1 = i.ID_Uff1 THEN 0 ELSE 1 END,
                    l2.ID_Uff1
            ) p
        ) can

        JOIN Departments d
          ON d.ID_Uff1 = can.Canon_Uff1
         AND ISNULL(ISNULL(d.ID_Uff2,0),0) = eff.Eff_Uff2
         AND ISNULL(ISNULL(d.ID_Uff3,0),0) = eff.Eff_Uff3
    ),

    Assignments AS (
        SELECT
            department_id,
            id        = IDPersonale,
            person_id = IDPersonale,
            role      = Sigla_incarico
        FROM IncarichiMapped
    ),

    People AS (
    SELECT
        ep.IDPersonale AS id,
        CASE
            WHEN ep.Militare = 1
                THEN ISNULL(g.SiglaGrado + ' ', '') + ep.Cognome + ' ' + ep.Nome
            ELSE ISNULL(tit.Sigla_titolo + ' ', '') + ep.Cognome + ' ' + ep.Nome
        END AS name,
        pmr.Descr_incarico AS functionName,
        ISNULL(ptr.photo_key,'') AS photo,
        ep.TelefonoUfficio AS Phone,

        Email =
			ISNULL(
				NULLIF(
					STUFF((
						SELECT ', ' + cp.Denominazione
						FROM dbo.Cassette_Postali cp
						WHERE cp.IDPersonale = ep.IDPersonale
						  AND NULLIF(LTRIM(RTRIM(cp.Denominazione)),'') IS NOT NULL
						ORDER BY cp.Denominazione
						FOR XML PATH(''), TYPE
					).value('.','nvarchar(max)'), 1, 2, ''),
				''),
			'')
    FROM (
        SELECT
            IDPersonale,
            Descr_incarico,
            rn = ROW_NUMBER() OVER (
                PARTITION BY IDPersonale
                ORDER BY
                    ISNULL(principale,0) DESC,
                    ISNULL(livello_tipo,0) DESC,
                    ISNULL(Data_inizio,'19000101') DESC,
                    ISNULL(id_incarico,0) DESC
            )
        FROM IncarichiMapped
    ) pmr
    JOIN ElencoPersonale ep
      ON ep.IDPersonale = pmr.IDPersonale
     AND ep.Stato_Servizio = 'Attivo'
    LEFT JOIN Profilo_militare pm
      ON pm.IDPersonale = ep.IDPersonale
    LEFT JOIN Gradi g
      ON g.ID_Grado = pm.ID_Grado
    LEFT JOIN PersCivile pc
      ON pc.IDPersonale = ep.IDPersonale
    LEFT JOIN dbo.Titoli tit
      ON tit.ID_Titolo = pc.ID_TitoloAtt
    OUTER APPLY (
        SELECT TOP 1 p2r.photo_key
        FROM dbo.PhotoToRank p2r
        WHERE
            (ep.Militare = 1 AND p2r.id_Grado  = pm.ID_Grado)
            OR
            (ep.Militare = 0 AND p2r.id_Titolo = pc.ID_TitoloAtt)
        ORDER BY p2r.ID DESC
    ) ptr
    WHERE pmr.rn = 1
)


    /* =====================================================
       4) JSON finale
       ===================================================== */
    SELECT @json =
    (
        SELECT
            '2.0' AS api_version,

            /* assignments */
            JSON_QUERY(
                ISNULL((SELECT * FROM Assignments FOR JSON PATH), '[]')
            ) AS assignments,

            /* chart object */
            JSON_QUERY(
            (
                SELECT
                    r.id,
                    r.name,
                    r.parent_id,
                    r.manager_id,
                    r.manager_name,
                    r.staff_department,
                    CAST(1 AS bit) AS showChildren,

                    /* ===== LIV1 ===== */
                    JSON_QUERY(
                        ISNULL(
                        (
                            SELECT
                                l1.id,
                                l1.name,
                                l1.parent_id,
                                l1.manager_id,
                                l1.manager_name,
                                l1.staff_department,
                                CAST(1 AS bit) AS showChildren,

                                /* ===== LIV2 ===== */
                                JSON_QUERY(
                                    ISNULL(
                                    (
                                        SELECT
                                            l2.id,
                                            l2.name,
                                            l2.parent_id,
                                            l2.manager_id,
                                            l2.manager_name,
                                            l2.staff_department,
                                            CAST(1 AS bit) AS showChildren,

                                            /* ===== LIV3 ===== */
                                            JSON_QUERY(
                                                ISNULL(
                                                (
                                                    SELECT
                                                        l3.id,
                                                        l3.name,
                                                        l3.parent_id,
                                                        l3.manager_id,
                                                        l3.manager_name,
                                                        l3.staff_department,
                                                        CAST(1 AS bit) AS showChildren,
                                                        JSON_QUERY('[]') AS children
                                                    FROM DeptWithManager l3
                                                    WHERE l3.parent_id = l2.id
                                                    ORDER BY
                                                        CASE WHEN l3.livello = 0 THEN 999999 ELSE l3.livello END,
                                                        l3.sigla
                                                    FOR JSON PATH
                                                ),
                                                '[]'
                                                )
                                            ) AS children

                                        FROM DeptWithManager l2
                                        WHERE l2.parent_id = l1.id
                                        ORDER BY
                                            CASE WHEN l2.livello = 0 THEN 999999 ELSE l2.livello END,
                                            l2.sigla
                                        FOR JSON PATH
                                    ),
                                    '[]'
                                    )
                                ) AS children

                            FROM DeptWithManager l1
                            WHERE l1.parent_id = r.id
                            ORDER BY
                                CASE WHEN l1.livello = 0 THEN 999999 ELSE l1.livello END,
                                l1.sigla
                            FOR JSON PATH
                        ),
                        '[]'
                        )
                    ) AS children

                FROM DeptWithManager r
                WHERE r.parent_id IS NULL
                FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
            )
            ) AS chart,

            /* people */
            JSON_QUERY(
                ISNULL(
                (
                    SELECT
                        p.id,
                        p.name,
                        p.functionName,
                        p.photo,
                        JSON_QUERY(
                            (
                                SELECT									
									p.Email    AS Email,
									CASE 
										WHEN p.Phone IS NULL OR LTRIM(RTRIM(CONVERT(varchar(50), p.Phone))) = '' THEN NULL
										WHEN ISNUMERIC(CONVERT(varchar(50), p.Phone)) = 1 THEN CONVERT(int, CONVERT(varchar(50), p.Phone))
										ELSE CONVERT(varchar(50), p.Phone)
									END        AS Phone									
								FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
                            )
                        ) AS fields
                    FROM People p
                    FOR JSON PATH
                ),
                '[]'
                )
            ) AS people

        FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
    );

    SET @json = ISNULL(@json, N'{}');

    /* Output finale: JS */
    SELECT
        CONCAT(
            N'var INPUT_DATA = ', @json, N';', CHAR(13), CHAR(10),
            N'var UPDATED_ON = "', @today, N'";'
        ) AS DataJs;
END;
