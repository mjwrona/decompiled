// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.Tracing.SafeStringFormat
// Assembly: Microsoft.VisualStudio.Services.Content.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC45E7D4-4445-41B3-8FA2-C13CD848D0F1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Common.dll

using System;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.Content.Common.Tracing
{
  public static class SafeStringFormat
  {
    internal static readonly IFormatProvider SafeFormat = (IFormatProvider) CultureInfo.InvariantCulture;

    public static string FormatSafe(string format, params object[] args) => SafeStringFormat.FormatSafe(SafeStringFormat.SafeFormat, format, args);

    public static string FormatSafe(IFormatProvider provider, string format, params object[] args)
    {
      if (args != null)
      {
        if (args.Length != 0)
        {
          try
          {
            return string.Format(provider, format, args);
          }
          catch (FormatException ex)
          {
            return format + SafeStringFormat.FormatSafe(" (" + ex.GetType().Name + ": " + ex.Message + ")");
          }
        }
      }
      return format;
    }

    public static string FormatSafe(this IFormattable formattable) => formattable.ToString((string) null, SafeStringFormat.SafeFormat);
  }
}
