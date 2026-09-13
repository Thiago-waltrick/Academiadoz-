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
