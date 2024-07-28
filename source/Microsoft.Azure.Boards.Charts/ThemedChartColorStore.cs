// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.Charts.ThemedChartColorStore
// Assembly: Microsoft.Azure.Boards.Charts, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EABADF19-3537-403E-8E3C-4185CE6D1F3B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.Charts.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server;
using System.Drawing;

namespace Microsoft.Azure.Boards.Charts
{
  internal class ThemedChartColorStore : IChartColorStore
  {
    private ClientTheme theme;

    public ThemedChartColorStore(IVssRequestContext requestContext) => this.theme = requestContext.GetService<IThemingService>().GetRequestedTheme(requestContext);

    public Color BackgroundColor
    {
      get
      {
        Color color;
        return this.TryGetColorFromTheme("palette-neutral-0", out color) ? color : ChartCoreColors.DefaultBackground;
      }
    }

    public Color ForegroundColor
    {
      get
      {
        Color color;
        return this.TryGetColorFromTheme("palette-neutral-100", out color) ? color : ChartCoreColors.DefaultForeground;
      }
    }

    public Color FillColor
    {
      get
      {
        Color color;
        return this.TryGetColorFromTheme("palette-primary", out color) ? color : ChartCoreColors.DefaultFill;
      }
    }

    public Color LineColor
    {
      get
      {
        Color color;
        return this.TryGetColorFromTheme("palette-neutral-10", out color) ? color : ChartCoreColors.DefaultLine;
      }
    }

    public bool IsDarkTheme
    {
      get
      {
        ClientTheme theme = this.theme;
        return theme != null && theme.IsDark;
      }
    }

    private bool TryGetColorFromTheme(string colorSemantic, out Color color)
    {
      color = Color.Empty;
      string str;
      if (this.theme == null || !this.theme.Data.TryGetValue(colorSemantic, out str) || string.IsNullOrEmpty(str))
        return false;
      string[] strArray = str.Split(',');
      if (strArray.Length != 3)
        return false;
      try
      {
        color = Color.FromArgb(int.Parse(strArray[0]), int.Parse(strArray[1]), int.Parse(strArray[2]));
      }
      catch
      {
        return false;
      }
      return true;
    }
  }
}
