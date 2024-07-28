// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Reporting.DataServices.Model.ChartColors
// Assembly: Microsoft.TeamFoundation.Reporting.DataServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0871DF71-209E-4628-905A-D95195A70FEC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Reporting.DataServices.dll

using System.Text.RegularExpressions;

namespace Microsoft.TeamFoundation.Reporting.DataServices.Model
{
  public static class ChartColors
  {
    internal static bool IsSupportedBackgroundColor(string color) => Regex.IsMatch(color, "^#([0-9a-fA-F]{3}|[0-9a-fA-F]{6})$");
  }
}
