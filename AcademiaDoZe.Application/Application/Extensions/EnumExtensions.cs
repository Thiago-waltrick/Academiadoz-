// Thiago Augusto Ruskowski Waltrick
using System;
using AcademiaDoZe.Domain.Enums;
using AcademiaDoZe.Application.Enums;

namespace AcademiaDoZe.Application.Extensions
{
    public static class EnumExtensions
    {
        // ColaboradorTipo
        public static AppColaboradorTipo ToApp(this ColaboradorTipo src)
        {
            return src switch
            {
                ColaboradorTipo.Administrador => AppColaboradorTipo.Administrador,
                ColaboradorTipo.Atendente => AppColaboradorTipo.Atendente,
                ColaboradorTipo.Instrutor => AppColaboradorTipo.Instrutor,
                ColaboradorTipo.Administrativo => AppColaboradorTipo.Administrativo,
                ColaboradorTipo.Gerente => AppColaboradorTipo.Gerente,
                ColaboradorTipo.Terceirizado => AppColaboradorTipo.Terceirizado,
                _ => throw new ArgumentOutOfRangeException(nameof(src), src, null)
            };
        }

        public static ColaboradorTipo ToDomain(this AppColaboradorTipo src)
        {
            return src switch
            {
                AppColaboradorTipo.Administrador => ColaboradorTipo.Administrador,
                AppColaboradorTipo.Atendente => ColaboradorTipo.Atendente,
                AppColaboradorTipo.Instrutor => ColaboradorTipo.Instrutor,
                AppColaboradorTipo.Administrativo => ColaboradorTipo.Administrativo,
                AppColaboradorTipo.Gerente => ColaboradorTipo.Gerente,
                AppColaboradorTipo.Terceirizado => ColaboradorTipo.Terceirizado,
                _ => throw new ArgumentOutOfRangeException(nameof(src), src, null)
            };
        }

        // ColaboradorVinculo
        public static AppColaboradorVinculo ToApp(this ColaboradorVinculo src)
        {
            return src switch
            {
                ColaboradorVinculo.CLT => AppColaboradorVinculo.CLT,
                ColaboradorVinculo.Estagio => AppColaboradorVinculo.Estagio,
                _ => throw new ArgumentOutOfRangeException(nameof(src), src, null)
            };
        }

        public static ColaboradorVinculo ToDomain(this AppColaboradorVinculo src)
        {
            return src switch
            {
                AppColaboradorVinculo.CLT => ColaboradorVinculo.CLT,
                AppColaboradorVinculo.Estagio => ColaboradorVinculo.Estagio,
                _ => throw new ArgumentOutOfRangeException(nameof(src), src, null)
            };
        }

        // MatriculaPlano
        public static AppMatriculaPlano ToApp(this MatriculaPlano src)
        {
            return src switch
            {
                MatriculaPlano.Mensal => AppMatriculaPlano.Mensal,
                MatriculaPlano.Trimestral => AppMatriculaPlano.Trimestral,
                MatriculaPlano.Semestral => AppMatriculaPlano.Semestral,
                MatriculaPlano.Anual => AppMatriculaPlano.Anual,
                _ => throw new ArgumentOutOfRangeException(nameof(src), src, null)
            };
        }

        public static MatriculaPlano ToDomain(this AppMatriculaPlano src)
        {
            return src switch
            {
                AppMatriculaPlano.Mensal => MatriculaPlano.Mensal,
                AppMatriculaPlano.Trimestral => MatriculaPlano.Trimestral,
                AppMatriculaPlano.Semestral => MatriculaPlano.Semestral,
                AppMatriculaPlano.Anual => MatriculaPlano.Anual,
                _ => throw new ArgumentOutOfRangeException(nameof(src), src, null)
            };
        }

        // MatriculaRestricoes
        public static AppMatriculaRestricoes ToApp(this MatriculaRestricoes src)
        {
            return (AppMatriculaRestricoes)src;
        }

        public static MatriculaRestricoes ToDomain(this AppMatriculaRestricoes src)
        {
            return (MatriculaRestricoes)src;
        }
    }
}
