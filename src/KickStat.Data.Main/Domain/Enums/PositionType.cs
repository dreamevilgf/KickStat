using System.ComponentModel.DataAnnotations;

namespace FootStat.Data.Entities.Enums;

public enum PositionType : byte
{
    [Display(Name = "Не выбрана")]
    None,
    [Display(Name = "Зона пенальти. Своя половина")]
    OwnPenaltyCenter,
    [Display(Name = "Зона правого углового. Своя половина")]
    OwnCornerRight,
    [Display(Name = "Зона левого углового. Своя половина")]
    OwnCornerLeft,
    [Display(Name = "Опорная зона центр. Своя половина")]
    OwnSupport,
    [Display(Name = "Опорная зона слева. Своя половина")]
    OwnLeft,
    [Display(Name = "Опорная зона справа. Своя половина")]
    OwnRight,
    [Display(Name = "Центральная зона центр")]
    Center,
    [Display(Name = "Центральная зона слева")]
    CenterLeft,
    [Display(Name = "Центральная зона справа")]
    CenterRight,
    [Display(Name = "Зона пенальти. Половина соперника")]
    OpponentPenaltyCenter,
    [Display(Name = "Зона правого углового. Половина соперника")]
    OpponentCornerRight,
    [Display(Name = "Зона левого уголового. Половина соперника")]
    OpponentCornerLeft,
    [Display(Name = "Опорная зона центр. Половина соперника")]
    OpponentSupport,
    [Display(Name = "Опорная зона слева. Половина соперника")]
    OpponentLeft,
    [Display(Name = "Опорная зона справа. Половина соперника")]
    OpponentRight,
}