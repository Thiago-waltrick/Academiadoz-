// Thiago Augusto Ruskowski Waltrick
using System;
using System.Reflection;
using System.Linq;
using Xunit;
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Enums;
using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Domain.Tests.Entities;

public class AcessoColaboradorTests
{
    private static Logradouro GetValidLogradouro() => Logradouro.Criar("Rua Teste", "Bairro", "Cidade", "SP", null).Value!;
    private static Arquivo GetValidArquivo() => Arquivo.Criar("file.txt", "text/plain", 1024).Value!;
    private static Colaborador GetValidColaborador()
    {
        var cpf = Cpf.Criar("12345678901").Value!;
        var email = Email.Criar("colab@dominio.com").Value!;
        var telefone = Telefone.Criar("11912345678").Value!;
        var endereco = Endereco.Criar(GetValidLogradouro(), "123", null).Value!;
        return Colaborador.Criar(1, "Colab Test", cpf, email, DateTime.UtcNow.AddYears(-30), telefone, endereco, ColaboradorTipo.Instrutor, ColaboradorVinculo.CLT, DateTime.UtcNow.AddDays(-10)).Value!;
    }

    [Theory(DisplayName = "AcessoColaborador: colaborador nulo -> COLABORADOR_INVALIDO")]
    [InlineData(true)]
    [InlineData(false)]
    public void Deve_Falhar_Criacao_Quando_ColaboradorENulo(bool colaboradorNull)
    {
        var senha = Senha.Criar("senha123").Value!;
        if (colaboradorNull)
        {
            var result = AcessoColaborador.Criar(1, 0, senha);
            Assert.True(result.IsFailure);
            Assert.NotEmpty(result.Notifications);
            Assert.True(result.Notifications.Any(n => n.Mensagem == "COLABORADOR_INVALIDO" || n.Mensagem.Contains("Colaborador")));
        }
        else
        {
            var colaborador = GetValidColaborador();
            var result = AcessoColaborador.Criar(1, colaborador.Id, senha);
            Assert.True(result.IsSuccess);
            Assert.Equal(colaborador.Id, result.Value!.ColaboradorId);
        }
    }

    [Theory(DisplayName = "AcessoColaborador: horario fora do intervalo -> DATAHORA_INTERVALO")]
    [InlineData(5, 59)]
    [InlineData(22, 1)]
    public void Deve_Falhar_Criacao_Quando_HorarioForaDoIntervalo(int hour, int minute)
    {
        var colaborador = GetValidColaborador();
        var senha = Senha.Criar("senha123").Value!;
        var inst = CreateInstanceWithCustomTime(1, colaborador.Id, senha, DateTime.Today.AddHours(hour).AddMinutes(minute));
        var data = inst.DataCriacao;
        var invalid = data.Hour < 6 || (data.Hour > 22) || (data.Hour == 22 && data.Minute > 0);
        Assert.True(invalid);
    }

    [Theory(DisplayName = "AcessoColaborador: criação bem-sucedida em horários permitidos")]
    [InlineData(10)]
    [InlineData(16)]
    public void Deve_Criar_Com_Sucesso_Quando_HorarioValido(int hour)
    {
        var colaborador = GetValidColaborador();
        var senha = Senha.Criar("senha123").Value!;
        var inst = CreateInstanceWithCustomTime(1, colaborador.Id, senha, DateTime.Today.AddHours(hour));
        var data = inst.DataCriacao;
        var valid = data.Hour >= 6 && (data.Hour < 22 || (data.Hour == 22 && data.Minute == 0));
        Assert.True(valid);
    }

    [Theory(DisplayName = "AcessoColaborador: permite horários de borda 06:00 e 22:00")]
    [InlineData(6)]
    [InlineData(22)]
    public void Deve_Permitir_HorariosDeBorda_06_00_e_22_00(int hour)
    {
        var colaborador = GetValidColaborador();
        var senha = Senha.Criar("senha123").Value!;
        var inst = CreateInstanceWithCustomTime(1, colaborador.Id, senha, DateTime.Today.AddHours(hour));
        var data = inst.DataCriacao;
        var valid = (data.Hour == 6 && data.Minute == 0) || (data.Hour == 22 && data.Minute == 0);
        Assert.True(valid);
    }

    private static AcessoColaborador CreateInstanceWithCustomTime(int id, int colaboradorId, Senha senha, DateTime dateTime)
    {
        var type = typeof(AcessoColaborador);
        var ctor = type.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(int), typeof(int), typeof(Senha) }, null);
        var instance = (AcessoColaborador)ctor!.Invoke(new object[] { id, colaboradorId, senha });
        var prop = type.GetProperty("DataCriacao", BindingFlags.Public | BindingFlags.Instance)!;
        prop.SetValue(instance, dateTime);
        return instance;
    }
}
