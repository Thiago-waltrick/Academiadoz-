-- Thiago Augusto Ruskowski Waltrick
-- Script de criação para SQL Server
SET NOCOUNT ON;
-- Script de criação que preserva dados existentes: cria tabelas, índices e FKs somente se não existirem.

-- 1) tb_logradouro
IF OBJECT_ID('dbo.tb_logradouro', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.tb_logradouro (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        nome NVARCHAR(200) NOT NULL,
        bairro NVARCHAR(150) NULL,
        cidade NVARCHAR(100) NULL,
        estado NVARCHAR(2) NULL,
        cep VARCHAR(8) NULL
    );
END
GO

IF NOT EXISTS(SELECT 1 FROM sys.indexes WHERE name = 'idx_logradouro_nome' AND object_id = OBJECT_ID('dbo.tb_logradouro'))
BEGIN
    CREATE INDEX idx_logradouro_nome ON dbo.tb_logradouro(nome);
END
GO

-- 2) tb_aluno
IF OBJECT_ID('dbo.tb_aluno', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.tb_aluno (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        nome NVARCHAR(200) NOT NULL,
        data_nascimento DATETIME2 NULL,
        cpf VARCHAR(11) NULL,
        email NVARCHAR(200) NULL,
        telefone VARCHAR(20) NULL,
        end_numero NVARCHAR(20) NULL,
        end_complemento NVARCHAR(200) NULL,
        logradouro_nome NVARCHAR(200) NULL,
        logradouro_bairro NVARCHAR(150) NULL,
        logradouro_cidade NVARCHAR(100) NULL,
        logradouro_estado NVARCHAR(2) NULL,
        logradouro_cep VARCHAR(8) NULL
    );
END
GO

IF NOT EXISTS(SELECT 1 FROM sys.indexes WHERE name = 'idx_aluno_nome' AND object_id = OBJECT_ID('dbo.tb_aluno'))
BEGIN
    CREATE INDEX idx_aluno_nome ON dbo.tb_aluno(nome);
END
GO

-- 3) tb_colaborador
IF OBJECT_ID('dbo.tb_colaborador', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.tb_colaborador (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        nome NVARCHAR(200) NOT NULL,
        data_nascimento DATETIME2 NULL,
        data_admissao DATETIME2 NULL,
        tipo INT NULL,
        vinculo INT NULL,
        cpf VARCHAR(11) NULL,
        email NVARCHAR(200) NULL,
        telefone VARCHAR(20) NULL
    );
END
GO

IF NOT EXISTS(SELECT 1 FROM sys.indexes WHERE name = 'idx_colaborador_nome' AND object_id = OBJECT_ID('dbo.tb_colaborador'))
BEGIN
    CREATE INDEX idx_colaborador_nome ON dbo.tb_colaborador(nome);
END
GO

-- 4) tb_matricula
IF OBJECT_ID('dbo.tb_matricula', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.tb_matricula (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        aluno_id INT NOT NULL,
        plano INT NULL,
        data_inicio DATETIME2 NULL,
        data_fim DATETIME2 NULL
    );
END
GO

-- Adiciona colunas objetivo e obs_restricao se não existirem (sem apagar dados existentes)
IF COL_LENGTH('dbo.tb_matricula', 'objetivo') IS NULL
BEGIN
    ALTER TABLE dbo.tb_matricula ADD objetivo VARCHAR(200) NULL;
END
GO

IF COL_LENGTH('dbo.tb_matricula', 'obs_restricao') IS NULL
BEGIN
    ALTER TABLE dbo.tb_matricula ADD obs_restricao VARCHAR(200) NULL;
END
GO

-- Preencher registros existentes com valores de evidência quando aplicável
UPDATE dbo.tb_matricula SET objetivo = 'Thiago Augusto Ruskowski Waltrick' WHERE objetivo IS NULL;
UPDATE dbo.tb_matricula SET obs_restricao = 'SQLServer' WHERE obs_restricao IS NULL;
GO

-- Tornar objetivo NOT NULL (após preenchimento seguro)
IF COL_LENGTH('dbo.tb_matricula', 'objetivo') IS NOT NULL
BEGIN
    ALTER TABLE dbo.tb_matricula ALTER COLUMN objetivo VARCHAR(200) NOT NULL;
END
GO

IF NOT EXISTS(SELECT 1 FROM sys.foreign_keys WHERE name = 'fk_matricula_aluno')
BEGIN
    ALTER TABLE dbo.tb_matricula
        ADD CONSTRAINT fk_matricula_aluno FOREIGN KEY (aluno_id) REFERENCES dbo.tb_aluno(Id);
END
GO

IF NOT EXISTS(SELECT 1 FROM sys.indexes WHERE name = 'idx_matricula_aluno' AND object_id = OBJECT_ID('dbo.tb_matricula'))
BEGIN
    CREATE INDEX idx_matricula_aluno ON dbo.tb_matricula(aluno_id);
END
GO

-- 5) tb_acesso
IF OBJECT_ID('dbo.tb_acesso', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.tb_acesso (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        aluno_id INT NULL,
        data_acesso DATETIME2 NOT NULL DEFAULT(GETDATE())
    );
END
GO

IF NOT EXISTS(SELECT 1 FROM sys.foreign_keys WHERE name = 'fk_acesso_aluno')
BEGIN
    ALTER TABLE dbo.tb_acesso
        ADD CONSTRAINT fk_acesso_aluno FOREIGN KEY (aluno_id) REFERENCES dbo.tb_aluno(Id);
END
GO

IF NOT EXISTS(SELECT 1 FROM sys.indexes WHERE name = 'idx_acesso_aluno' AND object_id = OBJECT_ID('dbo.tb_acesso'))
BEGIN
    CREATE INDEX idx_acesso_aluno ON dbo.tb_acesso(aluno_id);
END
GO
