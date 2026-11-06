// Thiago Augusto Ruskowski Waltrick
using System;
using System.ComponentModel.DataAnnotations;

namespace AcademiaDoZe.Application.Enums
{
    [Flags]
    public enum AppMatriculaRestricoes
    {
        [Display(Name = "Nenhuma")]
        Nenhuma = 0,
        [Display(Name = "Piscina")]
        Piscina = 1 << 0,
        [Display(Name = "Sala de musculação")]
        SalaDeMusculacao = 1 << 1,
        [Display(Name = "Horário noturno")]
        HorarioNoturno = 1 << 2,
        [Display(Name = "Atividades específicas")]
        AtividadesEspecificas = 1 << 3,
        [Display(Name = "Diabetes")]
        Diabetes = 1 << 4,
        [Display(Name = "Pressão alta")]
        PressaoAlta = 1 << 5,
        [Display(Name = "Labirintite")]
        Labirintite = 1 << 6,
        [Display(Name = "Alergias")]
        Alergias = 1 << 7,
        [Display(Name = "Problemas respiratórios")]
        ProblemasRespiratorios = 1 << 8,
        [Display(Name = "Remédio contínuo")]
        RemedioContinuo = 1 << 9
    }
}
