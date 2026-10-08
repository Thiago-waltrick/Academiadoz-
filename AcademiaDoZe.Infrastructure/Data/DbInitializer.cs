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
END;";

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
