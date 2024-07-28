// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.Charts.ChartCoreColors
// Assembly: Microsoft.Azure.Boards.Charts, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EABADF19-3537-403E-8E3C-4185CE6D1F3B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.Charts.dll

using System.Drawing;

namespace Microsoft.Azure.Boards.Charts
{
  internal static class ChartCoreColors
  {
    public static Color DefaultBackground => ColorTranslator.FromHtml("#FFFFFF");

    public static Color DefaultForeground => ColorTranslator.FromHtml("#212121");

    public static Color DefaultFill => ChartAreaColors.BurndownCompleted;

    public static Color DefaultLine => Color.Gainsboro;
  }
}
