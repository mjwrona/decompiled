// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.Tracing.ConsoleMessageUtil
// Assembly: Microsoft.VisualStudio.Services.Content.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC45E7D4-4445-41B3-8FA2-C13CD848D0F1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Common.dll

using System;
using System.Globalization;
using System.IO;

namespace Microsoft.VisualStudio.Services.Content.Common.Tracing
{
  public sealed class ConsoleMessageUtil
  {
    private static readonly object ConsoleColorLock = new object();

    public static void PrintErrorMessage(string format, params object[] args) => ConsoleMessageUtil.PrintMessage(ConsoleColor.Red, Console.Out, format, args);

    public static void PrintErrorMessage(string message) => ConsoleMessageUtil.PrintMessage(message, ConsoleColor.Red, Console.Out);

    public static void PrintWarningMessage(string message) => ConsoleMessageUtil.PrintMessage(message, ConsoleColor.Yellow, Console.Out);

    public static void PrintWarningMessage(string format, params object[] args) => ConsoleMessageUtil.PrintMessage(ConsoleColor.Yellow, Console.Out, format, args);

    private static void PrintMessage(
      ConsoleColor color,
      TextWriter writer,
      string format,
      params object[] args)
    {
      ConsoleMessageUtil.PrintMessage(SafeStringFormat.FormatSafe((IFormatProvider) CultureInfo.CurrentUICulture, format, args), color, writer);
    }

    private static void PrintMessage(string message, ConsoleColor color, TextWriter writer)
    {
      lock (ConsoleMessageUtil.ConsoleColorLock)
      {
        ConsoleColor foregroundColor = Console.ForegroundColor;
        Console.ForegroundColor = color;
        try
        {
          writer.WriteLine(message);
        }
        finally
        {
          Console.ForegroundColor = foregroundColor;
        }
      }
    }
  }
}
