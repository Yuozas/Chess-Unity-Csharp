using System;
using UnityEngine;

public static class Extensions
{
    public static void SetTrue(this ref bool boolean) => boolean = true;

    public static void SetFalse(this ref bool boolean) => boolean = false;

    public static bool ModulusZero(this int number, int from) => number % from == 0;

    public static bool Even(this int number) => number.ModulusZero(2);

    public static bool Odd(this int number) => !number.Even();
    public static bool InRange(this int number, int min, int max) => number >= min && number <= max;
    public static PieceColor Flip(this PieceColor color) => color == PieceColor.White ? PieceColor.Black : PieceColor.White;

    public static Color32 Blend(this Color32 color, Color32 backColor, int amount) => new Color32(
            color.r + backColor.r * 0.01 * amount > byte.MaxValue ? byte.MaxValue : (byte)(color.r + backColor.r * 0.01 * amount),
            color.g + backColor.g * 0.01 * amount > byte.MaxValue ? byte.MaxValue : (byte)(color.g + backColor.g * 0.01 * amount),
            color.b + backColor.b * 0.01 * amount > byte.MaxValue ? byte.MaxValue : (byte)(color.b + backColor.b * 0.01 * amount),
            color.a + backColor.a > byte.MaxValue ? byte.MaxValue : (byte)(color.a + backColor.a)
        );
}
[System.Serializable]
public class StringEvent : UnityEngine.Events.UnityEvent<string> { }