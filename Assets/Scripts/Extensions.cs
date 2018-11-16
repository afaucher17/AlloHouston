﻿using System;

namespace CRI.HelloHouston
{
    public static class Extensions
    {
        public static TEnum RandomEnumValue<TEnum>() where TEnum: struct, IConvertible, IComparable, IFormattable
        {
            if (!typeof(TEnum).IsEnum)
                throw new Exception("TEnum must be an enum.");
            var v = Enum.GetValues(typeof(TEnum));
            return (TEnum)v.GetValue(new Random().Next(v.Length));
        }
    }
}
