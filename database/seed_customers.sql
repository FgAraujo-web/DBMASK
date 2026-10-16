
/*
    DBMASK - Dados de demonstração
    Autora: Flórida Araújo

    Objetivo:
    Inserir clientes fictícios para testar
    o mascaramento por perfil de acesso.

    Executar após a criação da base de dados
    através das EF Core Migrations.

    Script idempotente:
    evita inserir novamente os mesmos NIFs.
*/

USE DBMASK_Platform;
GO

SET NOCOUNT ON;

INSERT INTO dbo.Customers
    (Name, Nif, Email, Phone)
SELECT
    v.Name,
    v.Nif,
    v.Email,
    v.Phone
FROM
(
    VALUES
    (
        N'Ana Martins',
        '900000001',
        'ana.martins@example.com',
        '910000001'
    ),
    (
        N'Bruno Ferreira',
        '900000002',
        'bruno.ferreira@example.com',
        '910000002'
    ),
    (
        N'Carla Rodrigues',
        '900000003',
        'carla.rodrigues@example.com',
        '910000003'
    ),
    (
        N'Diogo Almeida',
        '900000004',
        'diogo.almeida@example.com',
        '910000004'
    ),
    (
        N'Elisa Pereira',
        '900000005',
        'elisa.pereira@example.com',
        '910000005'
    ),
    (
        N'Filipe Costa',
        '900000006',
        'filipe.costa@example.com',
        '910000006'
    ),
    (
        N'Gabriela Sousa',
        '900000007',
        'gabriela.sousa@example.com',
        '910000007'
    ),
    (
        N'Hugo Ribeiro',
        '900000008',
        'hugo.ribeiro@example.com',
        '910000008'
    )
) AS v (Name, Nif, Email, Phone)
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.Customers AS c
    WHERE c.Nif = v.Nif
);

PRINT 'Dados fictícios inseridos ou já existentes.';
GO

SELECT
    Id,
    Name,
    Nif,
    Email,
    Phone
FROM dbo.Customers
WHERE Nif BETWEEN '900000001' AND '900000008'
ORDER BY Id;
