// Thiago Augusto Ruskowski Waltrick
using Microsoft.Extensions.DependencyInjection;
using AcademiaDoZe.Application.Interfaces;
using AcademiaDoZe.Application.Services;

namespace AcademiaDoZe.Application
{
    public static class ApplicationDependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // Register application services
            services.AddScoped<IAlunoService, AlunoService>();
            services.AddScoped<IColaboradorService, ColaboradorService>();
            services.AddScoped<IMatriculaService, MatriculaService>();
            services.AddScoped<ILogradouroService, LogradouroService>();
            services.AddScoped<IAcessoAlunoService, AcessoAlunoService>();
            services.AddScoped<IAcessoColaboradorService, AcessoColaboradorService>();
            services.AddScoped<ITreinoService, TreinoService>();

            return services;
        }
    }
}
