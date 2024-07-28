// Decompiled with JetBrains decompiler
// Type: System.TimeSpanEx
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using System.Globalization;

namespace System
{
  internal static class TimeSpanEx
  {
    public static TimeSpan Parse(string value, CultureInfo info) => TimeSpan.Parse(value, (IFormatProvider) info);

    public static bool TryParse(string value, CultureInfo info, out TimeSpan output) => TimeSpan.TryParse(value, (IFormatProvider) info, out output);

    public static string ToString(this TimeSpan timeSpan, CultureInfo info, string format) => timeSpan.ToString(format, (IFormatProvider) info);
  }
}
