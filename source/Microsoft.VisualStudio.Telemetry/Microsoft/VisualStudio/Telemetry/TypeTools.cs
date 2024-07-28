// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.TypeTools
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using System;
using System.Globalization;

namespace Microsoft.VisualStudio.Telemetry
{
  internal static class TypeTools
  {
    public static bool IsNumericType(Type t) => t == typeof (byte) || t == typeof (sbyte) || t == typeof (Decimal) || t == typeof (double) || t == typeof (float) || t == typeof (short) || t == typeof (int) || t == typeof (long) || t == typeof (ushort) || t == typeof (uint) || t == typeof (ulong);

    public static bool TryConvertToUInt(object o, out uint result) => TypeTools.GuardConvert<uint>(new Func<object, uint>(Convert.ToUInt32), o, out result);

    public static bool TryConvertToInt(object o, out int result) => TypeTools.GuardConvert<int>(new Func<object, int>(Convert.ToInt32), o, out result);

    public static string ConvertToString(object o) => Convert.ToString(o, (IFormatProvider) CultureInfo.InvariantCulture);

    private static bool GuardConvert<T>(Func<object, T> convertFunc, object from, out T res)
    {
      res = default (T);
      try
      {
        res = convertFunc(from);
        return true;
      }
      catch (Exception ex)
      {
        switch (ex)
        {
          case FormatException _:
          case InvalidCastException _:
          case OverflowException _:
            break;
          default:
            throw;
        }
      }
      return false;
    }
  }
}
