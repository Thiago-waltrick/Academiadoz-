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
        logradouro_cep VARCHAR(8) NULL,
        foto_conteudo VARBINARY(MAX) NULL
    );
END
GO

IF COL_LENGTH('dbo.tb_aluno', 'foto_conteudo') IS NULL
BEGIN
    ALTER TABLE dbo.tb_aluno ADD foto_conteudo VARBINARY(MAX) NULL;
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
        plano INT NOT NULL,
        data_inicio DATE NOT NULL,
        data_fim DATE NOT NULL,
        objetivo VARCHAR(200) NULL,
        restricoes INT NOT NULL CONSTRAINT DF_tb_matricula_restricoes DEFAULT (0),
        obs_restricao VARCHAR(500) NULL,
        laudo_nome VARCHAR(255) NULL,
        laudo_content_type VARCHAR(100) NULL,
        laudo_conteudo VARBINARY(MAX) NULL
    );
END
GO

-- Adiciona os campos de Matrícula sem apagar dados existentes.
IF COL_LENGTH('dbo.tb_matricula', 'objetivo') IS NULL
BEGIN
    ALTER TABLE dbo.tb_matricula ADD objetivo VARCHAR(200) NULL;
END
GO

IF COL_LENGTH('dbo.tb_matricula', 'restricoes') IS NULL
BEGIN
    ALTER TABLE dbo.tb_matricula ADD restricoes INT NOT NULL CONSTRAINT DF_tb_matricula_restricoes DEFAULT (0);
END
GO

IF COL_LENGTH('dbo.tb_matricula', 'obs_restricao') IS NULL
BEGIN
    ALTER TABLE dbo.tb_matricula ADD obs_restricao VARCHAR(500) NULL;
END
GO

IF COL_LENGTH('dbo.tb_matricula', 'laudo_nome') IS NULL
BEGIN
    ALTER TABLE dbo.tb_matricula ADD laudo_nome VARCHAR(255) NULL;
END
GO

IF COL_LENGTH('dbo.tb_matricula', 'laudo_content_type') IS NULL
BEGIN
    ALTER TABLE dbo.tb_matricula ADD laudo_content_type VARCHAR(100) NULL;
END
GO

IF COL_LENGTH('dbo.tb_matricula', 'laudo_conteudo') IS NULL
BEGIN
    ALTER TABLE dbo.tb_matricula ADD laudo_conteudo VARBINARY(MAX) NULL;
END
GO

-- Os planos antigos usavam 0..3; converte uma vez para o contrato 1..4.
IF EXISTS (SELECT 1 FROM dbo.tb_matricula WHERE plano = 0)
BEGIN
    UPDATE dbo.tb_matricula
    SET plano = CASE WHEN plano BETWEEN 0 AND 3 THEN plano + 1 ELSE plano END;
END
GO

UPDATE dbo.tb_matricula SET plano = 1 WHERE plano IS NULL OR plano NOT BETWEEN 1 AND 4;
UPDATE dbo.tb_matricula SET data_inicio = CONVERT(DATE, GETDATE()) WHERE data_inicio IS NULL;
UPDATE dbo.tb_matricula
SET data_fim = CASE plano
    WHEN 1 THEN DATEADD(MONTH, 1, CONVERT(DATE, data_inicio))
    WHEN 2 THEN DATEADD(MONTH, 3, CONVERT(DATE, data_inicio))
    WHEN 3 THEN DATEADD(MONTH, 6, CONVERT(DATE, data_inicio))
    ELSE DATEADD(YEAR, 1, CONVERT(DATE, data_inicio))
END
WHERE data_fim IS NULL;
GO

ALTER TABLE dbo.tb_matricula ALTER COLUMN plano INT NOT NULL;
ALTER TABLE dbo.tb_matricula ALTER COLUMN data_inicio DATE NOT NULL;
ALTER TABLE dbo.tb_matricula ALTER COLUMN data_fim DATE NOT NULL;
ALTER TABLE dbo.tb_matricula ALTER COLUMN objetivo VARCHAR(200) NULL;
ALTER TABLE dbo.tb_matricula ALTER COLUMN obs_restricao VARCHAR(500) NULL;
GO

IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = 'CK_tb_matricula_plano')
BEGIN
    ALTER TABLE dbo.tb_matricula
        ADD CONSTRAINT CK_tb_matricula_plano CHECK (plano BETWEEN 1 AND 4);
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

-- 6) Execução de treinos
IF OBJECT_ID('dbo.tb_treino_sessao', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.tb_treino_sessao (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        iniciada_em DATETIME2 NOT NULL,
        concluida_em DATETIME2 NULL
    );
END
GO

IF NOT EXISTS(SELECT 1 FROM sys.indexes WHERE name = 'ux_treino_sessao_ativa' AND object_id = OBJECT_ID('dbo.tb_treino_sessao'))
BEGIN
    CREATE UNIQUE INDEX ux_treino_sessao_ativa ON dbo.tb_treino_sessao(concluida_em) WHERE concluida_em IS NULL;
END
GO

IF OBJECT_ID('dbo.tb_treino_exercicio', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.tb_treino_exercicio (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        sessao_id INT NOT NULL,
        nome NVARCHAR(200) NOT NULL,
        CONSTRAINT fk_treino_exercicio_sessao FOREIGN KEY (sessao_id) REFERENCES dbo.tb_treino_sessao(Id)
    );
END
GO

IF NOT EXISTS(SELECT 1 FROM sys.indexes WHERE name = 'idx_treino_exercicio_sessao' AND object_id = OBJECT_ID('dbo.tb_treino_exercicio'))
BEGIN
    CREATE INDEX idx_treino_exercicio_sessao ON dbo.tb_treino_exercicio(sessao_id);
END
GO

IF OBJECT_ID('dbo.tb_treino_serie', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.tb_treino_serie (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        exercicio_id INT NOT NULL,
        numero INT NOT NULL,
        carga_kg DECIMAL(8,2) NOT NULL,
        repeticoes INT NOT NULL,
        concluida BIT NOT NULL CONSTRAINT df_treino_serie_concluida DEFAULT(0),
        CONSTRAINT fk_treino_serie_exercicio FOREIGN KEY (exercicio_id) REFERENCES dbo.tb_treino_exercicio(Id),
        CONSTRAINT uq_treino_serie_numero UNIQUE (exercicio_id, numero),
        CONSTRAINT ck_treino_serie_numero CHECK (numero > 0),
        CONSTRAINT ck_treino_serie_carga CHECK (carga_kg >= 0),
        CONSTRAINT ck_treino_serie_repeticoes CHECK (repeticoes > 0)
    );
END
GO
