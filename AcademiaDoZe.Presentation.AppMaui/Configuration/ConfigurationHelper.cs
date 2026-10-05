using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Hosting;
using AcademiaDoZe.Application;
using AcademiaDoZe.Application.Enums;
using AcademiaDoZe.Infrastructure.Data;
using AcademiaDoZe.Infrastructure.Repositories;
using AcademiaDoZe.Domain.Repositories;

namespace AcademiaDoZe.Presentation.AppMaui.Configuration
{
    public static class ConfigurationHelper
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            ArgumentNullException.ThrowIfNull(services);
            var repoConfig = new RepositoryConfig
            {
                DatabaseType = AppDatabaseType.SqlServer
            };

            services.AddSingleton(repoConfig);
            services.AddSingleton(serviceProvider =>
            {
                var connectionString = repoConfig.ConnectionString
                    ?? throw new InvalidOperationException("Conecte-se ao banco antes de usar os repositórios.");
                return new DbProvider("Microsoft.Data.SqlClient", connectionString);
            });
            services.AddScoped<ILogradouroRepository, LogradouroRepository>();
            services.AddScoped<IAlunoRepository, AlunoRepository>();
            services.AddScoped<IColaboradorRepository, ColaboradorRepository>();
            services.AddScoped<IMatriculaRepository, MatriculaRepository>();
            services.AddScoped<IAcessoAlunoRepository, AcessoAlunoRepository>();
            services.AddScoped<IAcessoColaboradorRepository, AcessoColaboradorRepository>();
            services.AddScoped<ITreinoRepository, TreinoRepository>();
            services.AddApplication();
        }
    }
}
