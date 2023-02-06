using System.ComponentModel.DataAnnotations;

namespace FootStat.Data.Entities.Enums;

public enum EventType : byte
{
    [Display(Name = "Пасс")]
    Pass,
    [Display(Name = "Гол")]
    Goal,
    [Display(Name = "Потеря мяча")]
    Lost,
    [Display(Name = "Удар в створ")]
    ShootOnGoal,
    [Display(Name = "Удар мимо ворот")]
    ShootWide,
    [Display(Name = "Бросок аута")]
    Out,
    [Display(Name = "Подача углового")]
    Corner,
    [Display(Name = "Отбор мяча")]
    Interrupt,
    [Display(Name = "Выход на поле")]
    Entering,
    [Display(Name = "Уход с поля")]
    Exiting
}