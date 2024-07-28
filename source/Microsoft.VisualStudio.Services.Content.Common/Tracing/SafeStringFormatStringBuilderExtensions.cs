// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.Tracing.SafeStringFormatStringBuilderExtensions
// Assembly: Microsoft.VisualStudio.Services.Content.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC45E7D4-4445-41B3-8FA2-C13CD848D0F1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Common.dll

using System;
using System.Text;

namespace Microsoft.VisualStudio.Services.Content.Common.Tracing
{
  public static class SafeStringFormatStringBuilderExtensions
  {
    public static void AppendFormatSafe(
      this StringBuilder message,
      string format,
      params object[] args)
    {
      if (args == null || args.Length == 0)
      {
        message.Append(format);
      }
      else
      {
        StringBuilder stringBuilder = new StringBuilder();
        try
        {
          stringBuilder.AppendFormat(SafeStringFormat.SafeFormat, format, args);
          message.AppendFormat(SafeStringFormat.SafeFormat, format, args);
        }
        catch (FormatException ex)
        {
          message.Append(format);
          message.Append(SafeStringFormat.FormatSafe(" (" + ex.GetType().Name + ": " + ex.Message + ")"));
        }
      }
    }
  }
}
