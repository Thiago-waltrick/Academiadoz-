// Thiago Augusto Ruskowski Waltrick
using System;
using System.IO;
using System.Threading.Tasks;

namespace AcademiaDoZe.Infrastructure.Data
{
    /// <summary>
    /// Executa scripts de criação e carga inicial do banco de dados usando DbProvider.
    /// </summary>
    public static class DbInitializer
    {
        /// <summary>
        /// Adiciona, sem remover dados, as colunas usadas pelo repositório de matrículas.
        /// Compatibiliza bancos criados com versões anteriores do esquema.
        /// </summary>
        public static Task EnsureMatriculaSchemaAsync(DbProvider provider)
        {
            if (provider == null) throw new ArgumentNullException(nameof(provider));

            const string sql = @"
IF EXISTS (SELECT 1 FROM sys.tables WHERE schema_id = SCHEMA_ID(N'dbo') AND name = N'tb_matricula')
BEGIN
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.tb_matricula') AND name = N'restricoes')
        ALTER TABLE dbo.tb_matricula ADD restricoes INT NOT NULL DEFAULT (0);

    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.tb_matricula') AND name = N'laudo_nome')
        ALTER TABLE dbo.tb_matricula ADD laudo_nome VARCHAR(255) NULL;

    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.tb_matricula') AND name = N'laudo_content_type')
        ALTER TABLE dbo.tb_matricula ADD laudo_content_type VARCHAR(100) NULL;

    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.tb_matricula') AND name = N'laudo_conteudo')
        ALTER TABLE dbo.tb_matricula ADD laudo_conteudo VARBINARY(MAX) NULL;
END
ELSE
BEGIN
    ;THROW 50001, 'A tabela dbo.tb_matricula não existe no banco conectado.', 1;
END;

IF EXISTS (SELECT 1 FROM sys.tables WHERE schema_id = SCHEMA_ID(N'dbo') AND name = N'tb_aluno')
BEGIN
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.tb_aluno') AND name = N'foto_conteudo')
        ALTER TABLE dbo.tb_aluno ADD foto_conteudo VARBINARY(MAX) NULL;
END
ELSE
BEGIN
    ;THROW 50002, 'A tabela dbo.tb_aluno não existe no banco conectado.', 1;
END;

IF EXISTS (SELECT 1 FROM sys.tables WHERE schema_id = SCHEMA_ID(N'dbo') AND name = N'tb_colaborador')
BEGIN
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.tb_colaborador') AND name = N'foto_conteudo')
        ALTER TABLE dbo.tb_colaborador ADD foto_conteudo VARBINARY(MAX) NULL;
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.tb_colaborador') AND name = N'end_numero')
        ALTER TABLE dbo.tb_colaborador ADD end_numero NVARCHAR(20) NULL;
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.tb_colaborador') AND name = N'end_complemento')
        ALTER TABLE dbo.tb_colaborador ADD end_complemento NVARCHAR(200) NULL;
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.tb_colaborador') AND name = N'logradouro_nome')
        ALTER TABLE dbo.tb_colaborador ADD logradouro_nome NVARCHAR(200) NULL;
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.tb_colaborador') AND name = N'logradouro_bairro')
        ALTER TABLE dbo.tb_colaborador ADD logradouro_bairro NVARCHAR(150) NULL;
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.tb_colaborador') AND name = N'logradouro_cidade')
        ALTER TABLE dbo.tb_colaborador ADD logradouro_cidade NVARCHAR(100) NULL;
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.tb_colaborador') AND name = N'logradouro_estado')
        ALTER TABLE dbo.tb_colaborador ADD logradouro_estado NVARCHAR(2) NULL;
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.tb_colaborador') AND name = N'logradouro_cep')
        ALTER TABLE dbo.tb_colaborador ADD logradouro_cep VARCHAR(8) NULL;
END
ELSE
BEGIN
    ;THROW 50003, 'A tabela dbo.tb_colaborador não existe no banco conectado.', 1;
END;";

            return provider.ExecuteNonQueryAsync(sql);
        }

        public static Task EnsureAcessoSchemaAsync(DbProvider provider)
        {
            if (provider == null) throw new ArgumentNullException(nameof(provider));
            const string sql = @"
IF OBJECT_ID(N'dbo.tb_acesso_aluno', N'U') IS NULL
    CREATE TABLE dbo.tb_acesso_aluno (Id INT IDENTITY(1,1) PRIMARY KEY, aluno_id INT NOT NULL, entrada DATETIME2 NOT NULL, saida DATETIME2 NULL);
IF OBJECT_ID(N'dbo.tb_acesso_colaborador', N'U') IS NULL
    CREATE TABLE dbo.tb_acesso_colaborador (Id INT IDENTITY(1,1) PRIMARY KEY, colaborador_id INT NOT NULL, entrada DATETIME2 NOT NULL, saida DATETIME2 NULL);";
            return provider.ExecuteNonQueryAsync(sql);
        }

        /// <summary>
        /// Executa uma lista de arquivos SQL na ordem informada.
        /// </summary>
        public static async Task InitializeAsync(DbProvider provider, params string[] scriptPaths)
        {
            if (provider == null) throw new ArgumentNullException(nameof(provider));
            if (scriptPaths == null) throw new ArgumentNullException(nameof(scriptPaths));

            foreach (var path in scriptPaths)
            {
                if (string.IsNullOrWhiteSpace(path)) continue;
                if (!File.Exists(path)) throw new FileNotFoundException("Script SQL não encontrado", path);
                await provider.ExecuteScriptFileAsync(path);
            }
        }
    }
}
