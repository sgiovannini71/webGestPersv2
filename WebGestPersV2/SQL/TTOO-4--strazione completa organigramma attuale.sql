------------------------------------------------------------
-- ORGANIGRAMMA COMPLETO ATTUALE
------------------------------------------------------------
SELECT
    -- LIVELLO 1
    L1.ID_Uff1,
    L1.SgUff1,
    L1.DescUff1,
    L1.livello AS Livello1,
    L1.competenze AS Competenze1,
    L1.Attivo,

    -- LIVELLO 2
    L2.ID_Uff2,
    L2.SgUff2,
    L2.DescUff2,
    L2.livello AS Livello2,
    L2.competenze AS Competenze2,

    -- LIVELLO 3
    L3.ID_Uff3,
    L3.SgUff3,
    L3.DescUff3,
    L3.livello AS Livello3,
    L3.competenze AS Competenze3
FROM dbo.Liv1Uff L1
LEFT JOIN dbo.Liv2Uff L2
    ON L2.ID_Uff1 = L1.ID_Uff1
LEFT JOIN dbo.Liv3Uff L3
    ON L3.ID_Uff1 = L2.ID_Uff1
   AND L3.ID_Uff2 = L2.ID_Uff2
ORDER BY
    L1.livello,
    L1.SgUff1,
    L2.livello,
    L2.SgUff2,
    L3.livello,
    L3.SgUff3;